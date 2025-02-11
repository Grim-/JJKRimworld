using RimWorld;
using UnityEngine;
using Verse;

namespace JJK
{
    public class Command_ToggleAbility : Command_Ability
    {
        private Ability_Toggleable toggleability;
        private System.Action toggleAction;

        public Command_ToggleAbility(Pawn pawn, Ability_Toggleable ability, System.Action onToggle)
            : base(ability, pawn)
        {
            this.toggleability = ability;
            this.toggleAction = onToggle;
        }

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        {
            GizmoResult result = base.GizmoOnGUI(topLeft, maxWidth, parms);

            Rect rect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
            Rect checkboxRect = new Rect(rect.x + 5f, rect.y + 5f, 24f, 24f);

            bool isChecked = this.toggleability.IsActive;
            Widgets.Checkbox(checkboxRect.position, ref isChecked, 24f, disabled);

            if (Widgets.ButtonInvisible(checkboxRect))
            {
                if (!disabled)
                {
                    toggleAction?.Invoke();
                }
            }

            return result;
        }
    }
}