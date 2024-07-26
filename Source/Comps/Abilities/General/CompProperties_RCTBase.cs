using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace JJK
{
    public abstract class CompProperties_RCTBase : CompProperties_CursedAbilityProps
    {
        public int PartRegenTickCount = 250;
        public float CECostPerTick = 0.005f;
        public float CEPartRegenCost = 0.1f;
        public float CEHediffCost = 0.05f;

        public float CEPartRegenSeverityBase = 0.1f;
        public float CEProblemRegenSeverityBase = 0.05f;

        public bool CanCureAddiction = false;
        public bool ShouldRemoveImplants = false;
    }

    public abstract class CompAbilityEffect_RCTBase : BaseCursedEnergyAbility
    {
        protected bool IsCurrentlyCasting = false;
        //protected int CurrentTick = 0;

        public new CompProperties_RCTBase Props
        {
            get
            {
                return (CompProperties_RCTBase)this.props;
            }
        }

        protected virtual bool ShouldStopCasting()
        {
            return false;
        }

        protected virtual void StopCasting()
        {
            IsCurrentlyCasting = false;
        }

        public virtual void HealTargetPawn(Gene_CursedEnergy CursedEnergy, Pawn Target)
        {
            if (!RestoreMissingParts(CursedEnergy, Target))
            {
                HealOtherProblems(CursedEnergy, Target);
            }
        }

        public virtual void AddRCTHediff(Pawn Pawn)
        {
            Hediff NewRCT = HediffMaker.MakeHediff(JJK.JJKDefOf.RCTRegenHediff, Pawn);

            // Get the brain part
            BodyPartRecord brainPart = Pawn.health.hediffSet.GetBrain();

            // If brain is not found, try to get the head
            if (brainPart == null)
            {
                brainPart = Pawn.health.hediffSet.GetNotMissingParts()
                    .FirstOrDefault(x => x.def == BodyPartDefOf.Head);
            }

            // Add the hediff to the brain or head
            if (brainPart != null)
            {
                Pawn.health.AddHediff(NewRCT, brainPart);
            }
            else
            {
                Messages.Message("RCT cannot be applied to " + Pawn.LabelShort + ". RCT stems from the brain, which is missing.",
                      Pawn, MessageTypeDefOf.NegativeEvent);
            }
        }

        public virtual void RemoveRCTHediff(Pawn Pawn)
        {
            Hediff CurrentRCTHediff = Pawn.health.hediffSet.GetFirstHediffOfDef(JJK.JJKDefOf.RCTRegenHediff);
            if (CurrentRCTHediff != null)
            {
                Pawn.health.RemoveHediff(CurrentRCTHediff);
            }
        }
        protected virtual bool RestoreMissingParts(Gene_CursedEnergy CursedEnergy, Pawn Target)
        {
            List<Hediff_MissingPart> MissingParts = GetMissingPartsPrioritized(Target);

            if (MissingParts != null && MissingParts.Count > 0)
            {
                Hediff_MissingPart HighestPrio = MissingParts.FirstOrDefault();

                if (HighestPrio != null)
                {
                    float CostForPart = GetPartCost();
                    if (CursedEnergy.Value >= CostForPart)
                    {
                        float StatHealBonusValue = parent.pawn.GetStatValue(JJKDefOf.JJK_RCTHealingBonus);

                        // Calculate healing progress
                        float healingProgress = 0.2f + (0.1f * StatHealBonusValue);

                        // Reduce severity of the missing part
                        HighestPrio.Severity -= healingProgress;

                        // If severity reaches 0 or below, fully restore the part
                        if (HighestPrio.Severity <= 0)
                        {
                            HealthUtility.Cure(HighestPrio);
                        }

                        parent.pawn.GetCursedEnergy()?.ConsumeCursedEnergy(CostForPart);
                    }
                }

                return true;
            }

            return false;
        }
        protected virtual void HealOtherProblems(Gene_CursedEnergy CasterCursedEnergy, Pawn Target)
        {
            Hediff problem = GetOtherProblems(Target);

            if (problem != null)
            {
                float CostForProblem = GetProblemCost(problem);

                if (CasterCursedEnergy.Value >= CostForProblem)
                {
                    float healingBonus = parent.pawn.GetStatValue(JJKDefOf.JJK_RCTHealingBonus);

                    // Reduce severity more with higher healing bonus
                    float severityReduction = Mathf.Min(problem.Severity, 0.1f * (1 + healingBonus));
                    problem.Severity -= severityReduction;

                    if (problem.Severity <= 0)
                    {
                        HealthUtility.Cure(problem);
                    }

                    CasterCursedEnergy.ConsumeCursedEnergy(CostForProblem);
                }
            }
        }

        //protected virtual bool RestoreMissingParts(Gene_CursedEnergy CursedEnergy, Pawn Target)
        //{
        //    List<Hediff_MissingPart> MissingParts = GetMissingPartsPrioritized(Target);

        //    if (MissingParts != null && MissingParts.Count > 0)
        //    {
        //        Hediff_MissingPart HighestPrio = MissingParts.FirstOrDefault();

        //        if (HighestPrio != null)
        //        {
        //            float CostForPart = GetPartCost();
        //            if (CursedEnergy.Value >= CostForPart)
        //            {
        //                HealthUtility.Cure(HighestPrio);
        //                parent.pawn.GetCursedEnergy()?.ConsumeCursedEnergy(parent.pawn, CostForPart);
        //            }
        //        }

        //        return true;
        //    }

        //    return false;
        //}

        //protected virtual void HealOtherProblems(Gene_CursedEnergy CasterCursedEnergy, Pawn Target)
        //{
        //    Hediff problem = GetOtherProblems(Target);

        //    if (problem != null)
        //    {
        //        float CostForProblem = GetProblemCost(problem);

        //        if (CasterCursedEnergy.Value >= CostForProblem)
        //        {
        //            HealthUtility.Cure(problem);
        //            CasterCursedEnergy.ConsumeCursedEnergy(parent.pawn, CostForProblem);
        //        }
        //    }
        //}


        protected List<Hediff_MissingPart> GetMissingPartsPrioritized(Pawn pawn)
        {
            return pawn.health.hediffSet.hediffs
                .OfType<Hediff_MissingPart>()
                .OrderByDescending(x => x.TendPriority)
                .ToList();
        }


        public virtual float GetPartCost()
        {
            return Props.CEPartRegenCost * parent.pawn.GetStatValue(JJKDefOf.JJK_CursedEnergyCost);
        }
        public virtual float GetTickCost()
        {
            return  Props.CECostPerTick * parent.pawn.GetStatValue(JJKDefOf.JJK_CursedEnergyCost);
        }

        public virtual float GetProblemCost(Hediff ProblemDiff)
        {
            return (Props.CEProblemRegenSeverityBase * ProblemDiff.Severity) * parent.pawn.GetStatValue(JJKDefOf.JJK_CursedEnergyCost);
        }

        protected Hediff GetOtherProblems(Pawn pawn)
        {
            return pawn.health.hediffSet.hediffs.Where(hediff =>
            {
                bool RemoveImplant = Props.ShouldRemoveImplants && hediff.GetType() == typeof(Hediff_Implant);
                bool CureAddiction = Props.CanCureAddiction && hediff.GetType() == typeof(Hediff_Addiction);

                return CureAddiction || RemoveImplant;
            })
            .OrderByDescending(x => x.TendPriority)
            .FirstOrDefault();
        }

        public override float GetCost()
        {
            return Props.cursedEnergyCost;
        }

        public abstract override void ApplyAbility(LocalTargetInfo target, LocalTargetInfo dest);
    }
}

    