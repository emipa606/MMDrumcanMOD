using System;
using System.Collections.Generic;
using System.Diagnostics;
using RimWorld;
using Verse;
using Verse.AI;

namespace DrumBath;

public class CompDrumBathAnimalJobManager : ThingComp
{
    public int lastBathingTick = -1;

    public Pawn Me;

    public CompProperties_DrumBathAnimalJobManager Props => (CompProperties_DrumBathAnimalJobManager)props;

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Values.Look(ref lastBathingTick, "lastBathingTick");
    }

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        base.PostSpawnSetup(respawningAfterLoad);
        Me = parent as Pawn;
    }

    public override void CompTickRare()
    {
        base.CompTickRare();
        if (Props.BathingMTBHours <= 0f || !CanJoyNow() || Find.TickManager.TicksGame <
            lastBathingTick + Math.Round(Props.BathingMTBHours * 2500f))
        {
            return;
        }

        Rand.PushState();
        var salt = Gen.HashCombineInt(parent.thingIDNumber, 26504059);
        Rand.Seed = parent.RandSeedForHour(salt);
        var num = Rand.MTBEventOccurs(Props.BathingMTBHours, 2500f, 2500f);
        Rand.PopState();
        if (!num)
        {
            return;
        }

        var job = TryGiveJob(Me);
        if (job == null)
        {
            return;
        }

        lastBathingTick = Find.TickManager.TicksGame;
        Me.jobs.StartJob(job, JobCondition.Incompletable);
    }

    private bool CanJoyNow()
    {
        if (Me is not { Spawned: true } || Me.Dead || Me.Downed || Me.InAggroMentalState || !Me.Awake())
        {
            return false;
        }

        return Me.CurJob == null || Me.CurJob.def.isIdle;
    }

    private Job TryGiveJob(Pawn pawn)
    {
        if (!Utl.TryFindDrumBathCell(pawn.Position, pawn, out var c))
        {
            return null;
        }

        var thing = c.GetThingList(pawn.Map).Find(delegate(Thing x)
        {
            if (x.def != ThingDefOf.DrumBath)
            {
                return false;
            }

            var compRefuelable = x.TryGetComp<CompRefuelable>();
            if (compRefuelable != null)
            {
                return !(compRefuelable.FuelPercentOfMax < 0.1f);
            }

            return true;
        });
        return new Job(JobDefOf.Job_BathingAtDrumBath, c, thing);
    }

    [DebuggerHidden]
    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
        foreach (var item in base.CompGetGizmosExtra())
        {
            yield return item;
        }

        if (!Prefs.DevMode || Me.Faction is not { IsPlayer: true } || !Me.RaceProps.Animal || Me.Downed ||
            Me.InMentalState)
        {
            yield break;
        }

        yield return new Command_Action
        {
            action = delegate
            {
                if (Me.CurJobDef == JobDefOf.Job_BathingAtDrumBath)
                {
                    return;
                }

                var job = TryGiveJob(Me);
                if (job == null)
                {
                    return;
                }

                lastBathingTick = Find.TickManager.TicksGame;
                Me.jobs.StartJob(job, JobCondition.Incompletable);
            },
            defaultLabel = "DrBa.drumBath".Translate(),
            defaultDesc = "DrBa.forceBathing".Translate()
        };
    }
}