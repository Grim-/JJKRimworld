using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace JJK
{
    public class Gizmo_MultiImageButton : Gizmo_MultiOptions
    {
        public Gizmo_MultiImageButton(List<Gizmo_MultiOption> options) : base(options)
        {

        }

        protected override void DrawListItem(Rect ButtonRect, Gizmo_MultiOption Option)
        {
            Widgets.DrawTextureFitted(ButtonRect, Option.Icon, 1f);
            TooltipHandler.TipRegion(ButtonRect, Option.Label);
            Rect labelRect = new Rect(ButtonRect.x, ButtonRect.yMax, ButtonWidth, 18f);
            Widgets.Label(labelRect, Option.Label.Truncate(8));
        }
    }
}