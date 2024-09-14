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

            if (Summons.Count > 0)
            {
                DestroySummons();
            }
            else
            {
                for (int i = 0; i < Props.NumberToSpawn; i++)
                {
                    IntVec3 RandomSpawnPosition = target.Cell + new IntVec3(Rand.Range(-2, 2), target.Cell.y, Rand.Range(-2, 2));

                    if (RandomSpawnPosition.InBounds(parent.pawn.MapHeld))
                    {
                        SpawnFleeingHare(Props.fleeingHareKindDef, RandomSpawnPosition, parent.pawn.MapHeld);
                    }
                }
            }

        }

        private void SpawnFleeingHare(PawnKindDef KindDef, IntVec3 spawnPosition, Map Map)
        {
            Pawn demondog = JJKUtility.SpawnShikigami(KindDef, parent.pawn, Map, spawnPosition);

            if (!Summons.Contains(demondog))
            {
                Summons.Add(demondog);
            }
        }


        private void DestroySummons()
        {
            foreach (var item in Summons)
            {
                if (!item.Destroyed)
                {
                    item.Destroy();
                }     
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