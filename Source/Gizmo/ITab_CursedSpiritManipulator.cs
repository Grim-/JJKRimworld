using RimWorld;
using System.Linq;
using UnityEngine;
using Verse;

namespace JJK
{
    public class ITab_CursedSpiritManipulator : ITab
    {
        private Vector2 scrollPosition = Vector2.zero;
        private const float ROW_HEIGHT = 40f;
        private const float ICON_SIZE = 30f;
        private const float LABEL_WIDTH = 100f;
        private const float HEALTH_WIDTH = 80f;
        private const float BUTTON_WIDTH = 80f;
        private const float COLUMN_SPACING = 5f;
        private const float SPACING = 5f;
        private int CurrentTabIndex = 0;

        public ITab_CursedSpiritManipulator()
        {
            this.labelKey = "TabCursedSpirit";
            this.tutorTag = "CursedSpirit";
            this.size = new Vector2(450f, 450f);
        }

        protected override void FillTab()
        {
            Rect rect = new Rect(0f, 0f, this.size.x, this.size.y).ContractedBy(10f);
            Rect viewRect = new Rect(0f, 0f, rect.width - 16f, 1000f);
            Pawn pawn = (Pawn)this.SelPawn;
            Hediff_CursedSpiritManipulator cursedSpiritManipulator = pawn.health.hediffSet.GetFirstHediffOfDef(JJKDefOf.JJK_CursedSpiritManipulator) as Hediff_CursedSpiritManipulator;

            Widgets.BeginScrollView(rect, ref scrollPosition, viewRect);
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(viewRect);

            if (pawn != null && cursedSpiritManipulator != null)
            {
                var absorbedCreatures = cursedSpiritManipulator.GetAbsorbedCreatures();
                listingStandard.Label($"Total Absorbed Creatures: {absorbedCreatures.Count}");
                listingStandard.GapLine();

                var groupedCreatures = absorbedCreatures
                    .GroupBy(c => c)
                    .OrderByDescending(g => g.Count());

                foreach (var group in groupedCreatures)
                {
                    DrawRow(pawn, cursedSpiritManipulator, group.Key, listingStandard);
                }
            }
            else
            {
                listingStandard.Label("No Cursed Spirit Manipulator data available");
            }

            listingStandard.End();
            Widgets.EndScrollView();
        }

        private void DrawRow(Pawn pawn, Hediff_CursedSpiritManipulator cursedSpiritManipulator, Pawn absorbedCreature, Listing_Standard listingStandard)
        {
            Rect rowRect = listingStandard.GetRect(ROW_HEIGHT);
            var layout = new RowLayoutManager(rowRect);
            Rect iconRect = layout.NextRect(ICON_SIZE, COLUMN_SPACING);
            Rect labelRect = layout.NextRect(LABEL_WIDTH, COLUMN_SPACING);
            Rect healthRect = layout.NextRect(HEALTH_WIDTH, COLUMN_SPACING);
            Rect summonButtonRect = layout.NextRect(BUTTON_WIDTH, COLUMN_SPACING);
            Rect removeButtonRect = layout.NextRect(BUTTON_WIDTH);

            Widgets.DrawTextureFitted(iconRect, absorbedCreature.kindDef.race.uiIcon, 1f);
            Widgets.HyperlinkWithIcon(iconRect, new Dialog_InfoCard.Hyperlink(absorbedCreature.def));
            Widgets.Label(labelRect, $"{absorbedCreature.Label}");
            Widgets.FillableBar(healthRect, absorbedCreature.health.summaryHealth.SummaryHealthPercent);

            if (cursedSpiritManipulator.IsCreatureActive(absorbedCreature))
            {
                if (Widgets.ButtonText(summonButtonRect, "UnSummon"))
                {
                    cursedSpiritManipulator.UnsummonCreature(absorbedCreature);
                }
            }
            else
            {
                if (Widgets.ButtonText(summonButtonRect, "Summon"))
                {
                    cursedSpiritManipulator.SummonCreature(absorbedCreature);
                }
            }

            if (Widgets.ButtonText(removeButtonRect, "Remove"))
            {
                cursedSpiritManipulator.DeleteAbsorbedCreature(absorbedCreature);
            }
        }

        public override bool IsVisible => base.IsVisible && this.SelPawn != null && this.SelPawn.IsCursedSpiritManipulator();
    }

    public class RowLayoutManager
    {
        private float currentX;
        private readonly float rowY;
        private readonly float rowHeight;
        private readonly float rowWidth;
        private readonly float horizontalPadding;
        private readonly float verticalPadding;

        public RowLayoutManager(Rect rowRect, float horizontalPadding = 10f, float verticalPadding = 5f)
        {
            this.rowWidth = rowRect.width;
            this.rowHeight = rowRect.height - (2 * verticalPadding);
            this.horizontalPadding = horizontalPadding;
            this.verticalPadding = verticalPadding;
            this.currentX = rowRect.x + horizontalPadding;
            this.rowY = rowRect.y + verticalPadding;
        }

        public Rect NextRect(float width, float? spacing = null)
        {
            if (currentX + width > rowWidth - horizontalPadding)
            {
                //prevent overflow
                width = rowWidth - horizontalPadding - currentX;
            }
            Rect rect = new Rect(currentX, rowY, width, rowHeight);
            currentX += width + (spacing ?? 0);
            return rect;
        }

        public float RemainingWidth => rowWidth - currentX - horizontalPadding;
    }
}