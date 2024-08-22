using RimWorld;
using Verse;
using Verse.Noise;

namespace JJK
{
    public abstract class BaseCursedEnergyAbility : CompAbilityEffect
    {
        public virtual new CompProperties_CursedAbilityProps Props
        {
            get
            {
                return (CompProperties_CursedAbilityProps)this.props;
            }
        }


        protected bool IsOnCooldown;


        protected int CurrentCDTick = 0;


        protected float CursedEnergyCostMult => parent.pawn.GetStatValue(JJKDefOf.JJK_CursedEnergyCost);
        protected virtual float BaseCost => Props.cursedEnergyCost;
        protected virtual float CastCost => BaseCost * CursedEnergyCostMult;



        public override bool GizmoDisabled(out string reason)
        {
            Gene_CursedEnergy gene_Hemogen = parent.pawn.GetCursedEnergy();

            if (gene_Hemogen == null)
            {
                reason = "AbilityDisabledNoCursedEnergyGENE".Translate(parent.pawn);
                return true;
            }

            if (parent.pawn.genes.HasActiveGene(JJKDefOf.Gene_JJKHeavenlyPact))
            {
                reason = "AbilityDisabledHeavenlyPact".Translate(parent.pawn);
                return true;
            }

            if (gene_Hemogen.Value < CastCost)
            {
                reason = "AbilityDisabledNoCursedEnergy".Translate(parent.pawn);
                return true;
            }

            reason = null;
            return false;
        }

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            ApplyAbility(target, dest);
            PostApply(target, dest);
        }

        public abstract void ApplyAbility(LocalTargetInfo target, LocalTargetInfo dest);

        public virtual void PostApply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            ApplyAbilityCost(parent.pawn);
        }

        public virtual void ApplyAbilityCost(Pawn Pawn)
        {
            ConsumeCursedEnergy(CastCost * CursedEnergyCostMult);
        }

        public virtual void ConsumeCursedEnergy(float Amount)
        {
            Gene_CursedEnergy _CursedEnergy = parent.pawn.GetCursedEnergy();

            if (_CursedEnergy != null)
            {
                _CursedEnergy.ConsumeCursedEnergy(Amount * CursedEnergyCostMult);
            }
            else
            {
                Log.Error($"No CursedEnergy Gene found for {parent.pawn.Name}, cannot consume cursed energy.");
            }
        }

        public virtual bool HasCursedEnergy(Pawn Pawn, float Cost)
        {
            Gene_CursedEnergy _CursedEnergy = Pawn.GetCursedEnergy();
            if (_CursedEnergy != null)
            {
                return _CursedEnergy.Value >= Cost;
            }
            return false;
        }
    }
}


