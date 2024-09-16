using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Verse;

namespace JJK
{
    [StaticConstructorOnStartup]
    public class JJKMod : Mod
    {
        public JJKMod(ModContentPack content) : base(content)
        {
         
        }


        public void SetupShaders()
        {
            Log.Message($"SetupShaders");
            // Load your custom shader from the asset bundle

            Shader CustomShader = Content.assetBundles.loadedAssetBundles[0].LoadAsset<Shader>("lavashader");

            if (CustomShader != null)
            {
                // Use reflection to get the private lookup dictionary
                FieldInfo lookupField = typeof(ShaderDatabase).GetField("lookup", BindingFlags.NonPublic | BindingFlags.Static);
                Dictionary<string, Shader> lookup = lookupField.GetValue(null) as Dictionary<string, Shader>;


                string shaderPath = "lavashader";
                if (lookup != null)
                {
                    if (!lookup.ContainsKey(shaderPath))
                    {
                        lookup[shaderPath] = CustomShader;
                        Log.Message($"Custom shader '{shaderPath}' added to ShaderDatabase.");
                    }
                }
                else
                {
                    Log.Error("Failed to access ShaderDatabase lookup.");
                }
            }
            else
            {
                Log.Error("Failed to load custom shader from asset bundle.");
            }
        }
    }

    public class CustomShaderDef : Def
    {

    }


}
    

