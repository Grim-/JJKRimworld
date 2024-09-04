using Verse;

namespace JJK
{
    public static class SummonUtility
    {
        public static Pawn GetMaster(this Pawn pawn)
        {
            Hediff_Shikigami shikigami = (Hediff_Shikigami)pawn.health.hediffSet.GetFirstHediffOfDef(JJKDefOf.JJK_Shikigami);
            if (shikigami != null)
            {
                return shikigami.Master;
            }

            return null;
        }

        public static void MakeDraftable(this Pawn pawn)
        {
            SummonDraftManager.RegisterDraftableCreature(pawn);
        }

        public static void RemoveDraftable(this Pawn pawn)
        {
            SummonDraftManager.UnregisterDraftableCreature(pawn);
        }
    }
}

    