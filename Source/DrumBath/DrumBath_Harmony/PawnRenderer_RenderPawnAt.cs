using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DrumBath;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace DrumBath_Harmony;

[HarmonyPatch]
internal class PawnRenderer_RenderPawnAt
{
    private static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.Method(typeof(PawnRenderer), nameof(PawnRenderer.RenderPawnAt));
        yield return AccessTools.Method(typeof(PawnRenderer), "ParallelPreRenderPawnAt");
    }

    public static void Prefix(ref Vector3 drawLoc, ref Rot4? rotOverride, Pawn ___pawn)
    {
        if (___pawn == null || ___pawn.Dead || ___pawn.Map == null ||
            !___pawn.health.hediffSet.HasHediff(HediffDefOf.Hed_BathingAtDrumBathPassive))
        {
            return;
        }

        var thing = ___pawn.CurJob.targetB.Thing;
        if (thing is not Building_DrumBath buildingDrumBath)
        {
            return;
        }

        rotOverride = Rot4.South;
        var altLayer = (AltitudeLayer)Mathf.Max((int)thing.def.altitudeLayer, 16);
        if (___pawn.RaceProps.Humanlike)
        {
            drawLoc = thing.Position.ToVector3ShiftedWithAltitude(altLayer);
            drawLoc.y += 0.0678f;
        }
        else
        {
            drawLoc = thing.Position.ToVector3ShiftedWithAltitude(altLayer);
            drawLoc.y += 0.0878f;
            if (Enumerable.FirstOrDefault(___pawn.def.modExtensions, x => x is AnimalGraphicSetter) is
                AnimalGraphicSetter
                animalGraphicSetter)
            {
                drawLoc += animalGraphicSetter.Offset;
            }
        }

        drawLoc.z += buildingDrumBath.BaseOffsetZ;
    }
}