using System;
using System.Reflection;
using RimWorld;
using Verse;

namespace DrumBath
{
    /// <summary>
    /// Bridge to Dubs Bad Hygiene, entirely through reflection.
    ///
    /// Why not a direct reference: DBH is a **soft** dependency. If the user does not have it,
    /// this mod must keep quiet rather than fail while the assembly loads. And the members
    /// targeted here are internal to DBH (`SanitationUtil`, `PrivacyUtil`): a hard reference
    /// would break the first time its author reworks them, whereas here we simply degrade.
    ///
    /// Everything is resolved once, on first access, and cached. If anything at all is missing,
    /// <see cref="Available"/> stays false and the component goes inert.
    /// </summary>
    public static class DbhBridge
    {
        private static bool resolved;
        private static bool available;

        private static Type needHygieneType;
        private static MethodInfo cleanMethod;          // Need_Hygiene.clean(float)
        private static MethodInfo bathingPrivacyLos;    // PrivacyUtil.BathingPrivacyLOS(Pawn, float)
        private static MethodInfo waterTempCheck;       // SanitationUtil.WaterTempCheck(Pawn, bool, bool)
        private static MethodInfo applyBathroomThought; // SanitationUtil.ApplyBathroomThought(Pawn, Thing)
        private static FieldInfo soakingWetField;       // DubDef.SoakingWet

        public static bool Available
        {
            get
            {
                Resolve();
                return available;
            }
        }

        private static void Resolve()
        {
            if (resolved) return;
            resolved = true;

            try
            {
                var asm = FindBadHygieneAssembly();
                if (asm == null) return;

                needHygieneType = asm.GetType("DubsBadHygiene.Need_Hygiene");
                cleanMethod = needHygieneType?.GetMethod("clean",
                    BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(float) }, null);

                var privacyUtil = asm.GetType("DubsBadHygiene.PrivacyUtil");
                bathingPrivacyLos = privacyUtil?.GetMethod("BathingPrivacyLOS",
                    BindingFlags.Public | BindingFlags.Static);

                var sanitationUtil = asm.GetType("DubsBadHygiene.SanitationUtil");
                waterTempCheck = sanitationUtil?.GetMethod("WaterTempCheck",
                    BindingFlags.Public | BindingFlags.Static);
                applyBathroomThought = sanitationUtil?.GetMethod("ApplyBathroomThought",
                    BindingFlags.Public | BindingFlags.Static);

                var dubDef = asm.GetType("DubsBadHygiene.DubDef");
                soakingWetField = dubDef?.GetField("SoakingWet",
                    BindingFlags.Public | BindingFlags.Static);

                // Cleaning is the heart of the mod: without it, nothing else is worth attempting.
                // The rest is comfort, and every one of those calls guards itself.
                available = needHygieneType != null && cleanMethod != null;

                if (!available)
                {
                    Log.Warning("[Drum Bath Hygiene] Dubs Bad Hygiene found, but Need_Hygiene.clean "
                              + "could not be resolved. The drum bath will not wash anyone.");
                }
            }
            catch (Exception ex)
            {
                available = false;
                Log.Warning("[Drum Bath Hygiene] could not bridge to Dubs Bad Hygiene: " + ex.Message);
            }
        }

        private static Assembly FindBadHygieneAssembly()
        {
            foreach (var pack in LoadedModManager.RunningModsListForReading)
            {
                foreach (var asm in pack.assemblies.loadedAssemblies)
                {
                    if (asm.GetName().Name == "BadHygiene") return asm;
                }
            }
            return null;
        }

        /// <summary>
        /// Binds this pawn's hygiene need to a delegate that fills it. Returns null if DBH is
        /// absent, or if the pawn has no such need at all — an animal in the bath, for one.
        ///
        /// BOUND ONCE, NOT CALLED THROUGH REFLECTION. `Thing.DoTick` calls `Tick()` on every tick
        /// without condition, so the component's own tick runs sixty times a second for as long as
        /// a pawn is soaking. A `MethodInfo.Invoke` there would mean a reflected call, a boxed
        /// float and a fresh argument array each time. A delegate closed over the need costs an
        /// ordinary call, and the lookup happens once per bath.
        /// </summary>
        public static Action<float> ResolveCleanAction(Pawn pawn)
        {
            if (!Available || pawn?.needs == null) return null;

            // Indexed rather than List.Find: no delegate, and this runs while the pawn is being
            // put into the bath, where the need list is the only thing we are after.
            var needs = pawn.needs.AllNeeds;
            Need found = null;
            for (var i = 0; i < needs.Count; i++)
            {
                if (needHygieneType.IsInstanceOfType(needs[i]))
                {
                    found = needs[i];
                    break;
                }
            }
            if (found == null) return null;

            try
            {
                return (Action<float>)Delegate.CreateDelegate(typeof(Action<float>), found, cleanMethod);
            }
            catch (Exception ex)
            {
                Log.WarningOnce("[Drum Bath Hygiene] could not bind Need_Hygiene.clean: " + ex.Message,
                    0x5D2B12);
                return null;
            }
        }

        /// <summary>
        /// Makes onlookers react: in DBH, washing in sight of others is a subject in itself.
        /// </summary>
        public static void BathingPrivacy(Pawn pawn)
            => TryInvoke(bathingPrivacyLos, new object[] { pawn, 6f });

        /// <summary>Hot or cold bath thought, depending on whether the drum is still heating.</summary>
        public static void WaterTemp(Pawn pawn, bool cold)
            => TryInvoke(waterTempCheck, new object[] { pawn, cold, true });

        /// <summary>Room-related thought: clean, shared or impressive bathroom.</summary>
        public static void BathroomThought(Pawn pawn, Thing fixture)
            => TryInvoke(applyBathroomThought, new object[] { pawn, fixture });

        /// <summary>
        /// Removes the "soaking wet" memory. Climbing out of a bath means being wet by definition:
        /// keeping the malus would be absurd, and DBH removes it itself after its own baths.
        /// </summary>
        public static void ClearSoakingWet(Pawn pawn)
        {
            if (!Available || soakingWetField == null) return;
            try
            {
                if (soakingWetField.GetValue(null) is ThoughtDef def)
                {
                    pawn?.needs?.mood?.thoughts?.memories?.RemoveMemoriesOfDef(def);
                }
            }
            catch (Exception ex)
            {
                Log.WarningOnce("[Drum Bath Hygiene] ClearSoakingWet failed: " + ex.Message, 0x5D2B11);
            }
        }

        private static void TryInvoke(MethodInfo method, object[] args)
        {
            if (!Available || method == null) return;
            try
            {
                method.Invoke(null, args);
            }
            catch (Exception ex)
            {
                Log.WarningOnce("[Drum Bath Hygiene] " + method.Name + " failed: " + ex.Message,
                    method.Name.GetHashCode());
            }
        }
    }
}
