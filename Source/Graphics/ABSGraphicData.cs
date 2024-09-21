using RimWorld;
using UnityEngine;
using Verse;

namespace JJK
{
    public class Graphic_ABS : Graphic_Single
    {

        private Shader _Shader;
        public new Shader Shader
        {
            get
            {
                if (_Shader == null)
                {
                    _Shader = Data.GetShaderFromAssets();
                }
                return _Shader;
            }
        }


        public ABSGraphicData Data => (ABSGraphicData)data;


        public override Material MatAt(Rot4 rot, Thing thing = null)
        {
            Material material =  base.MatAt(rot, thing);
            material.shader = Shader;
            foreach (var item in Data.shaderParameters)
            {
                item.Apply(material);
            }

            material.renderQueue = 3001;
            return material;
        }
    }

    public class ABSGraphicData : GraphicData
    {
        public string customShaderName;

        public new Shader Shader
        {
            get
            {
                return AssetBundleShaderManager.GetShaderByAssetName(customShaderName);
            }
        }
     
        public Shader GetShaderFromAssets()
        {
            if (!AssetBundleShaderManager.Cached)
            {
                AssetBundleShaderManager.CacheAllLoadedShaders();
            }

            if (AssetBundleShaderManager.HasShader(customShaderName))
            {
                return AssetBundleShaderManager.GetShaderByAssetName(customShaderName);
            }


            foreach (var mod in LoadedModManager.RunningMods)
            {
                foreach (var bundle in mod.assetBundles.loadedAssetBundles)
                {
                    Shader shader = bundle.LoadAsset<Shader>(customShaderName);
                    if (shader != null)
                    {
                        AssetBundleShaderManager.RegisterShader(customShaderName, shader);
                        return shader;
                    }
                }
            }

            Shader fallbackShader = ShaderDatabase.LoadShader(customShaderName);
            if (fallbackShader != null)
            {
                return fallbackShader;
            }
            Log.Warning($"Shader '{customShaderName}' not found in any asset bundle or by name.");
            return ShaderDatabase.DefaultShader;
        }
    }
}
