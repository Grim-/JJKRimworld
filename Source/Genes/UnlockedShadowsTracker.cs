using System.Collections.Generic;
using Verse;

namespace JJK
{
    public class UnlockedShadowsTracker : IExposable
    {
        private List<ShikigamiDef> earnedShadows = new List<ShikigamiDef>();
        public List<ShikigamiDef> EarnedShadows => earnedShadows;

        public bool HasUnlockedShikigami(ShikigamiDef shikigamiDef)
        {
            return earnedShadows.Contains(shikigamiDef);
        }

        public void UnlockShikigami(ShikigamiDef shikigamiDef)
        {
            if (!HasUnlockedShikigami(shikigamiDef))
            {
                earnedShadows.Add(shikigamiDef);
            }
        }

        public void ExposeData()
        {
            Scribe_Collections.Look(ref earnedShadows, "earnedShadows", LookMode.Def);

            if (Scribe.mode == LoadSaveMode.PostLoadInit && earnedShadows == null)
            {
                earnedShadows = new List<ShikigamiDef>();
            }
        }
    }
}