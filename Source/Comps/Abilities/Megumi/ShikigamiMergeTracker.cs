using Verse;

namespace JJK
{
    public class ShikigamiMergeTracker : IExposable
    {
        public ShikigamiDef OriginalShikigami;
        public ShikigamiDef MergedWithShikigami;
        public ShikigamiMergeEffectDef AppliedEffect;
        private ShikigamiMergeWorker worker;

        public ShikigamiMergeTracker()
        {
        }

        public ShikigamiMergeTracker(ShikigamiDef original, ShikigamiDef mergedWith, ShikigamiMergeEffectDef effect)
        {
            OriginalShikigami = original;
            MergedWithShikigami = mergedWith;
            AppliedEffect = effect;
            worker = effect.CreateWorker();
        }

        public ShikigamiMergeWorker Worker => worker;

        public void ExposeData()
        {
            Scribe_Defs.Look(ref OriginalShikigami, "originalShikigami");
            Scribe_Defs.Look(ref MergedWithShikigami, "mergedWithShikigami");
            Scribe_Defs.Look(ref AppliedEffect, "appliedEffect");

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                worker = AppliedEffect.CreateWorker();
            }
        }
    }
}