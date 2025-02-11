using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace JJK
{
    public class CompProperties_AbilityFleeingHareSummon : CompProperties_BaseShikigami
    {
        public int NumberToSpawn = 20;

        public CompProperties_AbilityFleeingHareSummon()
        {
            compClass = typeof(CompAbilityEffect_FleeingHareSummon);
        }
    }
    public class CompAbilityEffect_FleeingHareSummon : CompBaseShikigamiSummon
    {
        public new CompProperties_AbilityFleeingHareSummon Props => (CompProperties_AbilityFleeingHareSummon)props;

        protected override void SummonShikigami(IntVec3 Position, Map Map)
        {
            for (int i = 0; i < Props.NumberToSpawn; i++)
            {
                IntVec3 RandomSpawnPosition = Position + new IntVec3(Rand.Range(-2, 2), Position.y, Rand.Range(-2, 2));

                if (RandomSpawnPosition.InBounds(Map))
                {
                    TenShadowUser.GetOrGenerateShikigami(Props.shikigamiDef, Props.shikigamiDef.shikigami, RandomSpawnPosition, Map, true);
                }
            }
        }

    }


}