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

        protected override float GetFlagRadius(Pawn pawn)
        {
            return 10f;
        }

        //protected override IntVec3 GetFlagPosition(Pawn pawn)
        //{
        //    return pawn.GetMaster().Position;
        //}
    }
}

    