using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace JJK
{
    internal class CompAbilityEffect_CECost : BaseCursedEnergyAbility
    {
        public new CompProperties_AbilityCECost Props => (CompProperties_AbilityCECost)props;

        private bool HasEnoughHemogen
        {
            get
            {
                Gene_CursedEnergy gene_cursedEnergy = parent.pawn.GetCursedEnergy();
                if (gene_cursedEnergy == null || gene_cursedEnergy.Value < GetCost())
                {
                    return false;
                }

                return true;
            }
        }

        public override bool AICanTargetNow(LocalTargetInfo target)
        {
            return HasEnoughHemogen;
        }

        public override void ApplyAbility(LocalTargetInfo target, LocalTargetInfo dest)
        {
            Gene_CursedEnergy gene_cursedEnergy = parent.pawn.GetCursedEnergy();
            if (gene_cursedEnergy != null)
            {
                gene_cursedEnergy.ConsumeCursedEnergy(parent.pawn, GetCost());
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