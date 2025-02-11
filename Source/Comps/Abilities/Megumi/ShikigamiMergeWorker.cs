using Verse;

namespace JJK
{
    public abstract class ShikigamiMergeWorker
    {
        public virtual void ApplyMergeEffect(Pawn summon, ShikigamiMergeEffectDef effect)
        {
        }

        public virtual void RemoveMergeEffect(Pawn summon, ShikigamiMergeEffectDef effect)
        {
        }
    }
}