using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace JJK
{

    public abstract class CompProperties_CursedAbilityProps : CompProperties_AbilityEffect
    {
        public float cursedEnergyCost = 0.1f;
        public int cooldownTicks = 2500;
        public float burnoutStrain = 0f;
    }

    public abstract class BaseCursedEnergyAbility : CompAbilityEffect
    {
        public virtual new CompProperties_CursedAbilityProps Props
        {
            get
            {
                return (CompProperties_CursedAbilityProps)this.props;
            }
        }

        private Gene_CursedEnergy _CursedEnergy;
        private Gene_CursedEnergy CursedEnergy
        {
            get
            {
                if (_CursedEnergy == null)
                {
                    _CursedEnergy = parent.pawn.GetCursedEnergy();
                }

                return _CursedEnergy;
            }
        }
        protected bool IsOnCooldown;


        protected int CurrentCDTick = 0;


        protected float CursedEnergyCostMult => parent.pawn.GetStatValue(JJKDefOf.JJK_CursedEnergyCost, true, 100);
        protected virtual float BaseCost => Props.cursedEnergyCost;
        protected virtual float CastCost => BaseCost * CursedEnergyCostMult;


        protected virtual bool IgnoreBurnout => false;


        public override bool GizmoDisabled(out string reason)
        {
            if (CursedEnergy == null)
            {
                reason = "AbilityDisabledNoCursedEnergyGENE".Translate(parent.pawn);
                return true;
            }

            if (parent.pawn.genes.HasActiveGene(JJKDefOf.Gene_JJKHeavenlyPact))
            {
                reason = "AbilityDisabledHeavenlyPact".Translate(parent.pawn);
                return true;
            }

            if (ShouldDisableBecauseNoCE(CastCost))
            {
                reason = "AbilityDisabledNoCursedEnergy".Translate(parent.pawn);
                return true;
            }

            if (parent.pawn.health.hediffSet.HasHediff(JJKDefOf.JJK_CursedTechniqueBurnout) && !IgnoreBurnout)
            {
                reason = "AbilityDisabledCursedTechniqueBurnout".Translate(parent.pawn);
                return true;
            }
            reason = null;
            return false;
        }

        public virtual bool ShouldDisableBecauseNoCE(float Cost)
        {
            return parent.pawn.RaceProps.Humanlike ? !CursedEnergy.HasCursedEnergy(CastCost) : false;
        }

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            ApplyAbility(target, dest);
        }

        public abstract void ApplyAbility(LocalTargetInfo target, LocalTargetInfo dest);
    }


}

