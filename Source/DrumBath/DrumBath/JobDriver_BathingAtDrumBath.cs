using System.Collections.Generic;
using System.Diagnostics;
using RimWorld;
using Verse;
using Verse.AI;

namespace DrumBath;

public class JobDriver_BathingAtDrumBath : JobDriver_VisitJoyThing
{
    private const float BaseHediffChange = 0.001f;

    private Thing SpringThing => job.GetTarget(TargetIndex.B).Thing;

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return true;
    }

    [DebuggerHidden]
    protected override IEnumerable<Toil> MakeNewToils()
    {
        yield return new Toil().FailOnDestroyedNullOrForbidden(TargetIndex.B);
        yield return Toils_Reserve.Reserve(TargetIndex.B);
        yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.OnCell);
        yield return Toils_Reserve.Reserve(TargetIndex.A);
        yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.OnCell);
        var toil = new Toil
        {
            initAction = delegate
            {
                pawn.health.AddHediff(HediffDefOf.Hed_BathingAtDrumBathPassive);
                pawn.jobs.posture = PawnPosture.Standing;
                if (pawn.RaceProps.Humanlike)
                {
                    pawn.Drawer.renderer.SetAllGraphicsDirty();
                }
            },
            tickIntervalAction = delegate(int delta)
            {
                var jobPawn = pawn;
                if (jobPawn.RaceProps.Humanlike)
                {
                    jobPawn.GainComfortFromCellIfPossible(delta);
                    PawnUtility.GainComfortFromThingIfPossible(jobPawn, job.targetB.Thing, delta);
                    var extraJoyGainFactor = 1.5f;
                    JoyUtility.JoyTickCheckEnd(jobPawn, delta, JoyTickFullJoyAction.EndJob, extraJoyGainFactor);
                }

                HealthUtility.AdjustSeverity(jobPawn, HediffDefOf.Hed_BathingAtDrumBath, BaseHediffChange);
                HealthUtility.AdjustSeverity(jobPawn, RimWorld.HediffDefOf.Hypothermia, -0.0001f);
            }
        };
        toil.AddFinishAction(delegate
        {
            if (pawn.health.hediffSet.HasHediff(HediffDefOf.Hed_BathingAtDrumBathPassive))
            {
                pawn.health.RemoveHediff(
                    pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Hed_BathingAtDrumBathPassive));
            }

            if (!pawn.RaceProps.Humanlike)
            {
                return;
            }

            JoyUtility.TryGainRecRoomThought(pawn);
            pawn.Drawer.renderer.SetAllGraphicsDirty();
        });
        toil.defaultCompleteMode = ToilCompleteMode.Delay;
        toil.defaultDuration = pawn.RaceProps.Humanlike ? job.def.joyDuration : (int)(job.def.joyDuration * 0.5f);
        yield return toil;
    }

    public override string GetReport()
    {
        if (pawn.Position.Roofed(Map))
        {
            return "DrBa.relaxing".Translate();
        }

        if (Map.gameConditionManager.ConditionIsActive(GameConditionDefOf.Eclipse))
        {
            return "DrBa.relaxingEclipse".Translate();
        }

        if (Map.gameConditionManager.ConditionIsActive(GameConditionDefOf.Aurora))
        {
            return "DrBa.relaxingAurora".Translate();
        }

        var num = GenCelestial.CurCelestialSunGlow(Map);
        if (num < 0.1f)
        {
            return "DrBa.relaxingStars".Translate();
        }

        if (num >= 0.65f)
        {
            return "DrBa.relaxingClouds".Translate();
        }

        return GenLocalDate.DayPercent(pawn) < 0.5f
            ? "DrBa.relaxingSunrise".Translate()
            : "DrBa.relaxingSunset".Translate();
    }

    protected override void WaitTickAction(int delta)
    {
        pawn.GainComfortFromCellIfPossible(delta);
        var obj = pawn;
        const float extraJoyGainFactor = 1.3f;
        JoyUtility.JoyTickCheckEnd(obj, delta, JoyTickFullJoyAction.EndJob, extraJoyGainFactor, (Building)SpringThing);
    }
}