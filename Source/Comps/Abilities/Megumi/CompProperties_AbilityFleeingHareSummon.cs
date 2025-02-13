using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace JJK
{
    public class CompProperties_AbilityFleeingHareSummon : CompProperties_BaseShikigami
    {
        public int NumberToSpawn = 7;

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
            Pawn mainHaire = TenShadowUser.GetOrGenerateShikigami(Props.shikigamiDef, Props.shikigamiDef.shikigami, Position, Map, true);

            if (mainHaire.TryGetComp(out Comp_FleeingHareSummon fleeingHareSummon))
            {
                fleeingHareSummon.SetMainFleeingHare(mainHaire);
            }

            for (int i = 1; i < Props.NumberToSpawn; i++)
            {
                IntVec3 RandomSpawnPosition = Position + new IntVec3(Rand.Range(-2, 2), Position.y, Rand.Range(-2, 2));

                if (RandomSpawnPosition.InBounds(Map))
                {
                    CompAbilityEffect_FleeingHareSummon.GenerateNewFleeingHare(TenShadowUser, mainHaire, Position, Map);
                }
            }
        }


        public static Pawn GenerateNewFleeingHare(TenShadowGene TenShadowsUser, Pawn MainHare, IntVec3 Position, Map Map)
        {
            if (TenShadowsUser == null)
            {
                return null;
            }

            Pawn tempRabbit = TenShadowsUser.GetOrGenerateShikigami(JJKDefOf.Shikigami_FleeingHares, JJKDefOf.Shikigami_FleeingHares.shikigami, Position, Map, true);
            //tempRabbit.story.skinColorOverride = Color.gray;
            if (tempRabbit.TryGetComp(out Comp_FleeingHareSummon tempFleeingHareSummon) && MainHare != null)
            {
                tempFleeingHareSummon.SetMainFleeingHare(MainHare);
            }
            
            return tempRabbit;
        }
    }


}