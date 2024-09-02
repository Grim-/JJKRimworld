using RimWorld;
using Verse;
using Verse.AI;

namespace JJK
{
    public class ThinkNode_ConditionalSelfOrMasterHasTarget : ThinkNode_Conditional
    {
        protected override bool Satisfied(Pawn pawn)
        {
            if (pawn.IsSummon() && pawn.GetMaster() != null && pawn.GetMaster().mindState.enemyTarget != null || pawn.mindState.enemyTarget != null)
            {
                return true;
            }
            return false;
        }
    }
}

    