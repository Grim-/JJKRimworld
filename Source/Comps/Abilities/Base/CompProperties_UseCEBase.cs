using RimWorld;
using Verse;

namespace JJK
{
    public class CompProperties_UseCEBase : CompProperties_AbilityEffect
    {
        public float cursedEnergyCost = 0.1f;
        public int cooldownTicks = 2500;
        public float burnoutStrain = 0f;
    }

    // Base Component class
    public abstract class CompAbilityEffect_UseCEBase : CompAbilityEffect
    {
        public new CompProperties_UseCEBase Props => (CompProperties_UseCEBase)props;
        protected Gene_CursedEnergy _CursedEnergy;
        protected abstract Gene_CursedEnergy CursedEnergy { get; }

        protected bool IsOnCooldown;
        protected int CurrentCDTick = 0;
        protected virtual float CursedEnergyCostMult => parent.pawn.GetStatValue(JJKDefOf.JJK_CursedEnergyCost, true, 100);
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

            if (parent?.pawn == null)
            {
                reason = "AbilityDisabledNoPawn".Translate();
                return true;
            }

            if (parent.pawn.genes?.HasActiveGene(JJKDefOf.Gene_JJKHeavenlyPact) == true)
            {
                reason = "AbilityDisabledHeavenlyPact".Translate(parent.pawn);
                return true;
            }

            if (ShouldDisableBecauseNoCE(CastCost))
            {
                reason = "AbilityDisabledNoCursedEnergy".Translate(parent.pawn);
                return true;
            }

            if (parent.pawn.health?.hediffSet?.HasHediff(JJKDefOf.JJK_CursedTechniqueBurnout) == true && !IgnoreBurnout)
            {
                reason = "AbilityDisabledCursedTechniqueBurnout".Translate(parent.pawn);
                return true;
            }

            reason = null;
            return false;
        }

        public virtual bool ShouldDisableBecauseNoCE(float Cost)
        {
            if (CursedEnergy == null) return true;
            if (parent?.pawn?.RaceProps == null) return true;

            return parent.pawn.RaceProps.Humanlike ? !CursedEnergy.HasCursedEnergy(Cost) : false;
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
            if (Props.burnoutStrain != 0)
            {
                Hediff strain = pawn.health.GetOrAddHediff(JJKDefOf.JJK_CursedTechniqueStrain);
                strain.Severity += Props.burnoutStrain;
            }
        }
    }
}

