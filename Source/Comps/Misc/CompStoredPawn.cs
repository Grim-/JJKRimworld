using System.Collections.Generic;
using Verse;

namespace JJK
{


    /// <summary>
    /// Allows you to store a reference to a pawn in the Thing, this is saved.
    /// </summary>
    public class CompStoredPawn : ThingComp
    {
        private Pawn StoredPawn;
        public Pawn Pawn => StoredPawn;

        //private DollTransformationWorldComponent _DollManager;
        //private DollTransformationWorldComponent DollManager
        //{
        //    get
        //    {
        //        if (_DollManager == null)
        //        {
        //            _DollManager = Find.World.GetComponent<DollTransformationWorldComponent>();
        //        }
        //        return _DollManager;
        //    }
        //}

        public void StorePawn(Pawn pawn)
        {
            StoredPawn = pawn;
        }

        public bool HasStoredPawn()
        {
            return StoredPawn != null;
        }

        public Pawn ReleasePawn()
        {
            var pawn = StoredPawn;
            StoredPawn = null;
            return pawn;
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            if (respawningAfterLoad)
            {
                StorePawn(StoredPawn);
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Deep.Look(ref StoredPawn, "storedPawn");
        }


        public override string CompInspectStringExtra()
        {
            if (StoredPawn == null)
            {
                return "No pawn stored";
            }

            TaggedString pawnInfo = "Stored pawn: " + StoredPawn.NameFullColored;
            return pawnInfo;
        }

        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selectionPawn)
        {
            foreach (FloatMenuOption option in base.CompFloatMenuOptions(selectionPawn))
            {
                yield return option;
            }

            Pawn storedPawn = StoredPawn;
            yield return new FloatMenuOption($"View stored pawn info", () =>
            {
                Find.WindowStack.Add(new Dialog_InfoCard(storedPawn));
            });
        }
    }

    public class CompProperties_StoredPawn : CompProperties
    {
        public CompProperties_StoredPawn()
        {
            compClass = typeof(CompStoredPawn);
        }
    }

}