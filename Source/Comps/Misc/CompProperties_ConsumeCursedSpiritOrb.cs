using RimWorld;
using System.Linq;
using Verse;

namespace JJK
{
    public class CompProperties_CursedSpiritOrb : CompProperties
    {
        public CompProperties_CursedSpiritOrb()
        {
            compClass = typeof(Comp_CursedSpiritOrb);
        }
    }


    public class Comp_CursedSpiritOrb : ThingComp
    {
        public Pawn containedSpirit;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (!respawningAfterLoad && containedSpirit == null)
            {
                GenerateRandomSpirit();
            }
        }

        public void StoreSpirit(Pawn spirit)
        {
            containedSpirit = spirit;
        }

        public Pawn ReleaseSpirit()
        {
            Pawn spirit = containedSpirit;
            containedSpirit = null;
            return spirit;
        }

        public override void PrePostIngested(Pawn ingester)
        {
            base.PrePostIngested(ingester);
            Hediff_CursedSpiritManipulator cursedSpiritManipulator = ingester.GetCursedSpiritManipulator();
            if (cursedSpiritManipulator != null && containedSpirit != null)
            {
                cursedSpiritManipulator.AbsorbCreature(containedSpirit);
                Messages.Message($"{ingester.LabelShort} has absorbed {containedSpirit.LabelShort} from the orb.", MessageTypeDefOf.PositiveEvent);
            }
        }

        public override string CompInspectStringExtra()
        {
            if (containedSpirit != null)
            {
                return "Contained Spirit: " + containedSpirit.LabelCap;
            }
            return "Empty";
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Deep.Look(ref containedSpirit, "containedSpirit");
        }

        private void GenerateRandomSpirit()
        {
            // Get a random PawnKindDef that has the CursedSpiritExtension
            PawnKindDef randomCursedSpiritKind = DefDatabase<PawnKindDef>.AllDefs
                .Where(def => def.GetModExtension<CursedSpiritExtension>() != null)
                .RandomElementWithFallback();

            if (randomCursedSpiritKind != null)
            {
                containedSpirit = PawnGenerator.GeneratePawn(randomCursedSpiritKind);
            }
        }
    }
}