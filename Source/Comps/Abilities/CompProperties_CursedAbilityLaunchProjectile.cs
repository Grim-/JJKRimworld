using RimWorld;
using Verse;

namespace JJK
{
    public class CompProperties_CursedAbilityLaunchProjectile : CompProperties_AbilityLaunchProjectile
    {
        public float cursedEnergyCost = 10f;

        public CompProperties_CursedAbilityLaunchProjectile()
        {
            compClass = typeof(CursedCompAbilityEffect_LaunchProjectile);
        }
    }

    public class CursedCompAbilityEffect_LaunchProjectile : CompAbilityEffect_LaunchProjectile
    {
        new CompProperties_CursedAbilityLaunchProjectile Props => (CompProperties_CursedAbilityLaunchProjectile)props;

        public float GetCost()
        {
            return Props.cursedEnergyCost;
        }

        public override bool GizmoDisabled(out string reason)
        {
            Gene_CursedEnergy gene_Hemogen = parent.pawn.genes?.GetFirstGeneOfType<Gene_CursedEnergy>();
            if (gene_Hemogen == null)
            {
                reason = "AbilityDisabledNoCursedEnergyGENE";
                return true;
            }

            if (parent.pawn.genes.HasActiveGene(JJKDefOf.Gene_JJKHeavenlyPact))
            {
                reason = "AbilityDisabledHeavenlyPact".Translate(parent.pawn);
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

    }
}

    