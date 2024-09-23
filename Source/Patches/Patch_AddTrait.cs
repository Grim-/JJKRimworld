using HarmonyLib;
using RimWorld;
using Verse;

namespace JJK
{
    [HarmonyPatch(typeof(TraitSet), nameof(TraitSet.GainTrait), new System.Type[] { typeof(Trait), typeof(bool) })]
    public static class Patch_AddTrait
    {
        static void Postfix(TraitSet __instance, Trait trait, bool suppressConflicts = false)
        {
            Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            if (pawn != null && trait != null)
            {
                if (trait.def == JJKDefOf.JJK_SukunaTrait)
                {
                    JJKGeneUtil.GiveCursedEnergy(pawn);
                    JJKGeneUtil.ForceSorcererGradeGene(pawn, JJKDefOf.Gene_JJKSpecialGrade_Monstrous);
                    JJKUtility.AddTraitIfNotExist(pawn, TraitDef.Named("Cannibal"));
                }
            }
        }
    }
}
    

