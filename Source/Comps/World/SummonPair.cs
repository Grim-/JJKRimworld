using Verse;

namespace JJK
{
    public class SummonPair : IExposable
    {
        public Pawn Summoned;
        public Pawn Master;

        public SummonPair() { }

        public SummonPair(Pawn summoned, Pawn master)
        {
            Summoned = summoned;
            Master = master;
        }

        public void ExposeData()
        {
            Scribe_References.Look(ref Summoned, "Summoned");
            Scribe_References.Look(ref Master, "Master");
        }
    }
}

    