using RimWorld;
using Verse;
using Verse.AI;

namespace JJK
{
    public class ThinkNode_ConditionalSelfOrMasterHasTarget : ThinkNode_Conditional
    {
        protected override bool Satisfied(Pawn pawn)
        {
            if (pawn.IsSummon() && pawn.GetMaster() != null)
            {
                Pawn master = pawn.GetMaster();

                // Check if pawn or master has an enemy target
                if (pawn.mindState.enemyTarget != null || master.mindState.enemyTarget != null)
                {
                    //Log.Message($"{pawn.Label} or its master {master.Label} have an enemy target.");
                    return true;
                }

                //// Check if pawn is being attacked
                //if (pawn.mindState.lastAttackTargetTick > Find.TickManager.TicksGame - 400)
                //{
                //    Log.Message($"{pawn.Label} was recently attacked.");
                //    return true;
                //}

                //// Check if master is being attacked
                //if (master.mindState.lastAttackTargetTick > Find.TickManager.TicksGame - 400)
                //{
                //    Log.Message($"{pawn.Label}'s master {master.Label} was recently attacked.");
                //    return true;
                //}

                //// Check if pawn is in combat
                //if (pawn.mindState.lastEngageTargetTick > Find.TickManager.TicksGame - 400)
                //{
                //    Log.Message($"{pawn.Label} was recently in combat.");
                //    return true;
                //}

                //// Check if master is in combat
                //if (master.mindState.lastEngageTargetTick > Find.TickManager.TicksGame - 400)
                //{
                //    Log.Message($"{pawn.Label}'s master {master.Label} was recently in combat.");
                //    return true;
                //}

                // Check if there are any hostile pawns nearby
                if (PawnUtility.EnemiesAreNearby(pawn, 10, true))
                {
                    //Log.Message($"Enemies are near {pawn.Label}.");
                    return true;
                }

                // Check if there are any hostile pawns near the master
                if (PawnUtility.EnemiesAreNearby(master, 10, true))
                {
                   // Log.Message($"Enemies are near {pawn.Label}'s master {master.Label}.");
                    return true;
                }
            }
            return false;
        }
    }
}

    