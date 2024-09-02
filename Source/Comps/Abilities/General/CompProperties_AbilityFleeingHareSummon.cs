using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace JJK
{
    public class CompProperties_AbilityFleeingHareSummon : CompProperties_AbilityEffect
    {
        public PawnKindDef fleeingHareKindDef;
        public int NumberToSpawn = 20;

        public CompProperties_AbilityFleeingHareSummon()
        {
            compClass = typeof(CompAbilityEffect_FleeingHareSummon);
        }
    }
    public class CompAbilityEffect_FleeingHareSummon : CompAbilityEffect
    {
        public new CompProperties_AbilityFleeingHareSummon Props => (CompProperties_AbilityFleeingHareSummon)props;
        private List<Pawn> Summons = new List<Pawn>();

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            Map map = parent.pawn.Map;
            IntVec3 spawnPosition = target.Cell;

            if (Summons.Count > 0)
            {
                DestroySummons();
            }
            else
            {
                if (spawnPosition.Walkable(map))
                {
                    for (int i = 0; i < Props.NumberToSpawn; i++)
                    {
                        SpawnFleeingHare(Props.fleeingHareKindDef, spawnPosition + new IntVec3(Rand.Range(-2, 2), spawnPosition.y, Rand.Range(-2, 2)), map);
                    }

                }
            }


        }

        private void SpawnFleeingHare(PawnKindDef KindDef, IntVec3 spawnPosition, Map Map)
        {
            Pawn demondog = JJKUtility.SpawnShikigami(KindDef, parent.pawn, Map, spawnPosition);
            Job job = JobMaker.MakeJob(JJKDefOf.JJK_DefendMaster);
            job.SetTarget(TargetIndex.A, parent.pawn);
            demondog.jobs.StartJob(job, JobCondition.None);


            if (!Summons.Contains(demondog))
            {
                Summons.Add(demondog);
            }
        }


        private void DestroySummons()
        {
            foreach (var item in Summons)
            {
                item.Destroy();
            }

            Summons.Clear();
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Collections.Look(ref Summons, "fleeingHares", LookMode.Reference);
        }
    }


}