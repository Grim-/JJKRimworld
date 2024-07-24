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
            SummonDraftManager.RegisterDraftableCreature(parent as Pawn);
        }

        public override void PostDeSpawn(Map map)
        {
            SummonDraftManager.UnregisterDraftableCreature(parent as Pawn);
            base.PostDeSpawn(map);
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            SummonDraftManager.UnregisterDraftableCreature(parent as Pawn);
            base.PostDestroy(mode, previousMap);
        }
    }
}
    

