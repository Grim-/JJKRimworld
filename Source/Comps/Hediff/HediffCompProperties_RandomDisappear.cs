using Verse;

namespace JJK
{
    public class HediffCompProperties_RandomDisappear : HediffCompProperties
    {
        public int minDurationTicks = 180000;
        public int maxDurationTicks = 600000;

        public HediffCompProperties_RandomDisappear()
        {
            compClass = typeof(HediffComp_RandomDisappear);
        }
    }

    public class HediffComp_RandomDisappear : HediffComp
    {
        private int ticksToDisappear = -1;

        public HediffCompProperties_RandomDisappear Props => (HediffCompProperties_RandomDisappear)props;

        public override void CompPostMake()
        {
            base.CompPostMake();
            ticksToDisappear = Rand.Range(Props.minDurationTicks, Props.maxDurationTicks);
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);

            if (ticksToDisappear > 0)
            {
                ticksToDisappear--;
                if (ticksToDisappear <= 0)
                {
                    Pawn.health.RemoveHediff(parent);
                }
            }
        }
    }
}