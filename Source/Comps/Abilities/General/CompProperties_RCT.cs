using JJK;
using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Noise;
using static HarmonyLib.Code;

namespace JJK
{
    public class CompProperties_RCT : CompProperties_ToggleableEffect
    {
        public int PartRegenTickCount = 250;
        public float CECostPerTick = 0.005f;
        public float CEPartRegenCost = 0.1f;
        public float CEHediffCost = 0.05f;

        public float CEPartRegenSeverityBase = 0.1f;
        public float CEProblemRegenSeverityBase = 0.05f;

        public bool CanCureAddiction = false;
        public bool ShouldRemoveImplants = false;

        public CompProperties_RCT()
        {
            compClass = typeof(CompAbilityEffect_RCT);
        }
    }
    public class CompAbilityEffect_RCT : ToggleableCompAbilityEffect
    {
        new CompProperties_RCT Props => (CompProperties_RCT)props;
        private float accumulatedTicks = 0f;

        public override void ApplyAbility(LocalTargetInfo target, LocalTargetInfo dest)
        {
            Toggle();
        }

        public override void OnTickInterval()
        {
            base.OnTickInterval();

            Gene_CursedEnergy cursedEnergyGene = parent.pawn.genes?.GetFirstGeneOfType<Gene_CursedEnergy>();
            if (!IsActive || cursedEnergyGene == null)
                return;

            if (cursedEnergyGene.Value >= Props.CECostPerTick)
            {
                ConsumeCursedEnergy(Props.CECostPerTick);

                if (PawnHealingUtility.HasMissingBodyParts(parent.pawn) && cursedEnergyGene.Value >= Props.CEPartRegenCost)
                {
                    if (PawnHealingUtility.RestoreMissingPart(parent.pawn))
                    {
                        ConsumeCursedEnergy(Props.CEPartRegenCost);
                    }
                }
                else
                {
                    if (cursedEnergyGene.Value >= Props.CEHediffCost)
                    {
                        PawnHealingUtility.HealHealthProblem(parent.pawn);
                        ConsumeCursedEnergy(Props.CEHediffCost);
                    }
                }
            }
            else
            {
                JJKUtility.RemoveRCTHediff(parent.pawn);
            }
        }

        public override string CompInspectStringExtra()
        {
            string String = base.CompInspectStringExtra();

            if (IsActive) String = String + "RCT Active";
            return String;
        }

        public override void Activate()
        {
            base.Activate();

            if (!JJKUtility.HasRCTActive(parent.pawn))
            {
                JJKUtility.ApplyRCTHediffTo(parent.pawn);
            }
        }

        public override void DeActivate()
        {
            base.DeActivate();
            if (JJKUtility.HasRCTActive(parent.pawn))
            {
                JJKUtility.RemoveRCTHediff(parent.pawn);
            }
        }
    }

}