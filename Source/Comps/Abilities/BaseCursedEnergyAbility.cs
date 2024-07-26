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

            if (IsOnCooldown)
            {
                reason = "AbilityOnCooldown".Translate();
                return true;
            }

            if (gene_Hemogen.Value < GetCost())
            {
                reason = "AbilityDisabledNoCursedEnergy".Translate(parent.pawn);
                return true;
            }

            reason = null;
            return false;
        }


        protected virtual float TotalCursedEnergyCostOfQueuedAbilities()
        {
            float totalCost = 0f;

            // Check current job
            if (parent.pawn.jobs?.curJob?.verbToUse is Verb_CastAbility currentAbility)
            {

                totalCost += currentAbility.ability?.HemogenCost() * parent.pawn.GetStatValue(JJKDefOf.JJK_CursedEnergyCost) ?? 1f;
            }

            // Check job queue
            if (parent.pawn.jobs?.jobQueue != null)
            {
                foreach (var queuedJob in parent.pawn.jobs.jobQueue)
                {
                    if (queuedJob.job.verbToUse is Verb_CastAbility queuedAbility)
                    {
                        totalCost += queuedAbility.ability?.HemogenCost() * parent.pawn.GetStatValue(JJKDefOf.JJK_CursedEnergyCost) ?? 1f;
                    }
                }
            }

            return totalCost;
        }

        public virtual float GetCost()
        {
            return Props.cursedEnergyCost * this.parent.pawn.GetStatValue(JJKDefOf.JJK_CursedEnergyCost);
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

            StartCooldown();
        }


        public override void CompTick()
        {
            base.CompTick();

            if (IsOnCooldown)
            {
                CurrentCDTick++;

                if (CurrentCDTick >= Props.cooldownTicks)
                {
                    IsOnCooldown = false;
                    CurrentCDTick = 0;
                }
            }

        }

        public virtual void StartCooldown()
        {
            if (Props.cooldownTicks > 0)
            {
                IsOnCooldown = true;
                CurrentCDTick = 0;
            }
        }

        public virtual void ApplyAbilityCost(Pawn Pawn)
        {
            parent.pawn.GetCursedEnergy()?.ConsumeCursedEnergy(GetCost() * parent.pawn.GetStatValue(JJKDefOf.JJK_CursedEnergyCost));
        }
    }
}


