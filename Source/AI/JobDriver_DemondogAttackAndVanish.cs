using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace JJK
{
    public class JobDriver_DemondogAttackAndVanish : JobDriver
    {
        private Pawn Summoner => TargetPawnA;
        private Pawn AttackTarget => TargetPawnB;

        public DamageDef AttackDamageDef = DamageDefOf.Bite;
        public float DamageAmount = 20f;
        public float ArmourPen = 1f;
        public float ChanceToBleed = 0.5f;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true; // No need to reserve anything
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            // Move to target
            Toil gotoToil = Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.Touch);
            gotoToil.AddPreTickAction(() =>
            {
                if (ShouldDespawn())
                {
                    //DespawnDemondog();
                    EndJobWith(JobCondition.Incompletable);
                    return;
                }
            });

            //gotoToil.AddFinishAction(() =>
            //{
            //    DespawnDemondog();
            //});

            yield return gotoToil;


            Toil AttackToil = new Toil();

            AttackToil.AddPreTickAction(() =>
            {
                pawn.MentalState.RecoverFromState();

                if (ShouldDespawn())
                {
                    //DespawnDemondog();
                    EndJobWith(JobCondition.Incompletable);
                    return;
                }

            });

            AttackToil.initAction = () =>
            {
                if (ShouldDespawn())
                {             
                    EndJobWith(JobCondition.Incompletable);
                    return;
                }

                if (AttackTarget != null && !AttackTarget.Dead && !AttackTarget.Downed)
                {
                    //Verb ChosenVerb = pawn.meleeVerbs.TryGetMeleeVerb(AttackTarget);

                    //if (ChosenVerb != null)
                    //{

                    //}

                    DamageInfo dinfo = new DamageInfo(AttackDamageDef, DamageAmount, ArmourPen, -1f, pawn, null, null, DamageInfo.SourceCategory.ThingOrUnknown);
                    AttackTarget.TakeDamage(dinfo);
                    if (Rand.Chance(ChanceToBleed))
                    {
                        Hediff hediff = HediffMaker.MakeHediff(HediffDefOf.BloodLoss, AttackTarget, null);
                        hediff.Severity = 0.2f;
                        AttackTarget.health.AddHediff(hediff, null, null);
                    }

                    MoteMaker.ThrowText(pawn.DrawPos, pawn.Map, "Demonic bite!", Color.red, 3.65f);
                }

                //DespawnDemondog();
                EndJobWith(JobCondition.Succeeded);
            };


            AttackToil.AddFinishAction(() =>
            {
                //DespawnDemondog();
            });

            // Attack
            yield return AttackToil;
        }

        private bool ShouldDespawn()
        {
            if (SummonerIncapacitated())
            {
                Messages.Message($"Demondog despawning: summoner incapacitated.", MessageTypeDefOf.NegativeEvent, false);
                return true;
            }

            if (AttackTarget == null || AttackTarget.Destroyed || AttackTarget.DeadOrDowned)
            {
                Messages.Message($"Demondog despawning: target thing null or destroyed", MessageTypeDefOf.NegativeEvent, false);
                return true;
            }

            if (!AttackTarget.HostileTo(pawn))
            {
                Messages.Message($"Demondog despawning: target not hostile", MessageTypeDefOf.NegativeEvent, false);
                return true;
            }

            if (!pawn.CanReach(AttackTarget, PathEndMode.Touch, Danger.Deadly))
            {
                Messages.Message($"Demondog despawning: cannot reach target", MessageTypeDefOf.NegativeEvent, false);
                return true;
            }

            return false;
        }

        private bool SummonerIncapacitated()
        {
            return Summoner == null || Summoner.Dead || Summoner.Downed;
        }

        private void DespawnDemondog()
        {
            if (!pawn.Destroyed)
            {
                FleckMaker.ThrowSmoke(pawn.Position.ToVector3(), Map, 1.5f);
                FleckMaker.Static(pawn.Position, Map, JJKDefOf.JJK_BlackSmoke, 1.5f);
                pawn.Destroy();
            } 
        }

    }
}

    