using System.Collections.Generic;
using System.Linq;
using DrumBath;
using HarmonyLib;
using Verse;

namespace DrumBath_Harmony;

[HarmonyPatch(typeof(Pawn), nameof(Pawn.GetGizmos))]
internal class Pawn_GetGizmos
{
    public static void Postfix(ref IEnumerable<Gizmo> __result, Pawn __instance)
    {
        if (__result == null)
        {
            return;
        }

        var list = __result.ToList();
        var compDrumBathAnimalJobManager = __instance.TryGetComp<CompDrumBathAnimalJobManager>();
        if (compDrumBathAnimalJobManager != null)
        {
            foreach (var item in compDrumBathAnimalJobManager.CompGetGizmosExtra())
            {
                list.Add(item);
            }
        }

        __result = list;
    }
}