using RimWorld;
using Verse;
using Verse.AI;

namespace JJK
{
    public class ThinkNode_ConditionalSelfOrMasterHasTarget : ThinkNode_Conditional
    {
        protected override bool Satisfied(Pawn pawn)
        {
            if (pawn != null && pawn.IsShikigami() && pawn.GetMaster() != null)
            {
                Pawn master = pawn.GetMaster();

                if (pawn.mindState.enemyTarget != null || master.mindState.enemyTarget != null)
                {
                    //Log.Message($"{pawn.Label} or its master {master.Label} have an enemy target.");
                    return true;
                }

                if (PawnUtility.EnemiesAreNearby(pawn, 10, true) || PawnUtility.EnemiesAreNearby(master, 10, true))
                {
                    //Log.Message($"Enemies are near {pawn.Label}.");
                    return true;
                }
            }
            return false;
        }
    }
}

    