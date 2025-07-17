using UnityEngine;
using Verse;

namespace JJK
{
    //public class SwimmingAnimationWorker : AnimationWorker
    //{
    //    private const float BaseRotation = 55f;
    //    private const float WobbleAmplitude = 5f;
    //    private const float WobbleFrequency = 1f;

    //    public SwimmingAnimationWorker(Verse.AnimationDef def, Pawn pawn, AnimationPart part, PawnRenderNode node)
    //        : base(def, pawn, part, node)
    //    {
    //    }

    //    public override float AngleAtTick(int tick, PawnDrawParms parms)
    //    {
    //        if (parms.facing == Rot4.East || parms.facing == Rot4.West)
    //        {
    //            float rotation = parms.facing == Rot4.West ? -BaseRotation : BaseRotation;
    //            float time = tick / 60f;
    //            float wobble = Mathf.Sin(time * WobbleFrequency * 2 * Mathf.PI) * WobbleAmplitude;
    //            return rotation + wobble;
    //        }
    //        return base.AngleAtTick(tick, parms);
    //    }
    //}
}
