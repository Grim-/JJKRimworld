using Verse.AI;
using Verse;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

namespace JJK
{

    public class CompProperties_TargetedRCT : CompProperties_RCTBase
    {
        public float MaxRange = 30f;
        public CompProperties_TargetedRCT()
        {
            compClass = typeof(CompAbilityEffect_TargetedRCT);
        }
    }

    public class CompAbilityEffect_TargetedRCT : CompAbilityEffect_RCTBase
    {
        public new CompProperties_TargetedRCT Props => (CompProperties_TargetedRCT)props;
        private Pawn TargetPawn;
        public new bool IsCurrentlyCasting = false;

        public override void ApplyAbility(LocalTargetInfo target, LocalTargetInfo dest)
        {
            if (IsCurrentlyCasting)
            {
                // Toggle off
                IsCurrentlyCasting = false;
                parent.pawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
                //Log.Message($"[JJK] Ability toggled off for {parent.pawn}");
            }
            else
            {
                // Toggle on
                if (!(target.Thing is Pawn targetPawn)) return;

                IsCurrentlyCasting = true;
                TargetPawn = targetPawn;
                Log.Message($"[JJK] ApplyAbility called for {parent.pawn} targeting {targetPawn}");

                Job channelJob = JobMaker.MakeJob(JJKDefOf.ChannelRCT, targetPawn);
                channelJob.ability = parent;
                parent.pawn.jobs.StartJob(channelJob, JobCondition.InterruptForced);

               // Log.Message($"[JJK] Job started: {channelJob}");
            }
        }

        public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
        {
            if (IsCurrentlyCasting) return true; // Always allow toggling off

            bool baseCheck = base.CanApplyOn(target, dest);
            bool isPawn = target.Thing is Pawn;
            float distance = parent.pawn.Position.DistanceTo(target.Cell);
            bool inRange = distance <= Props.MaxRange;

            //Log.Message($"[JJK] CanApplyOn: Base: {baseCheck}, IsPawn: {isPawn}, Distance: {distance}, InRange: {inRange}");

            return baseCheck && isPawn && inRange;
        }
    }

}