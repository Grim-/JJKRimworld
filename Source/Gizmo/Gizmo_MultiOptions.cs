using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace JJK
{
    public abstract class Gizmo_MultiOptions : Gizmo
    {
        protected List<Gizmo_MultiOption> options;

        protected virtual int ButtonWidth => 48;
        protected virtual int ButtonHeight => 48;
        protected virtual int MaxButtonsPerRow => 8;
        protected virtual float Padding => 4f;



        public Gizmo_MultiOptions(List<Gizmo_MultiOption> options)
        {
            this.options = options;
        }

        public override float GetWidth(float maxWidth)
        {
            int buttonsPerRow = Mathf.Min(options.Count, MaxButtonsPerRow);
            float totalButtonWidth = buttonsPerRow * ButtonWidth;
            float totalPaddingWidth = Padding * 2; // Padding on left and right
            return Mathf.Min(maxWidth, totalButtonWidth + totalPaddingWidth);
        }

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        {
            float width = GetWidth(maxWidth);
            int rows = Mathf.CeilToInt((float)options.Count / MaxButtonsPerRow);
            float height = ButtonHeight * rows;

            Rect mainRect = new Rect(topLeft.x, topLeft.y, width, height);

            Widgets.DrawWindowBackground(mainRect);
            Text.Font = GameFont.Tiny;
            bool interacted = false;

            for (int i = 0; i < options.Count; i++)
            {
                int row = i / MaxButtonsPerRow;
                int col = i % MaxButtonsPerRow;
                Rect buttonRect = new Rect(
                    mainRect.x + col * ButtonWidth + Padding,
                    mainRect.y + row * ButtonHeight,
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

        protected virtual bool HandleInput(Gizmo_MultiOption option, Event ev)
        {
            if (ev.type == EventType.MouseDown)
            {
                if (ev.button == 0)
                {
                    if (ev.shift && option.OnShiftLeftClick != null)
                    {
                        OnOptionShiftClick(option);
                        return true;
                    }
                    else
                    {
                        OnOptionClick(option);
                        return true;
                    }
                }
                else if (ev.button == 1 && option.OnRightClick != null)
                {
                    OnOptionRightClick(option);
                    return true;
                }
            }
            return false;
        }



        private void OnOptionShiftClick(Gizmo_MultiOption Option)
        {
            SoundDefOf.Tick_High.PlayOneShotOnCamera();
            Option.OnShiftLeftClick();
            Event.current.Use();
        }

        private void OnOptionClick(Gizmo_MultiOption Option)
        {
            SoundDefOf.Tick_High.PlayOneShotOnCamera();
            Option.OnSelected();
            Event.current.Use();
        }

        private void OnOptionRightClick(Gizmo_MultiOption Option)
        {
            SoundDefOf.Tick_High.PlayOneShotOnCamera();
            Option.OnRightClick();
            Event.current.Use();
        }

        protected abstract void DrawListItem(Rect ButtonRect, Gizmo_MultiOption Option);
    }
    public class Gizmo_MultiOption
    {
        public string Label;
        public Texture2D Icon;
        public Action OnSelected;
        public Action OnRightClick;
        public Action OnShiftLeftClick;
        public float DisplayAlpha = 1;

        public Gizmo_MultiOption(string label, Texture2D icon, Action onSelected, Action onRightClick = null, Action onShiftLeftClick = null, float Alpha = 1)
        {
            Label = label;
            Icon = icon;
            OnSelected = onSelected;
            OnRightClick = onRightClick;
            OnShiftLeftClick = onShiftLeftClick;
            DisplayAlpha = Alpha;
        }
    }
}