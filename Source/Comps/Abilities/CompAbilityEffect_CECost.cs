using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace JJK
{
    internal class CompAbilityEffect_CECost : BaseCursedEnergyAbility
    {
        public new CompProperties_AbilityCECost Props => (CompProperties_AbilityCECost)props;

        private bool HasEnoughCursedEnergy
        {
            get
            {
                Gene_CursedEnergy gene_cursedEnergy = parent.pawn.GetCursedEnergy();
                if (gene_cursedEnergy == null || gene_cursedEnergy.Value < CastCost)
                {
                    return false;
                }

                return true;
            }
        }

        public override bool AICanTargetNow(LocalTargetInfo target)
        {
            return HasEnoughCursedEnergy;
        }

        public override void ApplyAbility(LocalTargetInfo target, LocalTargetInfo dest)
        {
            Gene_CursedEnergy gene_cursedEnergy = parent.pawn.GetCursedEnergy();
            if (gene_cursedEnergy != null)
            {
                gene_cursedEnergy.ConsumeCursedEnergy(CastCost);
            }
        }
    }

    public class CompProperties_AbilityCECost : CompProperties_CursedAbilityProps
    {
        public CompProperties_AbilityCECost()
        {
            compClass = typeof(CompAbilityEffect_CECost);
        }

    }
}