using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace JJK
{
    public class PawnOverlayNodeWorker : PawnRenderNodeWorker
    {
        private float animationProgress = 0f;
        private const float AnimationSpeed = 1f / 3000f; // Adjust based on your desired speed

        public override void AppendDrawRequests(PawnRenderNode node, PawnDrawParms parms, List<PawnGraphicDrawRequest> requests)
        {
            PawnOverlayNode overlayNode = node as PawnOverlayNode;
            if (overlayNode == null || overlayNode.Graphic == null) return;

            Mesh mesh = node.GetMesh(parms);
            if (mesh == null) return;

            Material material = overlayNode.GraphicFor(parms.pawn).MatAt(parms.facing);
            if (material == null) return;

            Color overlayColor = overlayNode.Props.overlayColor;
            overlayColor.a = overlayNode.Props.overlayAlpha;
            material.SetColor("_ColorOne", overlayColor);
            material.color = overlayColor;

            Vector3 drawLoc;
            Vector3 pivot;
            Quaternion quat;
            Vector3 scale;

            Vector3 offset = this.OffsetFor(node, parms, out pivot);
            node.GetTransform(parms, out drawLoc, out _, out quat, out scale);
            drawLoc += offset;

            if (overlayNode.Props.graphicData != null)
            {
                scale = new Vector3(overlayNode.Props.graphicData.drawSize.x, 1f, overlayNode.Props.graphicData.drawSize.y);
            }

            PawnGraphicDrawRequest request = new PawnGraphicDrawRequest(node, mesh, material);
            request.preDrawnComputedMatrix = Matrix4x4.TRS(drawLoc, quat, scale);
            requests.Add(request);
        }

        public override Vector3 OffsetFor(PawnRenderNode node, PawnDrawParms parms, out Vector3 pivot)
        {
            Vector3 baseOffset = base.OffsetFor(node, parms, out pivot);

            if (node is PawnOverlayNode overlayNode)
            {
                Vector3 leftRightOffset = Vector3.zero;

                if (parms.facing == Rot4.East)
                {
                    leftRightOffset = overlayNode.Props.eastOffset;
                }
                else if(parms.facing == Rot4.West)
                {
                    leftRightOffset = overlayNode.Props.westOffset;
                }
                Vector3 customOffset = overlayNode.Props.offset + leftRightOffset;
                return baseOffset + customOffset;
            }

            return baseOffset;
        }
        protected override Vector3 PivotFor(PawnRenderNode node, PawnDrawParms parms)
        {
            Vector3 basePivot = base.PivotFor(node, parms);
            if (node is PawnOverlayNode overlayNode)
            {
                Vector3 customPivotAdjustment = Vector3.zero;
                return basePivot + customPivotAdjustment;
            }

            return basePivot;
        }

        public override Vector3 ScaleFor(PawnRenderNode node, PawnDrawParms parms)
        {           
            if (node is PawnOverlayNode overlayNode && overlayNode.Props.graphicData != null)
            {
              return new Vector3(overlayNode.Props.graphicData.drawSize.x, 1f, overlayNode.Props.graphicData.drawSize.y);
            }

            return base.ScaleFor(node, parms);
        }

        public override float LayerFor(PawnRenderNode node, PawnDrawParms parms)
        {
            if (node is PawnOverlayNode overlayNode)
            {
                float baseLayer = base.LayerFor(node, parms);

                if (parms.facing == Rot4.North)
                {
                    return baseLayer + 1000f; 
                }
                else
                {
                    return baseLayer + overlayNode.Props.layerOffset;
                }
            }
            return base.LayerFor(node, parms);
        }

        public override Quaternion RotationFor(PawnRenderNode node, PawnDrawParms parms)
        {
            return base.RotationFor(node, parms);
        }
    }
}
