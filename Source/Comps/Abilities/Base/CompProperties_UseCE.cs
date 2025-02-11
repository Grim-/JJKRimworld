using RimWorld;
using Verse;

namespace JJK
{
    public class CompProperties_UseCE : CompProperties_AbilityEffect
    {
        public float cursedEnergyCost = 0.1f;
        public int cooldownTicks = 2500;
        public float burnoutStrain = 0f;

        public CompProperties_UseCE()
        {
            compClass = typeof(CompAbilityEffect_UseCE);
        }
    }

    public class CompAbilityEffect_UseCE : CompAbilityEffect
    {
        public new CompProperties_UseCE Props => (CompProperties_UseCE)props;

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
            ApplyAbilityCost(parent.pawn);
            ApplyCursedTechniqueStrain(parent.pawn);
        }

        public virtual void ApplyAbilityCost(Pawn Pawn)
        {
            CursedEnergy?.ConsumeCursedEnergy(CastCost);
        }
        protected virtual void ApplyCursedTechniqueStrain(Pawn pawn)
        {
            Hediff strain = pawn.health.GetOrAddHediff(JJKDefOf.JJK_CursedTechniqueStrain);
            strain.Severity += Props.burnoutStrain;
        }
    }
}

