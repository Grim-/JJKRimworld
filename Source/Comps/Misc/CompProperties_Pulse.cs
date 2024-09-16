using Verse;

namespace JJK
{
    public abstract class CompProperties_Pulse : CompProperties
    {
        public int TicksBetweenPulse = 80;
    }

    public abstract class BasePulseComp : ThingComp
    {
        public new CompProperties_Pulse Props => (CompProperties_Pulse)props;
        protected int TicksUntilPulse = 0;

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            ResetTimer();
        }

        public override void CompTick()
        {
            base.CompTick();

            TicksUntilPulse++;

            if (TicksUntilPulse >= Props.TicksBetweenPulse)
            {
                OnPulse();
                ResetTimer();
            }
        }

        protected virtual void ResetTimer()
        {
            TicksUntilPulse = 0;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref TicksUntilPulse, "TicksUntilExplosion");
        }

        public abstract void OnPulse();
    }
}