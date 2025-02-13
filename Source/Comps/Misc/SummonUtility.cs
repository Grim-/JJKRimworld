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


        public static Pawn GetTenShadowsMaster(this Pawn pawn)
        {
            if (pawn.TryGetComp(out Comp_TenShadowsSummon shadowsSummon))
            {
                return shadowsSummon.Master;
            }

            return null;
        }
    }
}

    