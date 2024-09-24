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

        public static float HeavenlyPactChance = 10f;
        public static float SixEyesChance = 10f;
        public static int AgeAbiltiesAwaken = 5;
        public static float CursedEnergyScalingCap = 10000f;

        public JJKMod(ModContentPack content) : base(content)
        {

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
}
    

