using RimWorld;
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

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);

            if (target.Pawn == null)
            {
                //error here
                return;
            }

            Map map = parent.pawn.Map;
            IntVec3 spawnPosition = parent.pawn.Position;

            if (spawnPosition.Walkable(map))
            {
                SpawnDemonDog(JJKDefOf.JJK_DemonDogBlack, spawnPosition + new IntVec3(-1, 0, 0), target.Pawn, map);
                SpawnDemonDog(JJKDefOf.JJK_DemonDogWhite, spawnPosition + new IntVec3(1, 0, 0), target.Pawn, map);
            }
        }

        public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
        {
            return base.CanApplyOn(target, dest) && target.Pawn.Faction != parent.pawn.Faction;
        }

        private void SpawnDemonDog(PawnKindDef KindDef, IntVec3 spawnPosition, Pawn TargetPawn, Map Map)
        {
            Pawn demondog = JJKUtility.SpawnShikigami(KindDef, parent.pawn, Map, spawnPosition);

            if (demondog != null)
            {
                FleckMaker.ThrowSmoke(spawnPosition.ToVector3(), Map, 1.5f);
                FleckMaker.Static(spawnPosition, Map, JJKDefOf.JJK_BlackSmoke, 1.5f);

                Job job = JobMaker.MakeJob(JJKDefOf.JJK_DemondogAttackAndVanish);
                job.SetTarget(TargetIndex.A, parent.pawn);
                job.SetTarget(TargetIndex.B, TargetPawn);
                demondog.jobs.StartJob(job, JobCondition.None);
            }
        }
    }
}