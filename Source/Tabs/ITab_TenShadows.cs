using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace JJK
{
    public class ITab_TenShadows : ITab
    {
        private Vector2 scrollPosition = Vector2.zero;
        private const float ROW_HEIGHT = 40f;
        private const float ICON_SIZE = 40f;
        private const float LABEL_WIDTH = 240f;
        private const float HEALTH_WIDTH = 100f;
        private const float BUTTON_WIDTH = 80f;
        private const float COLUMN_SPACING = 5f;

        public ITab_TenShadows()
        {
            this.labelKey = "TabTenShadows";
            this.tutorTag = "TenShadows";
            this.size = new Vector2(450f, 450f);
        }

        protected override void FillTab()
        {
            Rect rect = new Rect(0f, 0f, this.size.x, this.size.y).ContractedBy(10f);
            Rect viewRect = new Rect(0f, 0f, rect.width - 6f, 1000f);
            Pawn pawn = this.SelPawn;
            TenShadowGene tenShadowGene = pawn?.genes?.GetFirstGeneOfType<TenShadowGene>();

            Widgets.BeginScrollView(rect, ref scrollPosition, viewRect);
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(viewRect);

            if (pawn != null && tenShadowGene != null)
            {
                var allShikigami = tenShadowGene.Shikigami;
                listingStandard.Label($"Unlocked Shadows: {tenShadowGene.EarnedShadows.Count()}");
                listingStandard.GapLine();

                foreach (var shikigamiDef in allShikigami)
                {
                    DrawRow(pawn, tenShadowGene, shikigamiDef.Key, listingStandard);
                }
            }
            else
            {
                listingStandard.Label("No Ten Shadows Gene data available");
            }

            listingStandard.End();
            Widgets.EndScrollView();
        }

        private void DrawRow(Pawn pawn, TenShadowGene tenShadowGene, ShikigamiDef shikigamiDef, Listing_Standard listingStandard)
        {
            bool isUnlocked = tenShadowGene.HasUnlockedShikigami(shikigamiDef);
            bool isPermanentlyDead = tenShadowGene.IsShikigamiPermanentlyDead(shikigamiDef);

            Rect rowRect = listingStandard.GetRect(ROW_HEIGHT);
            if (!isUnlocked)
            {
                GUI.color = Color.grey;
            }
            else if (isPermanentlyDead)
            {
                GUI.color = Color.red;
            }

            var layout = new RowLayoutManager(rowRect);
            Rect iconRect = layout.NextRect(ICON_SIZE, COLUMN_SPACING);
            Rect labelRect = layout.NextRect(LABEL_WIDTH, COLUMN_SPACING);
            Rect healthRect = layout.NextRect(HEALTH_WIDTH, COLUMN_SPACING);
            //Rect summonButtonRect = layout.NextRect(BUTTON_WIDTH, COLUMN_SPACING);

            if (shikigamiDef.shikigami.race.uiIcon != null)
            {
                Widgets.DrawTextureFitted(iconRect, shikigamiDef.shikigami.race.uiIcon, 1f);
            }

            Widgets.Label(labelRect, $"{shikigamiDef.defName}");

            if (isUnlocked && !isPermanentlyDead)
            {
                var shikigamiData = tenShadowGene.GetShikigamiData(shikigamiDef);
                if (shikigamiData != null)
                {
                    float healthPercent = GetShikigamiHealthPercent(shikigamiData);
                    Widgets.FillableBar(healthRect, healthPercent);
                }
            }
            else
            {
                Widgets.FillableBar(healthRect, 0f);
            }

            //if (isUnlocked && !isPermanentlyDead)
            //{
            //    var activeSummons = tenShadowGene.GetActiveSummonsOfKind(shikigamiDef);
            //    bool isActive = activeSummons != null && activeSummons.Any();

            //    if (isActive)
            //    {
            //        if (Widgets.ButtonText(summonButtonRect, "Unsummon"))
            //        {
            //            tenShadowGene.UnsummonShikigami(shikigamiDef);
            //        }
            //    }
            //    else
            //    {
            //        if (Widgets.ButtonText(summonButtonRect, "Summon"))
            //        {
            //            if (tenShadowGene.CanSummonShikigamiKind(shikigamiDef))
            //            {
            //                tenShadowGene.GetOrGenerateShikigami(shikigamiDef, shikigamiDef.shikigami, SelPawn.Position, SelPawn.Map);
            //            }
            //        }
            //    }
            //}

            GUI.color = Color.white;
        }

        private float GetShikigamiHealthPercent(ShikigamiData shikigamiData)
        {
            var allPawns = new List<Pawn>();
            if (shikigamiData.StoredPawns != null)
            {
                allPawns.AddRange(shikigamiData.StoredPawns);
            }
            if (shikigamiData.ActiveShadows != null)
            {
                allPawns.AddRange(shikigamiData.ActiveShadows);
            }

            if (!allPawns.Any())
                return 1f;

            return allPawns.Average(p => p.health.summaryHealth.SummaryHealthPercent);
        }


        public override bool IsVisible => base.IsVisible && this.SelPawn != null && this.SelPawn.genes?.GetFirstGeneOfType<TenShadowGene>() != null;
    }
}
