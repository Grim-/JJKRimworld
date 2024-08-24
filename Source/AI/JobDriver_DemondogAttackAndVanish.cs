using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace JJK
{
    public class JobDriver_DemondogAttackAndVanish : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true; // No need to reserve anything
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            // Move to target
            Toil gotoToil = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            gotoToil.AddPreTickAction(() =>
            {
                if (ShouldDespawn())
                {
                    DespawnDemondog("Despawn condition met during movement");
                }
            });
            yield return gotoToil;


            Toil AttackToil = new Toil
            {
                initAction = () =>
                {
                    if (ShouldDespawn())
                    {
                        DespawnDemondog("Despawn condition met before attack");
                        return;
                    }

                    Pawn target = TargetA.Pawn;
                    if (target != null && !target.Dead && !target.Downed)
                    {
                        DamageInfo dinfo = new DamageInfo(DamageDefOf.Bite, 10f, 0f, -1f, pawn, null, null, DamageInfo.SourceCategory.ThingOrUnknown);
                        target.TakeDamage(dinfo);
                        if (Rand.Chance(0.5f))
                        {
                            Hediff hediff = HediffMaker.MakeHediff(HediffDefOf.BloodLoss, target, null);
                            hediff.Severity = 0.2f;
                            target.health.AddHediff(hediff, null, null);
                        }

                        MoteMaker.ThrowText(pawn.DrawPos, pawn.Map, "Demonic bite!", Color.red, 3.65f);
                    }

                    DespawnDemondog("Attack completed");
                }
            };

            AttackToil.AddPreTickAction(() =>
            {
                Pawn target = TargetA.Pawn;

                if (ShouldDespawn())
                {
                    DespawnDemondog("Demon dog attack target null or dead or downed.");
                    return;
                }

            });

            // Attack
            yield return AttackToil;
        }

        private bool ShouldDespawn()
        {
            return SummonerIncapacitated() ||
                   TargetA.Thing == null ||
                   !TargetA.Thing.HostileTo(pawn) ||
                   TargetA.Pawn.DeadOrDowned ||
                   TargetB.Pawn.DeadOrDowned ||
                   !pawn.CanReach(TargetA, PathEndMode.Touch, Danger.Deadly);
        }

        private bool SummonerIncapacitated()
        {
            Pawn summoner = TargetB.Pawn;
            return summoner == null || summoner.Dead || summoner.Downed;
        }

        private void DespawnDemondog(string reason)
        {
            Log.Message($"Demondog despawning: {reason}"); // Uncommented for debugging
            if (pawn.Map != null) // Check if pawn's map is not null
            {
                MoteMaker.MakeStaticMote(pawn.Position, pawn.MapHeld, ThingDefOf.Mote_Leaf, 2);
            }
            pawn.Destroy();
            EndJobWith(JobCondition.Succeeded);
        }

    }

    public class JobGiver_DemondogAttackTarget : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            if (pawn.jobs.curJob?.def != JJKDefOf.JJK_DemondogAttackAndVanish)
            {
                DespawnDemondog(pawn, "Invalid job");
                return null;
            }

            LocalTargetInfo target = pawn.CurJob.GetTarget(TargetIndex.A);
            if (!target.IsValid || !(target.Thing is Pawn) || !target.Thing.HostileTo(pawn) || !pawn.CanReach(target, PathEndMode.Touch, Danger.Deadly))
            {
                DespawnDemondog(pawn, "Target invalid or unreachable");
                return null;
            }

            return pawn.jobs.curJob;
        }

        private void DespawnDemondog(Pawn pawn, string reason)
        {
            if (!pawn.Destroyed)
            {
                // Log.Message($"Demondog despawning: {reason}"); // Uncomment for debugging
                pawn.Destroy();
            }
     
        }
    }
}

    