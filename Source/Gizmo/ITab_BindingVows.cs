using RimWorld;
using UnityEngine;
using Verse;

namespace JJK
{
    public class ITab_BindingVows : ITab
    {
        private Vector2 scrollPosition = Vector2.zero;
        private const float ROW_HEIGHT = 30f;

        public ITab_BindingVows()
        {
            this.labelKey = "TabBindingVows";
            this.tutorTag = "BindingVows";
            this.size = new Vector2(400f, 300f);
        }

        protected override void FillTab()
        {
            Rect rect = new Rect(0f, 0f, this.size.x, this.size.y).ContractedBy(10f);
            Rect viewRect = new Rect(0f, 0f, rect.width - 16f, 1000f);
            Pawn pawn = (Pawn)this.SelPawn;

            Widgets.BeginScrollView(rect, ref scrollPosition, viewRect);
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(viewRect);

            listingStandard.End();
            Widgets.EndScrollView();
        }

        public override bool IsVisible => base.IsVisible && this.SelPawn != null && this.SelPawn.GetCursedEnergy() != null;
    }
}