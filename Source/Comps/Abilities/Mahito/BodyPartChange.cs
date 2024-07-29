using Verse;

namespace JJK
{
    public class BodyPartChange : IExposable
    {
        public BodyPartDef BodyPartDef;
        public int BodyPartIndex;
        public float MaintainCost;
        public BodyPartDef OriginalPartDef;
        public HediffDef NewHediffDef;
        public HediffDef OriginalHediffDef;
        public BodyPartRecord BodyPart;

        public void ExposeData()
        {
            Scribe_Defs.Look(ref BodyPartDef, "bodyPartDef");
            Scribe_Values.Look(ref BodyPartIndex, "bodyPartIndex");
            Scribe_Values.Look(ref MaintainCost, "maintainCost");
            Scribe_Defs.Look(ref OriginalPartDef, "originalPartDef");
            Scribe_Defs.Look(ref OriginalHediffDef, "originalHediffDef");
            Scribe_Defs.Look(ref NewHediffDef, "newHediffDef");
        }
    }
}