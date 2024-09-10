using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace JJK
{
    public class JobGiver_SummonedCreatureFollowMaster : JobGiver_AIFollowMaster
    {
        protected Pawn Master = null;

        protected override Pawn GetFollowee(Pawn pawn)
        {
            if (pawn.IsShikigami())
            {
                //Log.Error($"pawn is summon master is {pawn.GetMaster()}");
                return pawn.GetMaster();
            }
            return null;
        }

        protected override float GetRadius(Pawn pawn)
        {
            return 3f;
        }

        protected override Job TryGiveJob(Pawn pawn)
        {
           // Log.Message($"TryGiveJob called for {pawn.LabelShort}");

            Pawn followee = GetFollowee(pawn);
            if (followee == null)
            {
                Log.Error($"Followee is null for {pawn.LabelShort}");
                return null;
            }

            if (!followee.Spawned)
            {
                Log.Message($"Followee {followee.LabelShort} is not spawned");
                return null;
            }

            if (!pawn.CanReach(followee, PathEndMode.OnCell, Danger.Deadly, false, false, TraverseMode.ByPawn))
            {
                Log.Message($"{pawn.LabelShort} cannot reach {followee.LabelShort}");
                return null;
            }

            //Log.Message($"creating follow close job.");
            Job job = JobMaker.MakeJob(JobDefOf.FollowClose, followee);
            job.expiryInterval = 200;
            job.followRadius = GetRadius(pawn);
            //pawn.mindState.canFleeIndividual = false;
            job.SetTarget(TargetIndex.A, GetFollowee(pawn));


            job.reportStringOverride = "Following Summoner";
            //Log.Message($"Created follow job for {pawn.LabelShort} to follow {followee.LabelShort}");
            return job;
        }
    }

    public class JobDriver_FormationFollow : JobDriver_FollowClose
    {

        private Vector3 CalculateFormationPosition(Vector3 leaderPos)
        {
            // Implement your formation logic here
            // This is a simple example that places pawns in a line behind the leader
            int index = pawn.thingIDNumber % 5; // Assuming 5 pawns per row
            return leaderPos + new Vector3(index - 2, 0, -index / 5);
        }
    }
}

    