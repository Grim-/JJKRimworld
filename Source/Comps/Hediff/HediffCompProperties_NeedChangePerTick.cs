using RimWorld;
using Verse;

namespace JJK
{
    public class HediffCompProperties_NeedChangePerTick : HediffCompProperties
    {
        public NeedDef need;
        public float changeAmount = 10f;
        public int tickInterval = 2500;

        public HediffCompProperties_NeedChangePerTick()
        {
            compClass = typeof(HediffComp_NeedChangePerTick);
        }
    }

    public class HediffComp_NeedChangePerTick : HediffComp
    {
        private int ticks = 0;
        public HediffCompProperties_NeedChangePerTick Props => (HediffCompProperties_NeedChangePerTick)props;

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            if (Pawn.Spawned && this.Props.need != null && this.Pawn.needs != null)
            {
                ticks++;
                if (ticks >= Props.tickInterval)
                {
                    if (this.Pawn.needs.TryGetNeed(Props.need) is Need need)
                    {
                        need.CurLevel += Props.changeAmount;
                    }
                    ticks = 0;
                }
            }

        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look(ref ticks, "ticks");
        }
    }
}


