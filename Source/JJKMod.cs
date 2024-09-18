using HarmonyLib;
using RimWorld;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using Verse;

namespace JJK
{
    [StaticConstructorOnStartup]
    public class JJKMod : Mod
    {
        public static Dictionary<string, Shader> CustomShaders = new Dictionary<string, Shader>();

        public JJKMod(ModContentPack content) : base(content)
        {
           // CoroutineGO CoroutineGO = Find.Root.gameObject.AddComponent<CoroutineGO>();
           // CoroutineGO.StartCoroutine(DelayForLikeEver());
        }

        //private IEnumerator DelayForLikeEver()
        //{
        //    yield return new WaitForSeconds(15f);
        //   ///SetupShaders();
        //    LoadCustomShaders();
        //    ApplyCustomShaders();
        //    yield break;              
        //}

        public void SetupShaders()
        {
            Log.Message($"SetupShaders");
            // Load your custom shader from the asset bundle




            foreach (var item in Content.assetBundles.loadedAssetBundles)
            {
                Log.Message(item.name);

                foreach (var asset in item.GetAllAssetNames())
                {
                    Log.Message($"Asset Name {asset}");
                }
            }


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


       
            //foreach (var item in DefDatabase<ThingDef>.AllDefs)
            //{
            //    if (item.graphicData != null)
            //    {
            //        if (item.graphicData.shaderType == null)
            //        {
            //            item.graphicData.shaderType = new ShaderTypeDef()
            //            {
            //                defName = 
            //            }
            //        }
            //    }
            //}
        }

        private void LoadCustomShaders()
        {
            foreach (var bundle in Content.assetBundles.loadedAssetBundles)
            {
                foreach (var assetName in bundle.GetAllAssetNames())
                {
                    if (assetName.EndsWith(".shader"))
                    {
                        Shader customShader = bundle.LoadAsset<Shader>(assetName);
                        if (customShader != null)
                        {
                            string shaderName = Path.GetFileNameWithoutExtension(assetName);
                            CustomShaders[shaderName] = customShader;
                            Log.Message($"Loaded custom shader: {shaderName}");
                        }
                    }
                }
            }
        }

        private void ApplyCustomShaders()
        {
            foreach (var def in DefDatabase<ThingDef>.AllDefs)
            {
                if (def.graphicData != null && def.graphicData.shaderType != null)
                {
                    string shaderName = def.graphicData.shaderType.ToString();
                    if (CustomShaders.TryGetValue(shaderName, out Shader customShader))
                    {
                        if (def.graphicData.shaderType is CustomShaderTypeDef custom)
                        {
                            custom.Shader = customShader;
                            Log.Message($"Applied custom shader {shaderName} to {def.defName}");
                        }


                    }
                }
            }
        }
    }

    public class CustomShaderTypeDef : ShaderTypeDef
    {
        public new Shader Shader
        {
            get
            {
                if (JJKMod.CustomShaders.TryGetValue(shaderPath, out Shader customShader))
                {
                    return customShader;
                }
                Log.Error($"Custom shader not found: {shaderPath}");
                return ShaderDatabase.DefaultShader;
            }
            set 
            { 
            
            }  // We don't need to implement the setter
        }

        public string NewShaderPath;
    }


    [DisallowMultipleComponent]
    public class CoroutineGO : MonoBehaviour
    {

    }
}
    

