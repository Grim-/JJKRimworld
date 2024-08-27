//using Verse;

//namespace JJK
//{
//    public class ZanpaktoGraphicData : GraphicData
//    {
//        public GraphicData sealedGraphic;
//        public GraphicData shikaiGraphic;
//        public GraphicData bankaiGraphic;

//        public ZanpaktoGraphicData()
//        {

//            graphicClass = typeof(Graphic_Zanpakto);
//        }
//    }


//    public class Graphic_Zanpakto : Graphic_WithPropertyBlock
//    {
//        private ZanpaktoGraphicData zanpaktoData;

//        private Graphic sealedGraphic;
//        private Graphic shikaiGraphic;
//        private Graphic bankaiGraphic;

//        private Texture2D SealedGraphic = null;
//        private Texture2D BankaiGraphic = null;

//        public ZanpaktoState CurrentState { get; set; } = ZanpaktoState.Sealed;

//        public override Material MatSingle
//        {
//            get
//            {
//                Graphic g = GetGraphicForState(CurrentState);
//                Log.Message("Graphic_Zanpakto MatSingle called");
//                Log.Message(g);
//                return bankaiGraphic.MatSingle;
//            }
//        }
//        public override void Init(GraphicRequest req)
//        {
//            base.Init(req);

//            Log.Message("Graphic_Zanpakto Init called");
//            if (!(req.graphicData is ZanpaktoGraphicData zanpaktoData))
//            {
//                Log.Error("Graphic_Zanpakto must use ZanpaktoGraphicData");
//                return;
//            }

//            this.sealedGraphic = GraphicDatabase.Get<Graphic_Single>(zanpaktoData.sealedGraphic.texPath, req.shader, this.drawSize, this.color, this.colorTwo, this.data);
//            this.shikaiGraphic = GraphicDatabase.Get<Graphic_Single>(zanpaktoData.shikaiGraphic.texPath, req.shader, this.drawSize, this.color, this.colorTwo, this.data);
//            this.bankaiGraphic = GraphicDatabase.Get<Graphic_Single>(zanpaktoData.bankaiGraphic.texPath, req.shader, this.drawSize, this.color, this.colorTwo, this.data);
//        }

//        public override Material MatAt(Rot4 rot, Thing thing = null)
//        {
//            if (thing == null)
//            {
//                return this.MatSingle;
//            }
//            return this.MatSingleFor(thing);
//        }

//        // Token: 0x060022EC RID: 8940 RVA: 0x000D3C15 File Offset: 0x000D1E15
//        public override Material MatSingleFor(Thing thing)
//        {
//            if (thing == null)
//            {
//                return this.MatSingle;
//            }
//            return GetGraphicForState(CurrentState).MatSingle;
//        }
//        public override Graphic GetColoredVersion(Shader newShader, Color newColor, Color newColorTwo)
//        {
//            return GetGraphicForState(CurrentState).GetColoredVersion(newShader, newColor, newColorTwo);
//        }

//        public override void TryInsertIntoAtlas(TextureAtlasGroup groupKey)
//        {
//            base.TryInsertIntoAtlas(groupKey);

//            GlobalTextureAtlasManager.TryInsertStatic(groupKey, (Texture2D)this.sealedGraphic.MatSingle.mainTexture);
//            GlobalTextureAtlasManager.TryInsertStatic(groupKey, (Texture2D)this.shikaiGraphic.MatSingle.mainTexture);
//            GlobalTextureAtlasManager.TryInsertStatic(groupKey, (Texture2D)this.bankaiGraphic.MatSingle.mainTexture);
//        }

//        //protected override void DrawMeshInt(Mesh mesh, Vector3 loc, Quaternion quat, Material mat)
//        //{
//        //    base.DrawMeshInt(mesh, loc, quat, mat);

//        //    Log.Message("Graphic_Zanpakto DrawMeshInt called");
//        //}


//        //public override void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
//        //{
//        //    Log.Message("Graphic_Zanpakto DrawWorker called");
//        //    Graphic currentGraphic = GetGraphicForState(CurrentState);
//        //    Log.Message(currentGraphic);
//        //    currentGraphic.Draw(loc, rot, thing, extraRotation);
//        //    currentGraphic.DrawWorker(loc, rot, thingDef, thing, extraRotation);
//        //}


//        public void SetCurrentState(ZanpaktoState newState)
//        {
//            CurrentState = newState;
//            Log.Message("Graphic_Zanpakto SetCurrentState called");
//        }

//        public Graphic GetGraphicForState(ZanpaktoState state)
//        {
//            Graphic graphicToUse = null;
//            switch (state)
//            {
//                case ZanpaktoState.Sealed:
//                    graphicToUse = sealedGraphic;
//                    break;
//                case ZanpaktoState.Shikai:
//                    graphicToUse = shikaiGraphic;
//                    break;
//                case ZanpaktoState.Bankai:
//                    graphicToUse = bankaiGraphic;
//                    break;
//            }

//            return graphicToUse;
//        }
//    }
//}