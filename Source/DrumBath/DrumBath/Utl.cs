using System;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace DrumBath;

public static class Utl
{
    public static bool TryFindDrumBathCell(IntVec3 root, Pawn searcher, out IntVec3 result)
    {
        if (searcher.Map.weatherManager.RainRate > 0.1f || !JoyUtility.EnjoyableOutsideNow(searcher))
        {
            Predicate<IntVec3> cellValidator = c =>
                c.Roofed(searcher.Map) &&
                c.GetThingList(searcher.Map).Any(x =>
                    x.def == ThingDefOf.DrumBath && searcher.CanReserveAndReach(x, PathEndMode.OnCell, Danger.None)) &&
                c.GetTemperature(searcher.Map) < searcher.GetStatValue(StatDefOf.ComfyTemperatureMax) &&
                c.GetTemperature(searcher.Map) > searcher.GetStatValue(StatDefOf.ComfyTemperatureMin) - 10f;
            Predicate<Region> validator = r =>
                !r.IsForbiddenEntirely(searcher) &&
                r.TryFindRandomCellInRegionUnforbidden(searcher, cellValidator, out _);
            var traverseParms = TraverseParms.For(searcher);
            if (CellFinder.TryFindClosestRegionWith(root.GetRegion(searcher.Map), traverseParms, validator, 300,
                    out var result2))
            {
                return CellFinder.RandomRegionNear(result2, 14, traverseParms, validator, searcher)
                    .TryFindRandomCellInRegionUnforbidden(searcher, cellValidator, out result);
            }

            result = root;
            return false;
        }

        Predicate<IntVec3> cellValidator2 = c =>
            !c.Roofed(searcher.Map) &&
            c.GetThingList(searcher.Map).Any(x =>
                x.def == ThingDefOf.DrumBath && searcher.CanReserveAndReach(x, PathEndMode.OnCell, Danger.None)) &&
            c.GetTemperature(searcher.Map) < searcher.GetStatValue(StatDefOf.ComfyTemperatureMax) &&
            c.GetTemperature(searcher.Map) > searcher.GetStatValue(StatDefOf.ComfyTemperatureMin) - 10f;
        Predicate<Region> validator2 = r =>
            !r.IsForbiddenEntirely(searcher) &&
            r.TryFindRandomCellInRegionUnforbidden(searcher, cellValidator2, out _);
        var traverseParms2 = TraverseParms.For(searcher);
        if (CellFinder.TryFindClosestRegionWith(root.GetRegion(searcher.Map), traverseParms2, validator2, 300,
                out var result3))
        {
            return CellFinder.RandomRegionNear(result3, 14, traverseParms2, validator2, searcher)
                .TryFindRandomCellInRegionUnforbidden(searcher, cellValidator2, out result);
        }

        Predicate<IntVec3> cellValidator3 = c =>
            c.GetThingList(searcher.Map).Any(x =>
                x.def == ThingDefOf.DrumBath && searcher.CanReserveAndReach(x, PathEndMode.OnCell, Danger.None)) &&
            c.GetTemperature(searcher.Map) < searcher.GetStatValue(StatDefOf.ComfyTemperatureMax) &&
            c.GetTemperature(searcher.Map) > searcher.GetStatValue(StatDefOf.ComfyTemperatureMin) - 10f;
        Predicate<Region> validator3 = r =>
            !r.IsForbiddenEntirely(searcher) &&
            r.TryFindRandomCellInRegionUnforbidden(searcher, cellValidator3, out _);
        var traverseParms3 = TraverseParms.For(searcher);
        if (CellFinder.TryFindClosestRegionWith(root.GetRegion(searcher.Map), traverseParms3, validator3, 300,
                out var result4))
        {
            return CellFinder.RandomRegionNear(result4, 14, traverseParms3, validator3, searcher)
                .TryFindRandomCellInRegionUnforbidden(searcher, cellValidator3, out result);
        }

        result = root;
        return false;
    }

    public static void MoteMaker_ThrowVapor(Vector3 loc, Map map, float size)
    {
        if (!loc.ShouldSpawnMotesAt(map) || map.moteCounter.SaturatedLowPriority)
        {
            return;
        }

        var obj = (MoteThrown)ThingMaker.MakeThing(RimWorld.ThingDefOf.Mote_Bombardment);
        obj.Scale = Rand.Range(1.5f, 2.5f) * size;
        obj.rotationRate = Rand.Range(-30f, 30f);
        obj.exactPosition = loc - new Vector3(-0.2f, 0f, -0.1f);
        obj.SetVelocity(Rand.Range(-20, 30), Rand.Range(0.3f, 0.5f));
        GenSpawn.Spawn(obj, loc.ToIntVec3(), map);
    }
}