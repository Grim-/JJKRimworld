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
        public static float SixEyesCursedReinforcementBonus = 6000;
        public static int AgeAbiltiesAwaken = 5;
        public static float CursedEnergyScalingCap = 10000f;

        public static int NewRandomCTGeneInheritanceChance = 20;




        public JJKMod(ModContentPack content) : base(content)
        {

        }
    }
}
    

