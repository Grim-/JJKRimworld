using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace JJK
{
    public class CompProperties_SummonCreature : CompProperties_CursedAbilityProps
    {
        public float EnergyCost = 25f;

        public CompProperties_SummonCreature()
        {
            compClass = typeof(CompAbilityEffect_SummonCreature);
        }
    }
    public class CompAbilityEffect_SummonCreature : BaseCursedEnergyAbility
    {
        public new CompProperties_SummonCreature Props => (CompProperties_SummonCreature)props;

        public override void ApplyAbility(LocalTargetInfo target, LocalTargetInfo dest)
        {

            var manager = Find.World.GetComponent<AbsorbedCreatureManager>();

            if (manager != null)
            {
                List<FloatMenuOption> SummonOptions = new List<FloatMenuOption>();
                var groupedOptions = manager.GetAbsorbedCreatures(parent.pawn);

                foreach (var group in groupedOptions)
                {
                    SummonOptions.Add(new FloatMenuOption(group.def.defName, () => SummonCreature(parent.pawn, group)));
                }

                Find.WindowStack.Add(new FloatMenu(SummonOptions));
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            var manager = Find.World.GetComponent<AbsorbedCreatureManager>();
            if (manager != null)
            {
                var absorbedCreatures = manager.GetAbsorbedCreatures(parent.pawn);
                if (absorbedCreatures.Count > 0)
                {
                    var options = new List<Gizmo_MultiOptions.Option>();
                    foreach (var creature in absorbedCreatures)
                    {
                        options.Add(new Gizmo_MultiOptions.Option(
                            creature.def.defName,
                            creature.def.uiIcon,
                            () =>
                            {
                                if (manager.IsActiveAbsorbedCreature(parent.pawn, creature))
                                {
                                    UnSummonCreature(parent.pawn, creature);
                                }
                                else
                                    SummonCreature(parent.pawn, creature);
                            },
                            () =>
                            {
                                if (manager.HasAbsorbedCreatue(parent.pawn, creature))
                                {
                                    manager.DeleteAbsorbedCreature(parent.pawn, creature);
                                }
                            }
                        ));
                    }
                    yield return new Gizmo_MultiOptions(options);
                }
            }
        }

        public void SummonCreature(Pawn caster, Pawn creature)
        {
            var manager = Find.World.GetComponent<AbsorbedCreatureManager>();


            if (manager != null)
            {
                if (manager.SummonCreature(caster, creature))
                {
                    Messages.Message($"{caster.LabelShort} has summoned {creature.LabelShort}.", MessageTypeDefOf.PositiveEvent);
                }
                
            }          
        }

        public void UnSummonCreature(Pawn caster, Pawn creature)
        {
            var manager = Find.World.GetComponent<AbsorbedCreatureManager>();

            if (manager != null)
            {
                if (manager.UnsummonCreature(caster, creature))
                {
                    Messages.Message($"{caster.LabelShort} has unsummoned {creature.LabelShort}.", MessageTypeDefOf.PositiveEvent);
                }

            }
        }
    }

    public class Gizmo_MultiOptions : Gizmo
    {
        private List<Option> options;
        private const float ButtonSize = 24f;
        private const int MaxButtonsPerRow = 8;
        private const float Padding = 4f;

        public class Option
        {
            public string Label;
            public Texture2D Icon;
            public Action OnSelected;
            public Action OnRightClick;
            public Action OnShiftLeftClick;

            public Option(string label, Texture2D icon, Action onSelected, Action onRightClick = null, Action onShiftLeftClick = null)
            {
                Label = label;
                Icon = icon;
                OnSelected = onSelected;
                OnRightClick = onRightClick;
                OnShiftLeftClick = onShiftLeftClick;
            }
        }

        public Gizmo_MultiOptions(List<Option> options)
        {
            this.options = options;
        }

        public override float GetWidth(float maxWidth)
        {
            int buttonsPerRow = Mathf.Min(options.Count, MaxButtonsPerRow);
            return Mathf.Min(maxWidth, buttonsPerRow * ButtonSize + Padding * 2);
        }

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        {
            float width = GetWidth(maxWidth);
            float height = ButtonSize * Mathf.Ceil((float)options.Count / MaxButtonsPerRow) + Padding * 2;
            Rect mainRect = new Rect(topLeft.x, topLeft.y, width, height);
            Widgets.DrawWindowBackground(mainRect);
            Text.Font = GameFont.Tiny;
            bool interacted = false;

            for (int i = 0; i < options.Count; i++)
            {
                int row = i / MaxButtonsPerRow;
                int col = i % MaxButtonsPerRow;
                Rect buttonRect = new Rect(
                    mainRect.x + col * ButtonSize + Padding,
                    mainRect.y + row * ButtonSize + Padding,
                    ButtonSize,
                    ButtonSize
                );

                if (Mouse.IsOver(buttonRect))
                {
                    if (Event.current.type == EventType.MouseDown)
                    {
                        if (Event.current.button == 0) // Left click
                        {
                            if (Event.current.shift)
                            {
                                if (options[i].OnShiftLeftClick != null)
                                {
                                    SoundDefOf.Tick_High.PlayOneShotOnCamera();
                                    options[i].OnShiftLeftClick();
                                    interacted = true;
                                    Event.current.Use();
                                }
                            }
                            else
                            {
                                SoundDefOf.Tick_High.PlayOneShotOnCamera();
                                options[i].OnSelected();
                                interacted = true;
                                Event.current.Use();
                            }
                        }
                        else if (Event.current.button == 1) // Right click
                        {
                            if (options[i].OnRightClick != null)
                            {
                                SoundDefOf.Tick_High.PlayOneShotOnCamera();
                                options[i].OnRightClick();
                                interacted = true;
                                Event.current.Use();
                            }
                        }
                    }
                }

                Widgets.DrawTextureFitted(buttonRect, options[i].Icon, 1f);
                TooltipHandler.TipRegion(buttonRect, options[i].Label);
                Rect labelRect = new Rect(buttonRect.x, buttonRect.yMax, ButtonSize, 18f);
                Widgets.Label(labelRect, options[i].Label.Truncate(8));
            }

            if (disabled)
            {
                Widgets.DrawTextureFitted(mainRect, Widgets.CheckboxOffTex, 1f);
            }

            return interacted ? new GizmoResult(GizmoState.Interacted) : new GizmoResult(GizmoState.Clear);
        }
    }
}