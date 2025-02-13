using Verse;

namespace JJK
{
    public class HediffCompProperties_SeverityPerInterval : HediffCompProperties
    {
        public float severityChange = 0.01f;
        public int tickInterval = 2500;

        public HediffCompProperties_SeverityPerInterval()
        {
            compClass = typeof(HediffComp_SeverityPerInterval);
        }
    }

    public class HediffComp_SeverityPerInterval : HediffComp
    {
        private int ticksUntilReduction;

        public HediffCompProperties_SeverityPerInterval Props => (HediffCompProperties_SeverityPerInterval)props;

        public override void CompPostMake()
        {
            base.CompPostMake();
            ResetInterval();
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);

            ticksUntilReduction--;
            if (ticksUntilReduction <= 0)
            {
                severityAdjustment += Props.severityChange;
                ResetInterval();
            }
        }

        private void ResetInterval()
        {
            ticksUntilReduction = Props.tickInterval;
        }

        public override void CompExposeData()
        {
            base.CompExposeData();


            Scribe_Values.Look(ref ticksUntilReduction, "ticksUntilReduction");
        }
    }
}


