using RimWorld;
using System.Linq;
using UnityEngine;
using Verse;

namespace JJK
{
    public class ITab_CursedSpiritManipulator : ITab
    {
        private Vector2 scrollPosition = Vector2.zero;
        private const float ROW_HEIGHT = 30f;

        public ITab_CursedSpiritManipulator()
        {
            this.labelKey = "TabCursedSpirit";
            this.tutorTag = "CursedSpirit";
            this.size = new Vector2(400f, 300f);
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
                    Rect rowRect = listingStandard.GetRect(ROW_HEIGHT);
                    Rect iconRect = new Rect(rowRect.x, rowRect.y, ROW_HEIGHT, ROW_HEIGHT);
                    Rect labelRect = new Rect(rowRect.x + ROW_HEIGHT + 10f, rowRect.y, rowRect.width - ROW_HEIGHT - 70f, ROW_HEIGHT);
                    Rect buttonRect = new Rect(rowRect.xMax - 60f, rowRect.y, 60f, ROW_HEIGHT);

                    Widgets.DrawTextureFitted(iconRect, group.Key.race.uiIcon, 1f);
                    Widgets.Label(labelRect, $"{group.Key.label}");
                    if (Widgets.ButtonText(buttonRect, "Remove"))
                    {
                        cursedSpiritManipulator.DeleteAbsorbedCreature(group.Key);
                    }
                }
            }
            else
            {
                listingStandard.Label("No Cursed Spirit Manipulator data available");
            }

            listingStandard.End();
            Widgets.EndScrollView();
        }

        public override bool IsVisible => base.IsVisible && this.SelPawn != null && this.SelPawn.IsCursedSpiritManipulator();
    }
}