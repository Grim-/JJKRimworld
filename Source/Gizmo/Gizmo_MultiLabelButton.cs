using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace JJK
{
    public class Gizmo_MultiLabelButton : Gizmo_MultiOptions
    {
        protected override int ButtonWidth => 80; 
        protected override int ButtonHeight => 36; 
        protected override int MaxButtonsPerRow => 4; 
        protected override float Padding => 5f;

        public Gizmo_MultiLabelButton(List<Gizmo_MultiOption> options) : base(options) { }

        public override float GetWidth(float maxWidth)
        {
            int buttonsPerRow = Mathf.Min(options.Count, MaxButtonsPerRow);
            float totalButtonWidth = buttonsPerRow * ButtonWidth;
            float totalPaddingWidth = Padding * (buttonsPerRow + 1);
            return Mathf.Min(maxWidth, totalButtonWidth + totalPaddingWidth);
        }

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        {
            float width = GetWidth(maxWidth);
            int rows = Mathf.CeilToInt((float)options.Count / MaxButtonsPerRow);
            float height = (ButtonHeight * rows) + (Padding * (rows + 1));

            Rect mainRect = new Rect(topLeft.x, topLeft.y, width, height);
            Widgets.DrawWindowBackground(mainRect);
            Text.Font = GameFont.Tiny;
            bool interacted = false;

            for (int i = 0; i < options.Count; i++)
            {
                int row = i / MaxButtonsPerRow;
                int col = i % MaxButtonsPerRow;
                Rect buttonRect = new Rect(
                    mainRect.x + (col * (ButtonWidth + Padding)) + Padding,
                    mainRect.y + (row * (ButtonHeight + Padding)) + Padding,
                    ButtonWidth,
                    ButtonHeight
                );

                if (Mouse.IsOver(buttonRect))
                {
                    interacted |= HandleInput(options[i], Event.current);
                }

                DrawListItem(buttonRect, options[i]);
            }

            if (disabled)
            {
                Widgets.DrawTextureFitted(mainRect, Widgets.CheckboxOffTex, 1f);
            }

            return interacted ? new GizmoResult(GizmoState.Interacted) : new GizmoResult(GizmoState.Clear);
        }

        protected override void DrawListItem(Rect buttonRect, Gizmo_MultiOption option)
        {
            Widgets.DrawBox(buttonRect);
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(buttonRect, option.Label);
            Text.Anchor = TextAnchor.UpperLeft;
            TooltipHandler.TipRegion(buttonRect, option.Label);
        }
    }
}