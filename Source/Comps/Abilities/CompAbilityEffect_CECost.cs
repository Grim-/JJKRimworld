using RimWorld;
using System;
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
                gene_cursedEnergy.ConsumeCursedEnergy(GetCost());
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
    public class Command_ToggleAbility : Command_Ability
    {
        private bool isActive;
        private System.Action toggleAction;

        public Command_ToggleAbility(Pawn pawn, Ability ability, bool initialState, System.Action onToggle)
            : base(ability, pawn)
        {

            this.isActive = initialState;
            this.toggleAction = onToggle;
        }

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        {
            GizmoResult result = base.GizmoOnGUI(topLeft, maxWidth, parms);

            Rect rect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
            Rect checkboxRect = new Rect(rect.x + 5f, rect.y + 5f, 24f, 24f);

            Widgets.Checkbox(checkboxRect.position, ref isActive, 24f, disabled);
            if (Widgets.ButtonInvisible(checkboxRect))
            {
                if (!disabled)
                {
                    isActive = !isActive;
                    toggleAction();
                }
            }

            return result;
        }
    }
}