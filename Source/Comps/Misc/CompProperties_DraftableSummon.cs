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

            if (parent is Pawn parentPawn)
            {
                DraftingUtility.RegisterDraftableCreature(parentPawn);
            }


        }

        public override void PostDeSpawn(Map map)
        {
            if (parent is Pawn parentPawn)
            {
                DraftingUtility.UnregisterDraftableCreature(parentPawn);
            }

   
            base.PostDeSpawn(map);
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            if (parent is Pawn parentPawn)
            {
                DraftingUtility.UnregisterDraftableCreature(parentPawn);
            }

    
            base.PostDestroy(mode, previousMap);
        }
    }
}
    

