using Verse;

namespace JJK
{
    public static class SummonUtility
    {
        public static bool IsSummoned(this Pawn pawn)
        {
            SummonedCreatureManager summonManager = Find.World.GetComponent<SummonedCreatureManager>();
            return summonManager.GetMaster(pawn) != null;
        }

        public static Pawn GetMaster(this Pawn pawn)
        {
            SummonedCreatureManager summonManager = Find.World.GetComponent<SummonedCreatureManager>();
            return summonManager.GetMaster(pawn);
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

    