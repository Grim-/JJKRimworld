using Verse;
using Verse.AI;

namespace JJK
{
    public class ThinkNode_ConditionalNearMaster : ThinkNode_Conditional
    {
        public float MaxDistanceToMaster = 5;

        protected override bool Satisfied(Pawn pawn)
        {
            if (pawn != null && pawn.IsShikigami() && pawn.GetMaster() != null)
            {
                Pawn master = pawn.GetMaster();
                return pawn.Position.DistanceTo(master.Position) <= MaxDistanceToMaster;
            }
            return false;
        }
    }
}

    