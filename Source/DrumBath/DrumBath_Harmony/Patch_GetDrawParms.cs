using DrumBath;
using HarmonyLib;
using Verse;

namespace DrumBath_Harmony;

[HarmonyPatch(typeof(PawnRenderer), "GetDrawParms")]
public class Patch_GetDrawParms
{
    public static void Postfix(ref PawnDrawParms __result, Pawn ___pawn)
    {
        var curJob = ___pawn.CurJob;
        if (curJob?.def != JobDefOf.Job_BathingAtDrumBath ||
            !___pawn.health.hediffSet.HasHediff(HediffDefOf.Hed_BathingAtDrumBathPassive))
        {
            return;
        }

        __result.flags &= ~PawnRenderFlags.Headgear;
        __result.flags &= ~PawnRenderFlags.Clothes;
    }
}