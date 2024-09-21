using Verse;

namespace JJK
{

    public class CompProperties_DraftableSummon : CompProperties
    {
        public CompProperties_DraftableSummon()
        {
            this.compClass = typeof(CompDraftableSummon);
        }
    }

    public class CompDraftableSummon : ThingComp
    {
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            DraftingUtility.RegisterDraftableCreature(parent as Pawn);
        }

        public override void PostDeSpawn(Map map)
        {
            DraftingUtility.UnregisterDraftableCreature(parent as Pawn);
            base.PostDeSpawn(map);
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            DraftingUtility.UnregisterDraftableCreature(parent as Pawn);
            base.PostDestroy(mode, previousMap);
        }
    }
}
    

