using UnityEngine;
using Verse;

namespace JJK
{
    public class CompProperties_CursedObjectPiece : CompProperties
    {
        public int TransformThreshold = 5;
        public FloatRange MinTimeBetweenConsumption = new FloatRange(6000, 24000);

        public float GetMinTimeBetweenConsumption() => Random.Range(MinTimeBetweenConsumption.min, MinTimeBetweenConsumption.max);



        public CompProperties_CursedObjectPiece()
        {
            compClass = typeof(CompCursedObjectPiece);
        }
    }

    public class CompCursedObjectPiece : ThingComp
    {
        public Pawn originPawn;

        public new CompProperties_CursedObjectPiece Props => (CompProperties_CursedObjectPiece)props;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_References.Look(ref originPawn, "originPawn");
        }

        public override void PrePostIngested(Pawn ingester)
        {
            base.PrePostIngested(ingester);

            Hediff_CursedObjectConsumer cursedObjectConsumer = (Hediff_CursedObjectConsumer)ingester.health.GetOrAddHediff(JJKDefOf.JJK_CursedObjectConsumer);


            if (cursedObjectConsumer != null)
            {
                Log.Message($"Ate a cursed object piece .. {this}");
                cursedObjectConsumer.ProcessConsumedPiece(this);
            }
        }
    }
}
