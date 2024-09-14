using UnityEngine;
using Verse;

namespace JJK
{
    [StaticConstructorOnStartup]
    public class JJKMod : Mod
    {
        public Shader Shader;

        public JJKMod(ModContentPack content) : base(content)
        {

            ////content.ReloadContent();
            //Log.Message("JJK Mod Init");
            //Log.Message($"Asset Bundles {content.assetBundles.loadedAssetBundles.EnumerableCount()}");


            //foreach (var item in content.assetBundles.loadedAssetBundles)
            //{
            //    Log.Message(item.name);

            //    foreach (var w in item.GetAllAssetNames())
            //    {
            //        Log.Message(w);
            //    }
            //}

            //Shader = (Shader)content.assetBundles.loadedAssetBundles[0].LoadAsset("Assets/YourShaderName.shader");
        }
    }
}
    

