using System;
using RimWorld;
using Verse;

namespace DrumBath
{
    public class HediffCompProperties_DrumBathHygiene : HediffCompProperties
    {
        /// <summary>
        /// Hygiene restored per tick. 0.0005 fills an empty gauge in 2000 ticks, that is half the
        /// length of a bath (joyDuration 4000): the pawn climbs out clean without the bath being
        /// reduced to a quick shower.
        /// </summary>
        public float cleanPerTick = 0.0005f;

        /// <summary>How often the onlookers' gaze is re-checked, in ticks.</summary>
        public int privacyCheckInterval = 300;

        public HediffCompProperties_DrumBathHygiene()
        {
            compClass = typeof(HediffComp_DrumBathHygiene);
        }
    }

    /// <summary>
    /// Wires the drum bath into Dubs Bad Hygiene.
    ///
    /// WHY ON A HEDIFF. The drum bath mod applies `Hed_BathingAtDrumBathPassive` for the whole
    /// length of the bath and removes it on the way out. That is already an exact marker for
    /// "this pawn is in the water": hooking onto it avoids patching its bath driver's `tickAction`
    /// with a sledgehammer — a lambda locked inside an iterator, which would break on its first
    /// update.
    ///
    /// Everything touching DBH goes through <see cref="DbhBridge"/>: without DBH, this component
    /// does nothing and says nothing.
    /// </summary>
    public class HediffComp_DrumBathHygiene : HediffComp
    {
        private bool started;
        private int ticks;

        // Runtime only, and deliberately outside CompExposeData: a delegate cannot be scribed, and
        // a save loaded mid-bath has to bind again anyway, against whatever Dubs Bad Hygiene is
        // loaded that time. Both fields come back false/null on load, which re-runs the binding.
        private bool cleanBound;
        private Action<float> clean;

        private HediffCompProperties_DrumBathHygiene Props
            => (HediffCompProperties_DrumBathHygiene)props;

        public override void CompPostTick(ref float severityAdjustment)
        {
            var pawn = Pawn;
            if (pawn == null || !DbhBridge.Available) return;

            // Entry effects happen on the first tick rather than in CompPostMake: when the hediff
            // is created, the pawn is not yet guaranteed to be spawned on the map.
            if (!started)
            {
                started = true;
                OnEnterBath(pawn);
            }

            // The need is looked up once per bath, not once per tick. A pawn without a hygiene need
            // leaves `clean` null for good, which is the quiet exit an animal takes.
            if (!cleanBound)
            {
                cleanBound = true;
                clean = DbhBridge.ResolveCleanAction(pawn);
            }
            clean?.Invoke(Props.cleanPerTick);

            ticks++;
            if (ticks % Props.privacyCheckInterval == 0)
            {
                DbhBridge.BathingPrivacy(pawn);
            }
        }

        private void OnEnterBath(Pawn pawn)
        {
            var bath = FindBath(pawn);

            // Cold = the drum is no longer burning. The drum bath mod heats it through
            // CompRefuelable: no more wood, no more hot water — and DBH already has both the
            // "hot bath" and "cold bath" thoughts.
            var cold = true;
            var refuelable = bath?.TryGetComp<CompRefuelable>();
            if (refuelable != null) cold = !refuelable.HasFuel;

            DbhBridge.WaterTemp(pawn, cold);
            DbhBridge.BathroomThought(pawn, bath);
            DbhBridge.ClearSoakingWet(pawn);
            DbhBridge.BathingPrivacy(pawn);
        }

        public override void CompPostPostRemoved()
        {
            base.CompPostPostRemoved();

            var pawn = Pawn;
            if (pawn == null || !DbhBridge.Available) return;

            // Climbing out of a bath: the mud carried on the body has no reason to remain. DBH
            // does the same at the end of its own baths.
            pawn.filth?.carriedFilth?.Clear();
            DbhBridge.BathingPrivacy(pawn);
        }

        /// <summary>
        /// The drum this pawn occupies. We look for it under their feet: the mod lays the pawn
        /// down inside it, so the building sits on their cell.
        /// </summary>
        private static Thing FindBath(Pawn pawn)
        {
            if (!pawn.Spawned) return null;

            var things = pawn.Position.GetThingList(pawn.Map);
            for (var i = 0; i < things.Count; i++)
            {
                if (things[i].def.defName == "DrumBath") return things[i];
            }
            return null;
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look(ref started, "started", false);
            Scribe_Values.Look(ref ticks, "ticks", 0);
        }
    }
}
