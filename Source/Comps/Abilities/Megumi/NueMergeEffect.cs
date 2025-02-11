using Verse;

namespace JJK
{
    public class NueMergeEffect : ShikigamiMergeWorker
    {
        public override void ApplyMergeEffect(Pawn summon, ShikigamiMergeEffectDef effect)
        {
            base.ApplyMergeEffect(summon, effect);

            //give them electrical resistance and weakened ability
        }
    }
}