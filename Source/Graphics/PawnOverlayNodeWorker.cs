using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace JJK
{
    public class PawnOverlayNodeWorker : PawnRenderNodeWorker
    {     
        public override void AppendDrawRequests(PawnRenderNode node, PawnDrawParms parms, List<PawnGraphicDrawRequest> requests)
        {
            base.AppendDrawRequests(node, parms, requests);

            PawnOverlayNode overlayNode = node as PawnOverlayNode;
            if (overlayNode == null || overlayNode.Graphic == null) return;

            Material material = overlayNode.GraphicFor(parms.pawn).MatAt(parms.facing);
            if (material == null) return;

            // Apply overlay color and alpha
            material = new Material(material);
            Color overlayColor = overlayNode.Props.overlayColor;
            overlayColor.a = overlayNode.Props.overlayAlpha;
            material.color = overlayColor;

            Mesh mesh = overlayNode.Graphic.MeshAt(parms.facing);
            if (mesh == null) return;

            Vector3 drawLoc;
            Vector3 pivot;
            Quaternion quat;
            Vector3 scale;
            node.GetTransform(parms, out drawLoc, out pivot, out quat, out scale);

            // Apply custom scale if specified in graphicData
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
                return baseOffset + overlayNode.Props.offset;
            }
            return baseOffset;
        }

        public override float LayerFor(PawnRenderNode node, PawnDrawParms parms)
        {
            if (node is PawnOverlayNode overlayNode)
            {
                return base.LayerFor(node, parms) + overlayNode.Props.layerOffset;
            }
            return base.LayerFor(node, parms);
        }
    }

    //public class PawnOverlayNodeProperties : PawnRenderNodeProperties
    //{
    //    public Color overlayColor = Color.white;
    //    public float overlayAlpha = 1f;
    //    public Vector3 offset = Vector3.zero;
    //    public float layerOffset = 0.1f;
    //    public float drawSize = 1.5f;
    //    public string texturePath = "Things/Pawn/Animal/Warg/Warg";

    //    public PawnOverlayNodeProperties()
    //    {
    //        this.nodeClass = typeof(PawnOverlayNode);
    //        this.workerClass = typeof(PawnOverlayNodeWorker);
    //    }
    //}

    //public class PawnOverlayNode : PawnRenderNode
    //{
    //    public new PawnOverlayNodeProperties Props => (PawnOverlayNodeProperties)props;
    //    private Graphic_Multi cachedGraphic;

    //    public PawnOverlayNode(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree)
    //    {
    //    }

    //    public Graphic_Multi Graphic
    //    {
    //        get
    //        {
    //            if (cachedGraphic == null)
    //            {
    //                cachedGraphic = GraphicDatabase.Get<Graphic_Multi>(Props.texturePath, ShaderDatabase.Cutout, Vector2.one * Props.drawSize, Props.overlayColor) as Graphic_Multi;
    //                Log.Message($"Warg overlay graphic created: {cachedGraphic != null}");
    //            }
    //            return cachedGraphic;
    //        }
    //    }
    //}

    //public class PawnOverlayNodeWorker : PawnRenderNodeWorker
    //{
    //    public override void AppendDrawRequests(PawnRenderNode node, PawnDrawParms parms, List<PawnGraphicDrawRequest> requests)
    //    {
    //        base.AppendDrawRequests(node, parms, requests);

    //        PawnOverlayNode overlayNode = node as PawnOverlayNode;
    //        if (overlayNode == null) return;

    //        Graphic_Multi graphic = overlayNode.Graphic;
    //        if (graphic == null) return;

    //        Material material = graphic.MatAt(parms.facing);
    //        if (material == null) return;

    //        // Apply overlay alpha
    //        material = new Material(material);
    //        Color materialColor = material.color;
    //        materialColor.a = overlayNode.Props.overlayAlpha;
    //        material.color = materialColor;

    //        Mesh mesh = graphic.MeshAt(parms.facing);
    //        if (mesh == null) return;

    //        Vector3 drawLoc;
    //        Vector3 pivot;
    //        Quaternion quat;
    //        Vector3 scale;
    //        node.GetTransform(parms, out drawLoc, out pivot, out quat, out scale);

    //        PawnGraphicDrawRequest request = new PawnGraphicDrawRequest(node, mesh, material);
    //        request.preDrawnComputedMatrix = Matrix4x4.TRS(drawLoc, quat, scale * overlayNode.Props.drawSize);
    //        requests.Add(request);

    //        Log.Message($"Warg overlay draw request added for {overlayNode.tree.pawn.Label} at position {drawLoc}");
    //    }

    //    public override Vector3 OffsetFor(PawnRenderNode node, PawnDrawParms parms, out Vector3 pivot)
    //    {
    //        Vector3 baseOffset = base.OffsetFor(node, parms, out pivot);
    //        if (node is PawnOverlayNode overlayNode)
    //        {
    //            return baseOffset + overlayNode.Props.offset;
    //        }
    //        return baseOffset;
    //    }

    //    public override float LayerFor(PawnRenderNode node, PawnDrawParms parms)
    //    {
    //        if (node is PawnOverlayNode overlayNode)
    //        {
    //            return base.LayerFor(node, parms) + overlayNode.Props.layerOffset;
    //        }
    //        return base.LayerFor(node, parms);
    //    }
    //}

    //public class PawnOverlayNode : PawnRenderNode
    //{
    //    public Color OverlayColor { get; set; } = Color.red;
    //    public float OverlayAlpha { get; set; } = 0.5f;


    //    public new PawnOverlayNodeProperties Props => (PawnOverlayNodeProperties)props;

    //    public PawnOverlayNode(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree)
    //    {

    //    }
    //}

    //public class PawnOverlayNodeProperties : PawnRenderNodeProperties
    //{
    //    public Color overlayColor = Color.red;
    //    public float overlayAlpha = 0.5f;
    //    public Vector3 offset = Vector3.zero;
    //    public float layerOffset = 1f;
    //    public GraphicData graphicData;

    //    public PawnOverlayNodeProperties()
    //    {
    //        this.nodeClass = typeof(PawnOverlayNode);
    //        this.workerClass = typeof(PawnOverlayNodeWorker);
    //    }
    //}

    //public class PawnOverlayNodeWorker : PawnRenderNodeWorker
    //{
    //    private Material _OverlayMat;
    //    private Material OverlayMat
    //    {
    //        get
    //        {
    //            if (_OverlayMat == null)
    //            {
    //                _OverlayMat = new Material(ShaderDatabase.Transparent);
    //            }
    //            return _OverlayMat;
    //        }
    //    }

    //    public override void AppendDrawRequests(PawnRenderNode node, PawnDrawParms parms, List<PawnGraphicDrawRequest> requests)
    //    {
    //        base.AppendDrawRequests(node, parms, requests);

    //        Material overlayMaterial = CreateOverlayMaterial(node as PawnOverlayNode, parms);

    //        if (overlayMaterial != null)
    //        {
    //            Mesh mesh = node.GetMesh(parms);
    //            if (mesh != null)
    //            {
    //                PawnGraphicDrawRequest pawnGraphicDrawRequest = new PawnGraphicDrawRequest(node, mesh, overlayMaterial);
    //                requests.Add(pawnGraphicDrawRequest);
    //            }
    //        }
    //    }

    //    public override Vector3 OffsetFor(PawnRenderNode node, PawnDrawParms parms, out Vector3 pivot)
    //    {
    //        if (node is PawnOverlayNode overlayNode)
    //        {
    //            return base.OffsetFor(node, parms, out pivot) + overlayNode.Props.offset;
    //        }
    //        return base.OffsetFor(node, parms, out pivot);
    //    }

    //    public override float LayerFor(PawnRenderNode node, PawnDrawParms parms)
    //    {
    //        if (node is PawnOverlayNode overlayNode)
    //        {
    //            return base.LayerFor(node, parms) + overlayNode.Props.layerOffset;
    //        }
    //        return base.LayerFor(node, parms);
    //    }

    //    private Material CreateOverlayMaterial(PawnOverlayNode node, PawnDrawParms parms)
    //    {
    //        if (node == null) return null;

    //        Color overlayColor = node.OverlayColor;
    //        overlayColor.a = node.OverlayAlpha;
    //        OverlayMat.color = overlayColor;
    //        return OverlayMat;
    //    }
    //}
}
