using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace DrumBath;

public class AnimalGraphicSetter : DefModExtension
{
    public List<PawnKindLifeStage> GraphicLifeStages = [];
    public Vector3 Offset = new Vector3(0f, 0f, 0f);
}