using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace JJK
{
    [StaticConstructorOnStartup]
    public static class AssetBundleShaderManager
    {
        private static Dictionary<string, Shader> ShaderCache = new Dictionary<string, Shader>();

        public static bool Cached = false;

        public static bool HasShader(string ShaderName)
        {
            return ShaderCache.ContainsKey(ShaderName);
        }


        public static void RegisterShader(string ShaderName, Shader Shader)
        {
            if (!HasShader(ShaderName))
            {
                ShaderCache.Add(ShaderName, Shader);
            }
            else
            {
                ShaderCache[ShaderName] = Shader;
            }
        }

        public static Shader GetShaderByAssetName(string ShaderName)
        {
            Log.Message($"Attemping to tetrie {ShaderName}");
            if (HasShader(ShaderName))
            {
                Log.Message($"{ShaderName} found.");
                return ShaderCache[ShaderName];
            }
            Log.Message($"{ShaderName} not found.");
            return null;
        }


        public static void CacheAllLoadedShaders()
        {
            Log.Message("Beginning Shader Cache");

            foreach (var mod in LoadedModManager.RunningMods)
            {
                foreach (var bundle in mod.assetBundles.loadedAssetBundles)
                {
                    foreach (var item in bundle.LoadAllAssets())
                    {
                        if (item is Shader shader)
                        {
                            RegisterShader(item.name, shader);

                        }
                    }
                }
            }

            Cached = true;
            Log.Message("Shader Cache complete");
        }

        public static void RemoveShader(string ShaderName)
        {
            if (HasShader(ShaderName))
            {
                ShaderCache.Remove(ShaderName);
            }
        }
    }
}
