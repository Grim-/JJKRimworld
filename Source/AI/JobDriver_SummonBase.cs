using Verse;
using Verse.AI;

namespace JJK
{
    public abstract class JobDriver_SummonBase : JobDriver
    {
        public Pawn Summoner => TargetPawnA;
        public Pawn Self => pawn;
    }
}

    