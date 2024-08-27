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
        public int TicksBetweenCost = 2500;
        public float PartHealCoefficient = 0.3f;
        public float HediffHealCoefficient = 1f;
        public float CECostPerTick = 5f;
        public float CEPartRegenCost = 100f;
        public float CEHediffCost = 30f;
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

        protected override bool IgnoreBurnout => true;

        public override int Ticks => Mathf.RoundToInt(parent.pawn.GetStatValue(JJKDefOf.JJK_RCTSpeed));

        private Gene_CursedEnergy _CursedEnergy;
        private Gene_CursedEnergy CursedEnergy
        {
            get
            {
                if (_CursedEnergy == null)
                {
                    _CursedEnergy = parent.pawn.genes?.GetFirstGeneOfType<Gene_CursedEnergy>();
                }


                return _CursedEnergy;
            }
        }



        private float RCTHealAmount => parent.pawn.GetStatValue(JJKDefOf.JJK_RCTHealingBonus, true, 100);

        public override void ApplyAbility(LocalTargetInfo target, LocalTargetInfo dest)
        {
            //do nothing the toggleable ability handles it, in order to udpate gizmo.
            // Toggle();
        }

        public override void CompTick()
        {
            base.CompTick();

            HandleMaintenanceCostTick();
        }

        private void HandleMaintenanceCostTick()
        {
            if (parent.pawn != null && IsActive && parent.pawn.IsHashIntervalTick(Props.TicksBetweenCost))
            {
                if (CursedEnergy.ValueCostMultiplied < Props.CECostPerTick)
                {
                    DeactiveOnParentAbility();
                }
                else
                {
                    CursedEnergy.ConsumeCursedEnergy(Props.CECostPerTick);
                }

            }
        }

        //still needs to actually 
        public override void OnTickInterval()
        {
            base.OnTickInterval();
            if (!IsActive || CursedEnergy == null) return;

            List<Hediff_MissingPart> MissingParts = PawnHealingUtility.GetMissingPartsPrioritized(parent.pawn);
            if (MissingParts.Count > 0)
            {
                RegenerateMissingParts();
            }
            else
            {
                HealHediffs();
            }
        }


        private void RegenerateMissingParts()
        {
            Hediff_MissingPart currentlyHealingPart = PawnHealingUtility.GetMostPrioritizedMissingPart(parent.pawn);

            float SeverityHealAmount = RCTHealAmount * Props.PartHealCoefficient;

            while (currentlyHealingPart != null && SeverityHealAmount > 0)
            {
                float PartMaxHP = currentlyHealingPart.Part.def.GetMaxHealth(parent.pawn);
                float CostToRegenPart = (PartMaxHP * Props.CEPartRegenCost) * Props.PartHealCoefficient;

                if (CursedEnergy.HasCursedEnergy(CostToRegenPart) && SeverityHealAmount > 0)
                {
                    float actualHeal = Math.Min(SeverityHealAmount, currentlyHealingPart.Severity);
                    currentlyHealingPart.Severity -= actualHeal;
                    SeverityHealAmount -= actualHeal;

                    if (currentlyHealingPart.Severity <= 0)
                    {
                        MoteMaker.ThrowText(parent.pawn.DrawPos, parent.pawn.Map, $"{currentlyHealingPart.def.label} restored on {parent.pawn.Name}", Color.green, 4f);
                        HealthUtility.Cure(currentlyHealingPart);
                        currentlyHealingPart = PawnHealingUtility.GetMostPrioritizedMissingPart(parent.pawn);
                    }

                    CursedEnergy.ConsumeCursedEnergy(CostToRegenPart);
                }
                else
                {
                    break;
                }
            }
        }


        private void HealHediffs()
        {
            Hediff currentlyHealing = PawnHealingUtility.GetMostSevereHealthProblem(parent.pawn);
            float HealingAmount = RCTHealAmount * Props.HediffHealCoefficient;

            while (currentlyHealing != null && HealingAmount > 0)
            {
                float PartCost = currentlyHealing.Severity * Props.CEHediffCost;

                if (CursedEnergy.HasCursedEnergy(PartCost) && HealingAmount > 0)
                {
                    float actualHeal = Math.Min(HealingAmount, currentlyHealing.Severity);
                    currentlyHealing.Severity -= actualHeal;
                    HealingAmount -= actualHeal;

                    if (currentlyHealing.Severity <= 0)
                    {
                        HealthUtility.Cure(currentlyHealing);
                        currentlyHealing = PawnHealingUtility.GetMostSevereHealthProblem(parent.pawn);
                    }
                }
                else
                {
                    break; // Stop healing if there's not enough CursedEnergy or HealingAmount
                }
            }
        }


        public override string CompInspectStringExtra()
        {
            string String = base.CompInspectStringExtra();

            if (IsActive) String = String + "Channeling Reverse Curse Technique";
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