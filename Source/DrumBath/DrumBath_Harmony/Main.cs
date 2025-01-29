using System.Reflection;
using HarmonyLib;
using Verse;

namespace DrumBath_Harmony;

[StaticConstructorOnStartup]
internal class Main
{
    static Main()
    {
        new Harmony("DrumBath.HarmonyPatch").PatchAll(Assembly.GetExecutingAssembly());
    }
}