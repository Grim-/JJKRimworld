using RimWorld;

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
}

    