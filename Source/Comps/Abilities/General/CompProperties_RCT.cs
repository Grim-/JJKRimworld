﻿using JJK;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public float HealCoefficient = 3f;
        public float CECostPerTick = 5f;
        public float CEPartRegenCost = 100f;
        public float CEHediffCost = 30f;
        public bool CanCureAddiction = false;
        public bool ShouldRemoveImplants = false;
        public EffecterDef AuraEffecter;

        public CompProperties_RCT()
        {
            compClass = typeof(CompAbilityEffect_RCT);
        }
    }
    public class CompAbilityEffect_RCT : ToggleableCompAbilityEffect
    {
        new CompProperties_RCT Props => (CompProperties_RCT)props;

        protected override bool IgnoreBurnout => true;

        public override int Ticks => Mathf.RoundToInt(Props.Ticks * RCTSpeedAmount);

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


        private Effecter AuraEffecter;


        protected virtual Pawn Caster => parent.pawn;
        protected virtual Pawn Target => parent.pawn;

        private float RCTSpeedAmount => parent.pawn.GetStatValue(JJKDefOf.JJK_RCTSpeed, true, 100);
        private float RCTHealAmount => parent.pawn.GetStatValue(JJKDefOf.JJK_RCTHealingBonus, true, 100);


        public override string ExtraTooltipPart()
        {
            string baseTooltip = base.ExtraTooltipPart();
            float estimatedCost = EstimateTotalHealingCost();

            if (string.IsNullOrEmpty(baseTooltip))
            {
                return $"Estimated CE cost to heal all conditions: {estimatedCost:F1}";
            }

            return baseTooltip + $"\nEstimated CE cost to heal all conditions: {estimatedCost:F1}";
        }


        public override void ApplyAbility(LocalTargetInfo target, LocalTargetInfo dest)
        {
            //do nothing the toggleable ability handles it, in order to udpate gizmo.
            // Toggle();
        }

        public override void CompTick()
        {
            base.CompTick();

            if (AuraEffecter != null)
            {
                AuraEffecter.EffectTick(parent.pawn, parent.pawn);
            }


            HandleMaintenanceCostTick();
        }
        private float EstimateTotalHealingCost()
        {
            if (Target == null) return 0f;

            float totalCost = 0f;

            // Estimate cost for missing parts
            List<Hediff_MissingPart> missingParts = PawnHealingUtility.GetMissingPartsPrioritized(Target);
            if (missingParts != null)
            {
                foreach (var part in missingParts)
                {
                    totalCost += GetCursedEnergyCost(part.Severity);
                }
            }

            // Estimate cost for injuries
            List<Hediff> injuries = PawnHealingUtility.GetMostSevereHealthProblems(Target);
            if (injuries != null)
            {
                foreach (var injury in injuries)
                {
                    totalCost += GetCursedEnergyCost(injury.Severity);
                }
            }

            // Estimate cost for blood loss
            Hediff bloodLoss = Target.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.BloodLoss);
            if (bloodLoss != null)
            {
                totalCost += GetCursedEnergyCost(bloodLoss.Severity);
            }

            // Estimate cost for burnout
            Hediff cursedTechniqueBurnout = Target.health.hediffSet.GetFirstHediffOfDef(JJKDefOf.JJK_CursedTechniqueBurnout);
            if (cursedTechniqueBurnout != null)
            {
                totalCost += GetCursedEnergyCost(cursedTechniqueBurnout.Severity);
            }

            return totalCost;
        }

        private void HandleMaintenanceCostTick()
        {
            if (Target != null && IsActive && Target.IsHashIntervalTick(Props.TicksBetweenCost))
            {
                if (CursedEnergy.ValueCostMultiplied < Props.CECostPerTick)
                {
                    DeActivate();
                }
                else
                {
                    CursedEnergy.ConsumeCursedEnergy(Props.CECostPerTick);
                }

            }
        }

  
        public override void OnTickInterval()
        {
            base.OnTickInterval();
            if (!IsActive || CursedEnergy == null) return;
            Log.Message($"RCT On Tick");
            RCTHealTick(Target);
        }


        //restore parts, then injuries, then blood, then JJK_CursedTechniqueBurnout hediff severity
        private void RCTHealTick(Pawn targetPawn)
        {
            float healAmount = GetHealValue(Props.HealCoefficient);
            healAmount = HealMissingParts(targetPawn, healAmount);
            healAmount = HealInjuries(targetPawn, healAmount);
            healAmount = RestoreBlood(targetPawn, healAmount);
            ReduceCursedTechniqueBurnout(targetPawn, healAmount);
        }

        private float HealMissingParts(Pawn targetPawn, float healAmount)
        {
            List<Hediff_MissingPart> missingParts = PawnHealingUtility.GetMissingPartsPrioritized(targetPawn);
            if (missingParts == null || missingParts.Count == 0) 
                return healAmount;

            foreach (var missingPart in missingParts)
            {
                float severity = missingPart.Severity;
                float costToHeal = GetCursedEnergyCost(severity);

                //float currentHP = missingPart.Part.
                //float partMaxHP = missingPart.Part.def.GetMaxHealth(targetPawn);



                Log.Message($"Missing part :: Severity {severity} Cost To Heal (in CE) {costToHeal} HealAmount {healAmount}");


                if (!CursedEnergy.HasCursedEnergy(costToHeal))
                {
                    Log.Message($"Missing part requres {costToHeal} but pawn only has {CursedEnergy.Value}");
                    break;
                }

                missingPart.Heal(healAmount);
                healAmount -= severity;
                CursedEnergy.ConsumeCursedEnergy(costToHeal);

                if (missingPart.Severity <= 0)
                {
                    HealthUtility.Cure(missingPart);
                }

                if (healAmount <= 0)
                {
                    break;
                }
            }

            return healAmount;
        }

        private float HealInjuries(Pawn targetPawn, float healAmount)
        {
            List<Hediff> injuries = PawnHealingUtility.GetMostSevereHealthProblems(targetPawn);

            if (injuries != null)
            {
                foreach (var injury in injuries)
                {
                    if (healAmount <= 0) break;

                    float severityToHeal = Math.Min(injury.Severity, healAmount);
                    float costToHeal = GetCursedEnergyCost(severityToHeal);

                    if (!CursedEnergy.HasCursedEnergy(costToHeal)) break;

                    injury.Severity -= severityToHeal;
                    healAmount -= severityToHeal;
                    CursedEnergy.ConsumeCursedEnergy(costToHeal);

                    if (injury.Severity <= 0)
                    {
                        HealthUtility.Cure(injury);
                    }
                }
            }



            return healAmount;
        }

        private float RestoreBlood(Pawn targetPawn, float healAmount)
        {
            Hediff bloodLoss = targetPawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.BloodLoss);
            if (bloodLoss == null || healAmount <= 0) return healAmount;

            float bloodToRestore = Math.Min(bloodLoss.Severity, healAmount);
            float costToRestore = GetCursedEnergyCost(bloodToRestore);

            if (!CursedEnergy.HasCursedEnergy(costToRestore)) return healAmount;

            bloodLoss.Severity -= bloodToRestore;
            healAmount -= bloodToRestore;
            CursedEnergy.ConsumeCursedEnergy(costToRestore);

            if (bloodLoss.Severity <= 0)
            {
                HealthUtility.Cure(bloodLoss);
            }

            return healAmount;
        }

        private void ReduceCursedTechniqueBurnout(Pawn targetPawn, float healAmount)
        {
            Hediff cursedTechniqueBurnout = targetPawn.health.hediffSet.GetFirstHediffOfDef(JJKDefOf.JJK_CursedTechniqueBurnout);
            if (cursedTechniqueBurnout == null || healAmount <= 0) return;

            float severityToReduce = Math.Min(cursedTechniqueBurnout.Severity, healAmount);
            float costToReduce = GetCursedEnergyCost(severityToReduce);

            if (!CursedEnergy.HasCursedEnergy(costToReduce)) return;

            cursedTechniqueBurnout.Severity -= severityToReduce;
            CursedEnergy.ConsumeCursedEnergy(costToReduce);

            if (cursedTechniqueBurnout.Severity <= 0)
            {
                targetPawn.health.RemoveHediff(cursedTechniqueBurnout);
            }
        }

        private float GetCursedEnergyCost(float severity)
        {
            return 20 * Mathf.Min(1, severity);
        }
        private void LogHealingAttempt(Hediff issue, float partMaxHP, float healAmount, float cost, bool isRegeneration)
        {
            string issueType = isRegeneration ? "part" : "health issue";
            Log.Message("Attempting to heal " + issueType + " " + issue.Label);
            Log.Message(" HP : (" + (issue.Part != null && issue.Part.def != null ? issue.Part.def.hitPoints.ToString() : "0") + " / " + partMaxHP.ToString() + ")");
            Log.Message(" Heal amount : " + healAmount.ToString());
            Log.Message(" CE Cost : " + cost.ToString());
        }

        private float GetHealValue(float CoEfficient)
        {
            return RCTHealAmount * CoEfficient;
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

            if (AuraEffecter != null)
            {
                AuraEffecter.Cleanup();

                AuraEffecter = null;
            }


            if (JJKUtility.HasRCTActive(Target))
            {
                JJKUtility.RemoveRCTHediff(Target);
            }

            DeactiveOnParentAbility();
        }




        public override string CompInspectStringExtra()
        {
            string String = base.CompInspectStringExtra();

            if (IsActive) String = String + "Channeling Reverse Curse Technique" + $" healing every {Ticks} ticks.";
            return String;
        }

    }
}