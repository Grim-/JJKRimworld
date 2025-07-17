using UnityEngine;
using Verse;

namespace JJK
{
    //public class IdleBobbingAnimationWorker : BaseAnimationWorker
    //{
    //    private const float WobbleAmplitude = 5f;
    //    private const float WobbleFrequency = 1f;

    //    public override float AngleAtTick(int tick, Verse.AnimationDef def, PawnRenderNode node, AnimationPart part, PawnDrawParms parms)
    //    {
    //        return 0;
    //    }

    //    public override bool Enabled(Verse.AnimationDef def, PawnRenderNode node, AnimationPart part, PawnDrawParms parms)
    //    {
    //        return true;
    //    }

    //    public override GraphicStateDef GraphicStateAtTick(int tick, Verse.AnimationDef def, PawnRenderNode node, AnimationPart part, PawnDrawParms parms)
    //    {
    //       base.GraphicStateAtTick(tick, def, node, part, parms);
    //    }


    //    //public override Vector3 OffsetAtTick(int tick, PawnDrawParms parms)
    //    //{
    //    //    float progress = (float)(tick % this.def.durationTicks) / this.def.durationTicks;
    //    //    float bobAmount = Mathf.Sin(progress * Mathf.PI * 2) * 0.05f; // Adjust amplitude as needed
    //    //    return new Vector3(1f, 0, 0f);
    //    //}

    //    public override Vector3 OffsetAtTick(int tick, Verse.AnimationDef def, PawnRenderNode node, AnimationPart part, PawnDrawParms parms)
    //    {
    //        Vector3 baseOffset = base.OffsetAtTick(tick, def, node, part, parms);
    //        float time = tick / 60f;
    //        float wobble = Mathf.Sin(time * WobbleFrequency * 2 * Mathf.PI) * WobbleAmplitude;
    //        return new Vector3(0, 0, wobble);
    //    }

    //    public override void PostDraw(Verse.AnimationDef def, PawnRenderNode node, AnimationPart part, PawnDrawParms parms, Matrix4x4 matrix)
    //    {
           
    //    }

    //    public override Vector3 ScaleAtTick(int tick, Verse.AnimationDef def, PawnRenderNode node, AnimationPart part, PawnDrawParms parms)
    //    {
          
    //    }
    //}
}
