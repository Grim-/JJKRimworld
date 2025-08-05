using System.Collections.Generic;
using Verse;

namespace JJK
{
    public class MergeTracker : IExposable
    {
        private Dictionary<ShikigamiDef, ShikigamiMergeTracker> mergedShikigami = new Dictionary<ShikigamiDef, ShikigamiMergeTracker>();

        public void MergeShikigami(ShikigamiDef original, ShikigamiDef target)
        {
            if (!mergedShikigami.ContainsKey(original))
            {
                mergedShikigami[original] = new ShikigamiMergeTracker(original, target);
            }
        }

        public void UnmergeShikigami(ShikigamiDef original)
        {
            mergedShikigami.Remove(original);
        }

        public bool IsShikigamiMerged(ShikigamiDef shikigami)
        {
            return mergedShikigami.ContainsKey(shikigami);
        }

        public ShikigamiMergeTracker GetMergeData(ShikigamiDef shikigami)
        {
            return mergedShikigami.TryGetValue(shikigami, out var tracker) ? tracker : null;
        }

        public void ExposeData()
        {
            Scribe_Collections.Look(ref mergedShikigami, "mergedShikigami", LookMode.Def, LookMode.Deep);

            if (Scribe.mode == LoadSaveMode.PostLoadInit && mergedShikigami == null)
            {
                mergedShikigami = new Dictionary<ShikigamiDef, ShikigamiMergeTracker>();
            }
        }
    }
}