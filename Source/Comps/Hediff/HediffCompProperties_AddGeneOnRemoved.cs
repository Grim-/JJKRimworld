using Verse;

namespace JJK
{
    public class HediffCompProperties_AddGeneOnRemoved : HediffCompProperties
    {
        public string GeneDefName = "Gene_JJKCursedEnergy";

        public HediffCompProperties_AddGeneOnRemoved()
        {
            compClass = typeof(HediffComp_AddGeneOnRemoved);
        }
    }


    public class HediffComp_AddGeneOnRemoved : HediffComp
    {
        public new HediffCompProperties_AddGeneOnRemoved Props => (HediffCompProperties_AddGeneOnRemoved)props;

        public override void CompPostPostRemoved()
        {
            base.CompPostPostRemoved();

            if (Pawn.genes != null)
            {
                GeneDef cursedEnergyGene = DefDatabase<GeneDef>.GetNamed(Props.GeneDefName);

                if (cursedEnergyGene != null)
                {
                    Pawn.genes.AddGene(cursedEnergyGene, true);
                }
            }
        }
    }
}