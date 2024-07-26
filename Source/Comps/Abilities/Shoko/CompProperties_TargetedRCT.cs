using RimWorld;
using System.Collections.Generic;
using Verse.AI;
using Verse;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

namespace JJK
{
    public class Stance_Busy_RCT : Stance_Busy
    {
        public Stance_Busy_RCT(int ticks, LocalTargetInfo focusTarg, Verb verb)
            : base(ticks, focusTarg, verb) { }
    }

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
                Log.Message($"[JJK] Ability toggled off for {parent.pawn}");
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

                Log.Message($"[JJK] Job started: {channelJob}");
            }
        }

        public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
        {
            if (IsCurrentlyCasting) return true; // Always allow toggling off

            bool baseCheck = base.CanApplyOn(target, dest);
            bool isPawn = target.Thing is Pawn;
            float distance = parent.pawn.Position.DistanceTo(target.Cell);
            bool inRange = distance <= Props.MaxRange;

            Log.Message($"[JJK] CanApplyOn: Base: {baseCheck}, IsPawn: {isPawn}, Distance: {distance}, InRange: {inRange}");

            return baseCheck && isPawn && inRange;
        }
    }
    public class JobDriver_ChannelRCT : JobDriver
    {
        private const TargetIndex TargetPawnIndex = TargetIndex.A;
        private CompAbilityEffect_TargetedRCT AbilityComp => job.ability?.CompOfType<CompAbilityEffect_TargetedRCT>();
        private Job targetOriginalJob;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            Log.Message($"[JJK] TryMakePreToilReservations called");
            return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            Log.Message($"[JJK] MakeNewToils started for {pawn} targeting {job.targetA.Thing}");

            this.FailOnDespawnedOrNull(TargetPawnIndex);
            this.FailOnAggroMentalState(TargetPawnIndex);

            yield return Toils_Goto.GotoThing(TargetPawnIndex, PathEndMode.Touch)
                .FailOnDespawnedNullOrForbidden(TargetPawnIndex);

            Log.Message($"[JJK] GotoThing toil added");

            Toil channelToil = new Toil();
            
            channelToil.initAction = () =>
            {
                Log.Message($"[JJK] channelToil initAction started");
                Pawn targetPawn = (Pawn)job.targetA.Thing;
                pawn.pather.StopDead();
                pawn.jobs.posture = PawnPosture.Standing;

                targetOriginalJob = targetPawn.CurJob;
                targetPawn.jobs.StopAll();
                targetPawn.pather.StopDead();
                targetPawn.jobs.StartJob(JobMaker.MakeJob(JobDefOf.Wait_MaintainPosture), JobCondition.InterruptForced);
                Log.Message($"[JJK] channelToil initAction completed");
            };


            int CurrentTick = 0;

            channelToil.tickAction = () =>
            {
                Pawn targetPawn = (Pawn)job.targetA.Thing;
                CurrentTick++;
                AbilityComp.AddRCTHediff(targetPawn);

                if (CurrentTick >= AbilityComp.Props.PartRegenTickCount * pawn.GetStatValue(JJKDefOf.JJK_RCTSpeedBonus))
                {
                    if (AbilityComp != null)
                    {
                        EffecterDefOf.ShamblerRaise.SpawnAttached(targetPawn, targetPawn.Map);
                        AbilityComp.HealTargetPawn(pawn.GetCursedEnergy(), targetPawn);
                        //Log.Message($"[JJK] Ability effects applied to {targetPawn}");
                    }
                    CurrentTick = 0;
                }

                // Log.Message($"[JJK] channelToil tickAction executed");
       
                if (ShouldEndJob())
                {
                    Log.Message($"[JJK] ShouldEndJob returned true, ending job");
                    EndJobWith(JobCondition.InterruptForced);
                    return;
                }

                pawn.pather.StopDead();
                targetPawn.pather.StopDead();
            };

            channelToil.AddFinishAction(() =>
            {
                Log.Message($"[JJK] channelToil finishAction executed");
                Pawn targetPawn = (Pawn)job.targetA.Thing;
                AbilityComp?.RemoveRCTHediff(targetPawn);
                if (AbilityComp != null) AbilityComp.IsCurrentlyCasting = false;

                if (targetOriginalJob != null && targetPawn.jobs != null)
                {
                    targetPawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
                    targetPawn.jobs.TryTakeOrderedJob(targetOriginalJob);
                    Log.Message($"[JJK] Restored original job for {targetPawn}");
                }
            });

            channelToil.defaultCompleteMode = ToilCompleteMode.Never;

            yield return channelToil;

            Log.Message($"[JJK] MakeNewToils completed");
        }

        private bool ShouldEndJob()
        {
            if (!(job.targetA.Thing is Pawn targetPawn))
            {
                Log.Message($"[JJK] ShouldEndJob: true. Reason: Target not pawn");
                return true;
            }

            Gene_CursedEnergy CursedEnergy = pawn.GetCursedEnergy();

            if (CursedEnergy == null)
            {
                Messages.Message($"No cursed energy gene.", MessageTypeDefOf.RejectInput);
                return true;
            }

            if (CursedEnergy.Value <= 0)
            {
                Messages.Message($"Not enough Cursed Energy", MessageTypeDefOf.RejectInput);
                return true;
            }

            bool needsHealing = targetPawn.health.HasHediffsNeedingTend() || HasMissingBodyParts(targetPawn);

            if (!needsHealing)
            {
                Messages.Message($"Target does not need healing", MessageTypeDefOf.RejectInput);
                return true;
            }

            if (pawn.Downed)
            {
                Messages.Message($"Cast is downed.", MessageTypeDefOf.RejectInput);
                return true;
            }

            if (!pawn.CanReach(job.targetA, PathEndMode.Touch, Danger.Deadly))
            {
                Messages.Message($"Cannot react target", MessageTypeDefOf.RejectInput);
                return true;
            }

            if (targetPawn == null)
            {
                Messages.Message($"Target is null", MessageTypeDefOf.RejectInput);
                return true;
            }

            if (targetPawn.Dead)
            {
                Messages.Message($"Target is dead", MessageTypeDefOf.RejectInput);
                return true;
            }

            if (CursedEnergy.Value < AbilityComp.GetPartCost())
            {
                Messages.Message($"Target does not need healing", MessageTypeDefOf.RejectInput);
                return true;
            }

            return false;
        }

        private bool HasMissingBodyParts(Pawn pawn)
        {
            return pawn.health.hediffSet.GetMissingPartsCommonAncestors().Any();
        }
    }

}