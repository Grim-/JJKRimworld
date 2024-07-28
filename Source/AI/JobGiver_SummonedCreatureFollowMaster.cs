using RimWorld;
using Verse;
using Verse.AI;

namespace JJK
{
    public class JobGiver_SummonedCreatureFollowMaster : JobGiver_AIFollowMaster
    {
        protected Pawn Master = null;

        protected override Pawn GetFollowee(Pawn pawn)
        {
           // Log.Message($"GetFollowee called for {pawn.LabelShort}");

            if (Master != null)
            {
               // Log.Message($"Returning cached Master: {Master.LabelShort}");
                return Master;
            }

            Master = Find.World.GetComponent<SummonedCreatureManager>().GetMasterFor(pawn);
            Log.Message($"Master from SummonedCreatureManager: {Master?.LabelShort ?? "null"}");

            if (Master == null || !Master.Spawned)
            {
                Master = Find.World.GetComponent<AbsorbedCreatureManager>().GetSummonerFor(pawn);
               // Log.Message($"Master from AbsorbedCreatureManager: {Master?.LabelShort ?? "null"}");
            }

            return Master;
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
                //Log.Error($"Followee is null for {pawn.LabelShort}");
                return null;
            }

            if (!followee.Spawned)
            {
                //Log.Message($"Followee {followee.LabelShort} is not spawned");
                return null;
            }

            if (!pawn.CanReach(followee, PathEndMode.OnCell, Danger.Deadly, false, false, TraverseMode.ByPawn))
            {
                //Log.Message($"{pawn.LabelShort} cannot reach {followee.LabelShort}");
                return null;
            }

            float radius = GetRadius(pawn);
            if (!JobDriver_FollowClose.FarEnoughAndPossibleToStartJob(pawn, followee, radius))
            {
               // Log.Message($"{pawn.LabelShort} is not far enough from {followee.LabelShort} to start follow job");
                return null;
            }

            Job job = JobMaker.MakeJob(JobDefOf.FollowClose, followee);
            job.expiryInterval = 200;
            job.checkOverrideOnExpire = true;
            job.followRadius = radius;

            pawn.mindState.canFleeIndividual = false;

            job.reportStringOverride = "Following Summoner";
            Log.Message($"Created follow job for {pawn.LabelShort} to follow {followee.LabelShort}");
            return job;
        }
    }
}

    