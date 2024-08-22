using System.Collections.Generic;
using Verse;

namespace JJK
{
    public class HediffCompProperties_GiveRandomGene : HediffCompProperties
    {
        public List<string> Genes;

        public HediffCompProperties_GiveRandomGene()
        {
            compClass = typeof(GiveRandomGene);
        }
    }

    public class GiveRandomGene : HediffComp
    {
        public new HediffCompProperties_GiveRandomGene Props => (HediffCompProperties_GiveRandomGene)props;

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);

            string ChosenGeneData = Props.Genes.RandomElement();
            GeneDef ChosenGene = DefDatabase<GeneDef>.GetNamed(ChosenGeneData);
            if (ChosenGene != null)
            {
                Pawn.genes.AddGene(ChosenGene, true);
            }
        }
    }
}

