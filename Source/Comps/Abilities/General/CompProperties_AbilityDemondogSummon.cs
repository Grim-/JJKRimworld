using RimWorld;
using System;
using Verse;
using Verse.AI;

namespace JJK
{
    public class CompProperties_AbilityDemondogSummon : CompProperties_AbilityEffect
    {
        public PawnKindDef demondogKindDef;

        public CompProperties_AbilityDemondogSummon()
        {
            compClass = typeof(CompAbilityEffect_DemondogSummon);
        }
    }


    public class CompAbilityEffect_DemondogSummon : CompAbilityEffect
    {
        public new CompProperties_AbilityDemondogSummon Props => (CompProperties_AbilityDemondogSummon)props;
        private bool ShouldSummonTotality = false;
        private bool CanSummon = true;

        private Pawn DemonDogTotality;
        private Pawn DemonDogWhite;
        private Pawn DemonDogBlack;



        private int TicksUntilAvailableAgain = 1250;
        private int ResummonTimer = 0;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);

            if (HasActive())
            {
                DestroyActive();
            }
            else
            {
                Map map = parent.pawn.Map;
                IntVec3 spawnPosition = parent.pawn.Position;

                if (spawnPosition.Walkable(map))
                {
                    Summon(spawnPosition, target.Pawn);
                }
            }
        }

        public override void CompTick()
        {
            base.CompTick();

            if (!CanSummon)
            {
                ResummonTimer++;

                if (ResummonTimer >= TicksUntilAvailableAgain)
                {
                    CanSummon = true;
                    ResummonTimer = 0;
                }
            }
            else
            {

            }
        }


        private void UpdateSummonStatus()
        {
            if (DemonDogBlack != null)
            {
                if (DemonDogBlack.Dead)
                {

                }
            }
        }


        private void Summon(IntVec3 Position, Pawn TargetPawn)
        {
            if (!CanSummon)
            {
                return;
            }

            if (ShouldSummonTotality)
            {
                DemonDogTotality = SpawnDemonDog(JJKDefOf.JJK_DivineDogTotality, Position, TargetPawn, parent.pawn.Map);
            }
            else
            {
                DemonDogBlack = SpawnDemonDog(JJKDefOf.JJK_DivineDogBlack, Position + new IntVec3(-1, 0, 0), TargetPawn, parent.pawn.Map);
                DemonDogWhite = SpawnDemonDog(JJKDefOf.JJK_DivineDogWhite, Position + new IntVec3(1, 0, 0), TargetPawn, parent.pawn.Map);
            }
        }


        private bool HasActive()
        {
            return DemonDogWhite != null || DemonDogBlack != null;
        }

        private void DestroyActive()
        {
            if (DemonDogBlack != null && !DemonDogBlack.Destroyed)
            {
                if (DemonDogBlack.TryGetComp(out CompOnDeathHandler compOnDeath))
                {
                    Log.Message($"unRegistering OnDeath Handler for {DemonDogBlack.Label} (shikigami)");
                    compOnDeath.OnDeath -= OnShikigamiDeath;
                }

                DemonDogBlack.Destroy();
            }

            if (DemonDogWhite != null && !DemonDogWhite.Destroyed)
            {
                if (DemonDogWhite.TryGetComp(out CompOnDeathHandler compOnDeath))
                {
                    Log.Message($"unRegistering OnDeath Handler for {DemonDogWhite.Label} (shikigami)");
                    compOnDeath.OnDeath -= OnShikigamiDeath;
                }

                DemonDogWhite.Destroy();
            }
        }

        public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
        {
            return base.CanApplyOn(target, dest);
        }

        private Pawn SpawnDemonDog(PawnKindDef KindDef, IntVec3 spawnPosition, Pawn TargetPawn, Map Map)
        {
            Pawn demondog = JJKUtility.SpawnShikigami(KindDef, parent.pawn, Map, spawnPosition);

            if (demondog.TryGetComp(out CompOnDeathHandler compOnDeath))
            {
                Log.Message($"Registering OnDeath Handler for {demondog.Label} (shikigami)");
                compOnDeath.OnDeath += OnShikigamiDeath;
            }

            if (TargetPawn != null)
            {
                TryStartAttackJob(demondog, TargetPawn);
            }
            return demondog;
        }

        private void OnShikigamiDeath(Thing obj)
        {
            if (obj is Pawn Pawn)
            {
                if (Pawn.kindDef == JJKDefOf.JJK_DivineDogBlack || Pawn.kindDef == JJKDefOf.JJK_DivineDogWhite)
                {
                    Messages.Message($"{Pawn.Label} Has died!", MessageTypeDefOf.NegativeEvent, true);

                    if (HasActive())
                    {
                        DestroyActive();
                    }
                    ShouldSummonTotality = true;
                    Summon(obj.Position, null);
                }
                else if(Pawn.kindDef == JJKDefOf.JJK_DivineDogTotality)
                {
                    CanSummon = false;
                }
            }

 

          
        }

        private void TryStartAttackJob(Pawn Pawn, Pawn TargetPawn)
        {
            if (TargetPawn != null)
            {
                Job job = JobMaker.MakeJob(JJKDefOf.JJK_DemondogAttackAndVanish);
                job.SetTarget(TargetIndex.A, parent.pawn);
                job.SetTarget(TargetIndex.B, TargetPawn);
                Pawn.jobs.StartJob(job, JobCondition.None);
            }
        }
        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Values.Look(ref CanSummon, "canSummon", false);
            Scribe_Values.Look(ref ShouldSummonTotality, "shouldSummonTotality", false);
            Scribe_References.Look(ref DemonDogBlack, "demonDogBlack");
            Scribe_References.Look(ref DemonDogWhite, "demonDogWhite");
        }
    }
}

//using RimWorld;
//using Verse;
//using Verse.AI;

//namespace JJK
//{
//    public class CompProperties_AbilityDemondogSummon : CompProperties_AbilityEffect
//    {
//        public PawnKindDef demondogKindDef;

//        public CompProperties_AbilityDemondogSummon()
//        {
//            compClass = typeof(CompAbilityEffect_DemondogSummon);
//        }
//    }


//    public class CompAbilityEffect_DemondogSummon : CompAbilityEffect
//    {
//        public new CompProperties_AbilityDemondogSummon Props => (CompProperties_AbilityDemondogSummon)props;


//        private Hediff_TenShadowsUser _TenShadowsUser;
//        private Hediff_TenShadowsUser TenShadowsUser
//        {
//            get
//            {
//                if (_TenShadowsUser == null)
//                {
//                    Hediff hediff = parent.pawn.health.hediffSet.GetFirstHediffOfDef(JJKDefOf.JJK_TenShadowsUser);

//                    if (hediff is Hediff_TenShadowsUser shadowsUser)
//                    {
//                        _TenShadowsUser = shadowsUser;
//                    }
//                }

//                return _TenShadowsUser;
//            }
//        }

//        private Pawn DemonDogWhite;
//        private Pawn DemonDogBlack;
//        private Pawn DemonDogTotality;

//        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
//        {
//            base.Apply(target, dest);

//            if (DemonDogWhite != null || DemonDogBlack != null)
//            {
//                if (!DemonDogBlack.Destroyed)
//                {
//                    DemonDogBlack.Destroy();
//                }

//                if (!DemonDogWhite.Destroyed)
//                {
//                    DemonDogWhite.Destroy();
//                }
//            }
//            else
//            {
//                Map map = parent.pawn.Map;
//                IntVec3 spawnPosition = parent.pawn.Position;


//                if (!spawnPosition.Walkable(map))
//                {
//                    Log.Error("Spawn postiion is unwalkable, cannot summon divine dogs");
//                    return;
//                }



//                if (TenShadowsUser != null)
//                {
//                    if (TenShadowsUser.ShouldSummonTotalityDivineDog)
//                    {
//                        if (TenShadowsUser.CanSummonShikigamiKind(JJKDefOf.JJK_DivineDogTotality))
//                        {
//                            DemonDogTotality = TenShadowsUser.SummonShikigami(JJKDefOf.JJK_DivineDogTotality, spawnPosition, parent.pawn.Map);

//                            if (target.Pawn != null && target.Pawn.Faction.HostileTo(parent.pawn.Faction))
//                            {
//                                TryStartAttackJob(DemonDogTotality, target.Pawn);
//                            }
//                        }
//                    }
//                    else
//                    {
//                        if (TenShadowsUser.CanSummonShikigamiKind(JJKDefOf.JJK_DivineDogBlack))
//                        {
//                            DemonDogBlack = TenShadowsUser.SummonShikigami(JJKDefOf.JJK_DivineDogBlack, spawnPosition + new IntVec3(-1, 0, 0), parent.pawn.Map);

//                            if (target.Pawn != null && target.Pawn.Faction.HostileTo(parent.pawn.Faction))
//                            {
//                                TryStartAttackJob(DemonDogBlack, target.Pawn);
//                            }
//                        }

//                        if (TenShadowsUser.CanSummonShikigamiKind(JJKDefOf.JJK_DivineDogWhite))
//                        {
//                            DemonDogWhite = TenShadowsUser.SummonShikigami(JJKDefOf.JJK_DivineDogWhite, spawnPosition + new IntVec3(-1, 0, 0), parent.pawn.Map);

//                            if (target.Pawn != null && target.Pawn.Faction.HostileTo(parent.pawn.Faction))
//                            {
//                                TryStartAttackJob(DemonDogWhite, target.Pawn);
//                            }
//                        }
//                    }

//                }
//            }
//        }

//        public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
//        {
//            return base.CanApplyOn(target, dest);
//        }

//        private void TryStartAttackJob(Pawn Pawn, Pawn TargetPawn)
//        {
//            if (TargetPawn != null)
//            {
//                Job job = JobMaker.MakeJob(JJKDefOf.JJK_DemondogAttackAndVanish);
//                job.SetTarget(TargetIndex.A, parent.pawn);
//                job.SetTarget(TargetIndex.B, TargetPawn);
//                Pawn.jobs.StartJob(job, JobCondition.None);
//            }
//        }

//        public override void PostExposeData()
//        {
//            base.PostExposeData();
//            Scribe_References.Look(ref DemonDogBlack, "demonDogBlack");
//            Scribe_References.Look(ref DemonDogWhite, "demonDogWhite");
//        }
//    }
//}