using RimWorld;
using System;
using Verse;
using Verse.AI;

namespace JJK
{
    public class CompProperties_AbilityDemondogSummon : CompProperties_BaseShikigami
    {
        public PawnKindDef demondogKindDef;

        public CompProperties_AbilityDemondogSummon()
        {
            compClass = typeof(CompAbilityEffect_DemondogSummon);
        }
    }

    public class CompAbilityEffect_DemondogSummon : CompBaseShikigamiSummon
    {
        public new CompProperties_AbilityDemondogSummon Props => (CompProperties_AbilityDemondogSummon)props;

        private enum DivineDogSummonMode
        {
            Individual,
            Totality,
            Cooldown
        }

        private DivineDogSummonMode CurrentState = DivineDogSummonMode.Individual;
        private Pawn DemonDogTotality;
        private Pawn DemonDogWhite;
        private Pawn DemonDogBlack;

        private int TicksUntilAvailableAgain = 1250;
        private int ResummonTimer = 0;

        public override void OnPawnTarget(Pawn Pawn, Map Map)
        {
            Summon(parent.pawn.Position, Pawn);
        }

        public override void OnLocationTarget(IntVec3 Position, Map Map)
        {
            Summon(Position, null);
        }

        public override void CompTick()
        {
            base.CompTick();

            if (CurrentState == DivineDogSummonMode.Cooldown)
            {
                ResummonTimer++;

                if (ResummonTimer >= TicksUntilAvailableAgain)
                {
                    OnCooldownStateEnd();
                }
            }
        }

        public override void Summon(IntVec3 Position, Pawn TargetPawn)
        {
            if (CurrentState == DivineDogSummonMode.Cooldown)
            {
                return;
            }

            if (CurrentState == DivineDogSummonMode.Totality)
            {
                DemonDogTotality = SpawnDemonDog(JJKDefOf.JJK_DivineDogTotality, Position, TargetPawn, parent.pawn.Map);
            }
            else
            {
                DemonDogBlack = SpawnDemonDog(JJKDefOf.JJK_DivineDogBlack, Position + new IntVec3(-1, 0, 0), TargetPawn, parent.pawn.Map);
                DemonDogWhite = SpawnDemonDog(JJKDefOf.JJK_DivineDogWhite, Position + new IntVec3(1, 0, 0), TargetPawn, parent.pawn.Map);
            }
        }

        public override bool HasActive()
        {
            return DemonDogWhite != null || DemonDogBlack != null || DemonDogTotality != null;
        }

        public override void DestroyActive()
        {
            DestroySummon(DemonDogBlack);
            DemonDogBlack = null;
            DestroySummon(DemonDogWhite);
            DemonDogWhite = null;
            DestroySummon(DemonDogTotality);
            DemonDogTotality = null;
        }

        public override void UnSummon()
        {
            DestroySummon(DemonDogBlack);
            DemonDogBlack = null;
            DestroySummon(DemonDogWhite);
            DemonDogWhite = null;
            DestroySummon(DemonDogTotality);
            DemonDogTotality = null;
        }

        private Pawn SpawnDemonDog(PawnKindDef KindDef, IntVec3 SpawnPosition, Pawn TargetPawn, Map Map)
        {
            Pawn DemonDog = JJKUtility.SpawnShikigami(KindDef, parent.pawn, Map, SpawnPosition);
            CreateSummonVFX(SpawnPosition, Map);

            if (DemonDog.TryGetComp(out Comp_OnDeathHandler CompOnDeath))
            {
                //Log.Message($"Registering OnDeath Handler for {DemonDog.Label} (shikigami)");
                CompOnDeath.OnDeath += OnShikigamiDied;
            }

            if (TargetPawn != null)
            {
                TryStartAttackJob(DemonDog, TargetPawn);
            }
            return DemonDog;
        }

        private void DestroySummon(Pawn Summon)
        {
            if (Summon != null && !Summon.Destroyed)
            {
                if (Summon.TryGetComp(out Comp_OnDeathHandler CompOnDeath))
                {
                    //Log.Message($"Unregistering OnDeath Handler for {Summon.Label} (shikigami)");
                    CompOnDeath.OnDeath -= OnShikigamiDied;
                }

                Summon.Destroy();
            }
        }

        private void TryStartAttackJob(Pawn Pawn, Pawn TargetPawn)
        {
            if (TargetPawn != null)
            {
                Job Job = JobMaker.MakeJob(JJKDefOf.JJK_DemondogAttackAndVanish);
                Job.SetTarget(TargetIndex.A, parent.pawn);
                Job.SetTarget(TargetIndex.B, TargetPawn);
                Pawn.jobs.StartJob(Job, JobCondition.None);
            }
        }

        private void OnShikigamiDied(Thing Obj)
        {
            //Log.Message($"OnShikigamiDied");
            if (Obj is Pawn Pawn)
            {
                if (Pawn.kindDef == JJKDefOf.JJK_DivineDogBlack || Pawn.kindDef == JJKDefOf.JJK_DivineDogWhite)
                {
                    Messages.Message($"{Pawn.Label} Has died!", MessageTypeDefOf.NegativeEvent, true);

                    if (HasActive())
                    {
                        DestroyActive();
                    }
                    CurrentState = DivineDogSummonMode.Totality;
                    Summon(Obj.Position, null);
                }
                else if (Pawn.kindDef == JJKDefOf.JJK_DivineDogTotality)
                {
                    Messages.Message($"{Pawn.Label} Has died! The ability is now on cooldown.", MessageTypeDefOf.NegativeEvent, true);
                    StartCooldownState();
                }
            }
        }


        private void StartCooldownState()
        {
            CurrentState = DivineDogSummonMode.Cooldown;
            ResummonTimer = 0;
            ShouldDisableGizmo = true;
        }

        private void OnCooldownStateEnd()
        {
            CurrentState = DivineDogSummonMode.Individual;
            ResummonTimer = 0;
            ShouldDisableGizmo = false;
        }


        public override string CompInspectStringExtra()
        {
            return base.CompInspectStringExtra() + $"Current Summon State {CurrentState}";
        }

        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Values.Look(ref CurrentState, "CurrentState", DivineDogSummonMode.Individual);
            Scribe_References.Look(ref DemonDogBlack, "DemonDogBlack");
            Scribe_References.Look(ref DemonDogWhite, "DemonDogWhite");
            Scribe_References.Look(ref DemonDogTotality, "DemonDogTotality");
            Scribe_Values.Look(ref ResummonTimer, "ResummonTimer", 0);
        }

    }
}