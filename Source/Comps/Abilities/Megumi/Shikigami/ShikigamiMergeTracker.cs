using Verse;

namespace JJK
{
    public class ShikigamiMergeTracker : IExposable
    {
        public ShikigamiDef OriginalShikigami;
        public ShikigamiDef MergedInto;
        public int MergeTick;

        public ShikigamiMergeTracker()
        {
        }

        public ShikigamiMergeTracker(ShikigamiDef original, ShikigamiDef mergedInto)
        {
            OriginalShikigami = original;
            MergedInto = mergedInto;
            MergeTick = Find.TickManager.TicksGame;
        }

        public void ExposeData()
        {
            Scribe_Defs.Look(ref OriginalShikigami, "originalShikigami");
            Scribe_Defs.Look(ref MergedInto, "mergedInto");
            Scribe_Values.Look(ref MergeTick, "mergeTick");
        }
    }
}