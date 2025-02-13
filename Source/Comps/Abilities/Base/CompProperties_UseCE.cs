using Verse;

namespace JJK
{
    // Original Properties class
    public class CompProperties_UseCE : CompProperties_UseCEBase
    {
        public CompProperties_UseCE()
        {
            compClass = typeof(CompAbilityEffect_UseCE);
        }
    }

    // Original Component class
    public class CompAbilityEffect_UseCE : CompAbilityEffect_UseCEBase
    {
        protected override Gene_CursedEnergy CursedEnergy
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
    }
}

