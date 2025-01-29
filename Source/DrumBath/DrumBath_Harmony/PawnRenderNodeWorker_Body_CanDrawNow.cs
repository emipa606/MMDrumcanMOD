using DrumBath;
using HarmonyLib;
using Verse;

namespace DrumBath_Harmony;

[HarmonyPatch(typeof(PawnRenderNodeWorker_Body), nameof(PawnRenderNodeWorker_Body.CanDrawNow))]
public class PawnRenderNodeWorker_Body_CanDrawNow
{
    public static void Postfix(ref bool __result, PawnDrawParms parms)
    {
        if (!__result)
        {
            return;
        }

        var curJob = parms.pawn.CurJob;
        if (curJob?.def != JobDefOf.Job_BathingAtDrumBath ||
            !parms.pawn.health.hediffSet.HasHediff(HediffDefOf.Hed_BathingAtDrumBathPassive))
        {
            return;
        }

        __result = false;
    }
}