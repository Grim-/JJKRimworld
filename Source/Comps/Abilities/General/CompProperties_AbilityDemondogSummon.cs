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

            Map map = parent.pawn.Map;
            IntVec3 spawnPosition = target.Cell;
            Pawn NearestValidEnemy = FindNearestEnemy(spawnPosition, map, parent.pawn.Faction);

            if (NearestValidEnemy != null)
            {
                if (spawnPosition.Walkable(map))
                {
                    SpawnDemonDog(JJKDefOf.JJK_DemonDogBlack, spawnPosition, NearestValidEnemy, map);
                    SpawnDemonDog(JJKDefOf.JJK_DemonDogWhite, spawnPosition, NearestValidEnemy, map);
                }
            }
            else
            {
                //no target found

            }
        }

        private Pawn FindNearestEnemy(IntVec3 center, Map map, Faction faction)
        {
            return (Pawn)GenClosest.ClosestThingReachable(
                center, map,
                ThingRequest.ForGroup(ThingRequestGroup.Pawn),
                PathEndMode.OnCell,
                TraverseParms.For(TraverseMode.NoPassClosedDoors),
                9999f,
                (Thing x) => x is Pawn p && p.Faction != faction && !p.Downed && !p.Dead && parent.pawn.CanReach(p, PathEndMode.Touch, Danger.Deadly)
            );
        }


        private void SpawnDemonDog(PawnKindDef KindDef, IntVec3 spawnPosition, Pawn TargetPawn, Map Map)
        {
            Pawn demondog = PawnGenerator.GeneratePawn(KindDef, parent.pawn.Faction);
            GenSpawn.Spawn(demondog, spawnPosition, Map);

            demondog.health.GetOrAddHediff(JJKDefOf.JJK_Shikigami);
            FleckMaker.ThrowSmoke(spawnPosition.ToVector3(), Map, 1.5f);
            FleckMaker.Static(spawnPosition, Map, JJKDefOf.JJK_BlackSmoke, 1.5f);

            Job job = JobMaker.MakeJob(JJKDefOf.JJK_DemondogAttackAndVanish);
            job.SetTarget(TargetIndex.A, TargetPawn);
            job.SetTarget(TargetIndex.B, parent.pawn);
            demondog.jobs.StartJob(job, JobCondition.None);
        }
    }
}