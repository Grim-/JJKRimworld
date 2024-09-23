using UnityEngine;
using Verse;

namespace JJK
{
    public class IdleBobbingAnimationWorker : AnimationWorker
    {
        private const float WobbleAmplitude = 5f;
        private const float WobbleFrequency = 1f;

        public IdleBobbingAnimationWorker(Verse.AnimationDef def, Pawn pawn, AnimationPart part, PawnRenderNode node)
            : base(def, pawn, part, node)
        {
        }

        //public override Vector3 OffsetAtTick(int tick, PawnDrawParms parms)
        //{
        //    float progress = (float)(tick % this.def.durationTicks) / this.def.durationTicks;
        //    float bobAmount = Mathf.Sin(progress * Mathf.PI * 2) * 0.05f; // Adjust amplitude as needed
        //    return new Vector3(1f, 0, 0f);
        //}

        public override Vector3 OffsetAtTick(int tick, PawnDrawParms parms)
        {
            Vector3 baseOffset = base.OffsetAtTick(tick, parms);
            float time = tick / 60f;
            float wobble = Mathf.Sin(time * WobbleFrequency * 2 * Mathf.PI) * WobbleAmplitude;
            return new Vector3(0, 0, wobble);
        }

    }
}
