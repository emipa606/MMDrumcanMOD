using RimWorld;
using UnityEngine;
using Verse;

namespace DrumBath;

public class Building_DrumBath : Building
{
    public float BaseOffsetZ;

    public float fuelConsumePerTick;
    private int LastMoteTick;

    private int NextMoteTick = Mathf.RoundToInt(Random.value * 500f);

    public CompRefuelable RefuelableComp => GetComp<CompRefuelable>();

    public override void Tick()
    {
        base.Tick();
        var ticksGame = Find.TickManager.TicksGame;
        if (ticksGame <= LastMoteTick + NextMoteTick)
        {
            return;
        }

        LastMoteTick = ticksGame;
        NextMoteTick = Mathf.RoundToInt(200f + (Random.value * 200f));
        Utl.MoteMaker_ThrowVapor(Position.ToVector3(), Map, 1f);
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref LastMoteTick, "LastMoteTick");
    }

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        var comp = GetComp<CompDrumBathZAdjuster>();
        if (comp != null)
        {
            BaseOffsetZ = comp.Props.BaseOffsetZ;
        }

        var comp2 = GetComp<CompRefuelable>();
        if (comp2 != null)
        {
            fuelConsumePerTick = comp2.Props.fuelConsumptionRate / GenDate.TicksPerDay;
        }
    }
}