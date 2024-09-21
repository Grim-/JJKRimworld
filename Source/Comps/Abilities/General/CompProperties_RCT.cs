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

        //still needs to actually 
        public override void OnTickInterval()
        {
            base.OnTickInterval();
            if (!IsActive || CursedEnergy == null) return;


            Log.Message($"RCT On Tick");
            HealTarget(Target);
        }

        public void HealTarget(Pawn TargetPawn)
        {
            HealPawnHealthIssues(TargetPawn, PawnHealingUtility.GetMissingPartsPrioritized(TargetPawn));
        }

        private void HealPawnHealthIssues(Pawn targetPawn, List<Hediff_MissingPart> MissingParts)
        {
            Hediff currentIssue = null;
            float healAmount;

            if (MissingParts != null && MissingParts.Count > 0)
            {
                currentIssue = PawnHealingUtility.GetMostPrioritizedMissingPart(targetPawn);
                healAmount = GetHealValue(Props.PartHealCoefficient);
            }
            else
            {
                currentIssue = PawnHealingUtility.GetMostSevereHealthProblem(targetPawn);
                healAmount = GetHealValue(Props.HediffHealCoefficient);
            }

            while (currentIssue != null && healAmount > 0)
            {
                float partMaxHP = 1f;
                if (currentIssue.Part != null && currentIssue.Part.def != null)
                {
                    partMaxHP = currentIssue.Part.def.GetMaxHealth(targetPawn);
                }

                float costPerHealPoint;
                if (MissingParts != null && MissingParts.Count > 0)
                {
                    costPerHealPoint = (Props.CEPartRegenCost * partMaxHP * Props.PartHealCoefficient) / currentIssue.Severity;
                }
                else
                {
                    costPerHealPoint = Props.CEHediffCost;
                }

                float maxPossibleHeal = Math.Min(healAmount, currentIssue.Severity);
                float availableCursedEnergy = CursedEnergy.Value;
                float actualHeal = Math.Min(maxPossibleHeal, availableCursedEnergy / costPerHealPoint);

                if (actualHeal > 0)
                {
                    float actualCost = actualHeal * costPerHealPoint;

                   //LogHealingAttempt(currentIssue, partMaxHP, actualHeal, actualCost, regenerateMissingParts);

                    currentIssue.Severity -= actualHeal;
                    healAmount -= actualHeal;
                    CursedEnergy.ConsumeCursedEnergy(actualCost);

                    if (currentIssue.Severity <= 0)
                    {
                        if (MissingParts != null && MissingParts.Count > 0)
                        {
                            MoteMaker.ThrowText(targetPawn.DrawPos, targetPawn.Map, "Regenerated : " + currentIssue.Label, Color.green, 4f);
                        }
                        else
                        {
                            MoteMaker.ThrowText(targetPawn.DrawPos, targetPawn.Map, "Healed : " + currentIssue.Label, Color.green, 4f);
                        }
                        HealthUtility.Cure(currentIssue);

                        if (MissingParts != null && MissingParts.Count > 0)
                        {
                            currentIssue = PawnHealingUtility.GetMostPrioritizedMissingPartFromList(targetPawn, MissingParts);
                        }
                        else
                        {
                            currentIssue = PawnHealingUtility.GetMostSevereHealthProblem(targetPawn);
                        }
                    }

                    //Log.Message(" Heal power remaining : " + healAmount.ToString());
                   // Log.Message(" Cursed Energy remaining : " + CursedEnergy.Value);
                }
                else
                {
                    DeactiveOnParentAbility();
                    break; // No healing possible due to lack of Cursed Energy
                }
            }
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
            return RCTHealAmount* CoEfficient;
        }





        public override string CompInspectStringExtra()
        {
            string String = base.CompInspectStringExtra();

            if (IsActive) String = String + "Channeling Reverse Curse Technique" + $" healing every {Ticks} ticks.";
            return String;
        }

        public override void Activate()
        {
            base.Activate();

            if (Props.AuraEffecter != null && AuraEffecter == null)
            {
                AuraEffecter = Props.AuraEffecter.SpawnAttached(Target, Target.MapHeld);

            }

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
    }
}