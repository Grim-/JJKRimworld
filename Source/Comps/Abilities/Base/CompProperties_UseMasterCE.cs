using Verse;

namespace JJK
{
    // New Properties class for summons
    public class CompProperties_UseMasterCE : CompProperties_UseCEBase
    {
        public CompProperties_UseMasterCE()
        {
            compClass = typeof(CompAbilityEffect_UseMasterCE);
        }
    }
    // New Component class for summons
    public class CompAbilityEffect_UseMasterCE : CompAbilityEffect_UseCEBase
    {
        protected override Gene_CursedEnergy CursedEnergy
        {
            get
            {
                if (_CursedEnergy == null)
                {
                    var tenShadowsComp = parent.pawn.GetComp<Comp_TenShadowsSummon>();
                    if (tenShadowsComp != null && tenShadowsComp.TenShadowsUser != null)
                    {
                        _CursedEnergy = tenShadowsComp.TenShadowsUser.CursedEnergy;
                    }
                }
                return _CursedEnergy;
            }
        }

        public override bool ShouldDisableBecauseNoCE(float Cost)
        {
            return CursedEnergy == null || !CursedEnergy.HasCursedEnergy(Cost);
        }

        protected override float CursedEnergyCostMult => CursedEnergy == null ? 1 : CursedEnergy.CostMult;

        public override void ApplyAbilityCost(Pawn Pawn)
        {
            CursedEnergy?.ConsumeCursedEnergy(CastCost);
        }

    }
}

