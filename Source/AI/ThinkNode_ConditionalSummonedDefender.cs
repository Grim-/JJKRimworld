using Verse;
using Verse.AI;

namespace JJK
{
    public class ThinkNode_ConditionalSummonedDefender : ThinkNode_Conditional
    {
        protected override bool Satisfied(Pawn pawn)
        {
            return pawn.GetMaster() != null;
        }
    }
}

    