using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace DrumBath;

public class JoyGiver_BathingAtDrumBath : JoyGiver
{
    private static List<Thing> candidates = [];

    public override Job TryGiveJob(Pawn pawn)
    {
        if (!Utl.TryFindDrumBathCell(pawn.Position, pawn, out var result))
        {
            return null;
        }

        var thing = result.GetThingList(pawn.Map).Find(delegate(Thing x)
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
        return new Job(def.jobDef, result, thing);
    }
}