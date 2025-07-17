using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;



namespace JJK
{
    [StaticConstructorOnStartup]
    internal class GeneGizmo_ResourceCursedEnergy : GeneGizmo_Resource
    {
        private static readonly Texture2D IchorCostTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.78f, 0.72f, 0.66f));

        private const float TotalPulsateTime = 0.85f;

        private List<Pair<IGeneResourceDrain, float>> tmpDrainGenes = new List<Pair<IGeneResourceDrain, float>>();

        protected bool _IsDraggingBar = false;
        protected override bool DraggingBar { get => _IsDraggingBar; set => _IsDraggingBar = value; }

        public GeneGizmo_ResourceCursedEnergy(Gene_Resource gene, List<IGeneResourceDrain> drainGenes, Color barColor, Color barhighlightColor)
        : base(gene, drainGenes, barColor, barhighlightColor)
    {
    }

    public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
    {
            GizmoResult result = base.GizmoOnGUI(topLeft, maxWidth, parms);
            float num = Mathf.Repeat(Time.time, 0.85f);
            if (gene is Gene_CursedEnergy cursedEnergy)
            {
                Target = num;
            }
            return result;
    }

        protected override void DrawHeader(Rect headerRect, ref bool mouseOverElement)
        {
            Gene_CursedEnergy CEGene;
            if ((gene.pawn.IsColonistPlayerControlled || gene.pawn.IsPrisonerOfColony) && gene is Gene_CursedEnergy cursedEnergy)
            {
                headerRect.xMax -= 24f;
                Rect rect = new Rect(headerRect.xMax, headerRect.y, 24f, 24f);
                Widgets.DefIcon(rect, FleckDefOf.Heart);
                if (Mouse.IsOver(rect))
                {
                    Widgets.DrawHighlight(rect);
                }
            }
            base.DrawHeader(headerRect, ref mouseOverElement);
        }

        protected override string GetTooltip()
        {
            tmpDrainGenes.Clear();
            string text = $"{gene.ResourceLabel.CapitalizeFirst().Colorize(ColoredText.TipSectionTitleColor)}: {gene.ValueForDisplay} / {gene.MaxForDisplay}\n";
            int RegenSpeed = (int)this.gene.pawn.GetStatValue(JJKDefOf.JJK_CursedEnergyRegenSpeed);
            float RegenAmount = this.gene.pawn.GetStatValue(JJKDefOf.JJK_CursedEnergyRegen);
            string Regen = $" Regenerates {RegenAmount} every {GenDate.ToStringTicksToPeriod(RegenSpeed)}";

            if (!gene.def.resourceDescription.NullOrEmpty())
            {
                text = text + "\n\n" + gene.def.resourceDescription.Formatted(gene.pawn.Named("PAWN")).Resolve();
            }
            return text + Regen;
        }
    }
}