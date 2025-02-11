using RimWorld;
using Verse;
using Verse.AI;

namespace JJK
{
    public class ThinkNode_ConditionalSelfOrMasterHasTarget : ThinkNode_Conditional
    {
        protected override bool Satisfied(Pawn pawn)
        {
            if (pawn == null || !pawn.IsShikigami())
            {
                return false;
            }

            if (!pawn.Spawned)
            {
                return false;
            }

            Pawn master = pawn.GetMaster();
            if (master == null)
            {
                return false;
            }

            if (pawn.mindState?.enemyTarget != null || master.mindState?.enemyTarget != null)
            {
                return true;
            }

            if (pawn.Spawned && master.Spawned)
            {
                if (PawnUtility.EnemiesAreNearby(pawn, 10, true) ||
                    PawnUtility.EnemiesAreNearby(master, 10, true))
                {
                    return true;
                }
            }

            return false;
        }
    }
}

    