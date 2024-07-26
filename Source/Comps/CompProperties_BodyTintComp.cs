using UnityEngine;
using Verse;

namespace JJK
{
    public class CompProperties_BodyTintComp : CompProperties
    {
        public Color Color = new Color(0.2f, 0.7f, 0.2f, 1f);

        public CompProperties_BodyTintComp()
        {
            this.compClass = typeof(BodyTintComp);
        }
    }


    public class BodyTintComp : ThingComp
    {
        new CompProperties_BodyTintComp Props => (CompProperties_BodyTintComp)props;
        private Color originalColor;
        private bool colorChanged = false;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            if (parent is Pawn pawn)
            {
                ApplyColor(pawn);
            }
        }

        public override void PostDeSpawn(Map map)
        {
            if (parent is Pawn pawn)
            {
                RestoreOriginalColor(pawn);
            }

            base.PostDeSpawn(map);
        }


        private void ApplyColor(Pawn pawn)
        {
            originalColor = pawn.Drawer.renderer.BodyGraphic.color;
            pawn.Drawer.renderer.BodyGraphic.color = Props.Color;
            pawn.Drawer.renderer.SetAllGraphicsDirty();
            colorChanged = true;
        }

        private void RestoreOriginalColor(Pawn pawn)
        {
            pawn.Drawer.renderer.BodyGraphic.color = originalColor;
            pawn.Drawer.renderer.SetAllGraphicsDirty();
            colorChanged = false;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Values.Look(ref originalColor, "BodyTintComp_bodyTintOriginalColor");
        }

    }
}


