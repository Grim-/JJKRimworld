using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using HarmonyLib;

namespace JJK
{
    //[StaticConstructorOnStartup]
    //public static class HarmonyPatches
    //{
    //    static HarmonyPatches()
    //    {
    //        Harmony harmonyPatch = new Harmony("kaisen_Jujustu");
    //        harmonyPatch.PatchAll();
    //    }
    //}

    //[HarmonyPatch(typeof(TraitSet), "GainTrait")]
    //public static class TraitSet_GainTraitSukuna_Patch
    //{
    //    // This method is called after the GainTrait method is executed
    //    public static void Postfix(Pawn ___pawn, Trait trait)
    //    {
    //        // Check if the gained trait matches the one you're interested in
    //       if (___pawn.story?.traits?.HasTrait(JJKDefOf.Sukuna_Vessel) ?? false)
    //        {
    //            // Define the genes you want to add
    //            GeneDef[] genesToAdd = new GeneDef[]
    //            {
    //                JJKDefOf.Gene_Cleave,
    //                JJKDefOf.Gene_FireArrow,
    //                JJKDefOf.Gene_Dismantle,
    //                JJKDefOf.Cursed_Energy
    //            };

    //            // Add each gene to the pawn's genetic makeup
    //            foreach (GeneDef geneDef in genesToAdd)
    //            {
    //                ___pawn?.genes?.AddGene(geneDef, true);
    //            }
    //        }
    //    }
    //}

    //[HarmonyPatch(typeof(TraitSet), "GainTrait")]
    //public static class TraitSet_GainTraitGojo_Patch
    //{
    //    // This method is called after the GainTrait method is executed
    //    public static void Postfix(Pawn ___pawn, Trait trait)
    //    {
    //        // Check if the gained trait matches the one you're interested in
    //        if (___pawn.story?.traits?.HasTrait(JJKDefOf.JJK_Gojo_SixEyes) ?? false)
    //        {
    //            // Define the genes you want to add
    //            GeneDef[] genesToAdd = new GeneDef[]
    //            {
    //                JJKDefOf.Gene_HollowPurple,
    //                JJKDefOf.Gene_Reversed_CursedTechnique_Blue,
    //                JJKDefOf.Gene_Reversed_CursedTechnique_Red,
    //                JJKDefOf.Cursed_Energy
    //            };

    //            // Add each gene to the pawn's genetic makeup
    //            foreach (GeneDef geneDef in genesToAdd)
    //            {
    //                ___pawn?.genes?.AddGene(geneDef, true);
    //            }
    //        }
    //    }
    //}
}
