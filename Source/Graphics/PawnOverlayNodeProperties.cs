using RimWorld;
using UnityEngine;
using Verse;

namespace JJK
{
    public class PawnOverlayNodeProperties : PawnRenderNodeProperties
    {
        public Color overlayColor = Color.white;
        public float overlayAlpha = 1f;
        public Vector3 offset = Vector3.zero;
        public float layerOffset = 0.1f;
        public GraphicData graphicData;

        public PawnOverlayNodeProperties()
        {
            this.nodeClass = typeof(PawnOverlayNode);
            this.workerClass = typeof(PawnOverlayNodeWorker);
        }
    }

    public class PawnOverlayNode : PawnRenderNode
    {
        public new PawnOverlayNodeProperties Props => (PawnOverlayNodeProperties)props;

        public PawnOverlayNode(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree)
        {
        }

        public override Graphic GraphicFor(Pawn pawn)
        {
            return GraphicDatabase.Get<Graphic_Multi>(Props.graphicData.texPath, base.ShaderFor(pawn), Props.graphicData.drawSize, Props.graphicData.color, Props.graphicData.colorTwo);
        }
    }
}
