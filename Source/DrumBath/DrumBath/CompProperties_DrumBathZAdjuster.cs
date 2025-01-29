using Verse;

namespace DrumBath;

public class CompProperties_DrumBathZAdjuster : CompProperties
{
    public float BaseOffsetZ;

    public CompProperties_DrumBathZAdjuster()
    {
        compClass = typeof(CompDrumBathZAdjuster);
    }
}