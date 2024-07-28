using RimWorld;
using System;
using Verse;
using Verse.AI;

namespace JJK
{
    public class JobGiver_SummonedCreatureFightEnemy : JobGiver_AIDefendMaster
    {
        protected Pawn Master = null;
        protected override Pawn GetDefendee(Pawn pawn)
        {
            Log.Message($"GetDefendee called for {pawn.LabelShort}");

            if (Master != null)
            {
                Log.Message($"Returning cached Master: {Master.LabelShort}");
                return Master;
            }

            Master = Find.World.GetComponent<SummonedCreatureManager>().GetMasterFor(pawn);
            Log.Message($"Master from SummonedCreatureManager: {Master?.LabelShort ?? "null"}");

            if (Master == null || !Master.Spawned)
            {
                Master = Find.World.GetComponent<AbsorbedCreatureManager>().GetSummonerFor(pawn);
                Log.Message($"Master from AbsorbedCreatureManager: {Master?.LabelShort ?? "null"}");
            }

            return Master;
        }

        protected override Job TryGiveJob(Pawn pawn)
        {
            Job job = base.TryGiveJob(pawn);
            job.reportStringOverride = "Defending Summoner";
            return job;
        }

        protected override float GetFlagRadius(Pawn pawn)
        {
            return 15;
        }
    }


    public class ThinkNode_ConditionalDoAlways : ThinkNode_Conditional
    {
        protected override bool Satisfied(Pawn pawn)
        {
            return true;
        }
    }
}

    