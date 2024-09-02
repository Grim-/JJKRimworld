using RimWorld;
using System;
using Verse;
using Verse.AI;

namespace JJK
{
    public class JobGiver_SummonedCreatureFightEnemy : JobGiver_AIDefendPawn
    {
        protected Pawn Master = null;

        protected override Job TryGiveJob(Pawn pawn)
        {
            this.chaseTarget = true;
            this.allowTurrets = true;
            this.ignoreNonCombatants = true;
            this.humanlikesOnly = false;
            Job job = base.TryGiveJob(pawn);
            job.reportStringOverride = "Defending Summoner";
            pawn.mindState.canFleeIndividual = false;
            return job;
        }

        protected override Pawn GetDefendee(Pawn pawn)
        {
            if (pawn.IsSummon())
            {
                return pawn.GetMaster();
            }
            return null;
        }

        //protected override Thing FindAttackTarget(Pawn pawn)
        //{

        //    if (GenAI.EnemyIsNear(pawn.GetMaster(), 5f, out Thing threat))
        //    {
        //        return threat;
        //    }

        //    return base.FindAttackTarget(pawn);
        //}

        protected override float GetFlagRadius(Pawn pawn)
        {
            return 50f;
        }
    }
}

    