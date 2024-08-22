using UnityEngine;
using Verse;

namespace JJK
{
    [StaticConstructorOnStartup]
    public class JJKMod : Mod
    {
        public static Material CustomMaterial { get; private set; }
        public JJKMod(ModContentPack content) : base(content)
        {
            //Shader customShader = content.assetBundles.loadedAssetBundles
            //if (customShader != null)
            //{
            //    CustomMaterial = new Material(customShader);
            //    // Set any properties on CustomMaterial here
            //}
            //else
            //{
            //    Log.Error("Failed to load custom shader from AssetBundle.");
            //}
        }
    }
}
    

