using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using static UnityEngine.GraphicsBuffer;

namespace JJK
{
    [StaticConstructorOnStartup]
    public static class JKKPatchClass
    {
        static JKKPatchClass()
        {
            var harmony = new Harmony("com.jjk.jjkpatches");
            harmony.PatchAll();
        }
    }


    [HarmonyPatch(typeof(Pawn))]
    [HarmonyPatch("GetDisabledWorkTypes")]
    public static class Patch_Pawn_GetDisabledWorkTypes
    {
        private static List<WorkTypeDef> cachedDisabledTypes;

        [HarmonyPostfix]
        public static void Postfix(Pawn __instance, ref List<WorkTypeDef> __result, bool permanentOnly)
        {
            if (__instance.health.hediffSet.HasHediff(JJKDefOf.JJK_ZombieWorkSlaveHediff))
            {
                if (cachedDisabledTypes == null)
                {
                    cachedDisabledTypes = new List<WorkTypeDef>
                {
                    WorkTypeDefOf.Childcare,
                    WorkTypeDefOf.Doctor,
                    WorkTypeDefOf.DarkStudy,
                    WorkTypeDefOf.Research
                };
                }

                // Combine the original disabled types with our additional disabled types
                if (__result == null)
                {
                    __result = new List<WorkTypeDef>(cachedDisabledTypes);
                }
            }
        }
    }


    [HarmonyPatch(typeof(Pawn_GeneTracker), nameof(Pawn_GeneTracker.AddGene), new System.Type[] { typeof(GeneDef), typeof(bool) })]
    public static class Patch_AddGene
    {
        static void Postfix(Pawn_GeneTracker __instance, GeneDef geneDef, bool xenogene)
        {
            Pawn pawn = __instance.pawn;

            if (geneDef == JJKDefOf.Gene_JJKLimitless && pawn.HasSixEyes())
            {
                if (!pawn.HasAbility(JJKDefOf.Gojo_HollowPurple))
                {
                    pawn.abilities.GainAbility(JJKDefOf.Gojo_HollowPurple);
                }
            }
            else if (geneDef == JJKDefOf.Gene_JJKSixEyes && pawn.IsLimitlessUser())
            {
                if (!pawn.HasAbility(JJKDefOf.Gojo_HollowPurple))
                {
                    pawn.abilities.GainAbility(JJKDefOf.Gojo_HollowPurple);
                }
            }
            else if(geneDef == JJKDefOf.Gene_JJKCursedEnergy)
            {
                JJKUtility.GiveRandomSorcererGrade(pawn);
            }
        }
    }

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
                    JJKUtility.GiveCursedEnergy(pawn);
                    JJKUtility.ForceSorcererGradeGene(pawn, JJKDefOf.Gene_JJKSpecialGrade_Monstrous);
                    JJKUtility.AddTraitIfNotExist(pawn, TraitDef.Named("Cannibal"));
                }
            }
        }
    }


    [HarmonyPatch(typeof(Pawn_HealthTracker), nameof(Pawn_HealthTracker.WouldBeDownedAfterAddingHediff), new Type[] { typeof(HediffDef), typeof(BodyPartRecord), typeof(float) })]
    public static class Patch_WouldBeDownedAfterAddingHediff
    {
        public static bool Prefix(Pawn_HealthTracker __instance, ref bool __result, HediffDef def, BodyPartRecord part, float severity)
        {
            Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();

            if (pawn != null && HasActiveRCT(pawn))
            {
                if (part == pawn.health.hediffSet.GetBrain())
                {
                    // Allow normal behavior for brain damage
                    return true;
                }

                // Prevent downing for any other part
                __result = false;
                Messages.Message($"{pawn.LabelShort} resisted being downed due to Reversed Curse Technique!", MessageTypeDefOf.PositiveEvent);
                return false;
            }

            // Allow normal behavior if RCT is not active
            return true;
        }

        private static bool HasActiveRCT(Pawn pawn)
        {
            return pawn.health.hediffSet.HasHediff(JJKDefOf.JJK_RCTRegenHediff);
        }
    }


    [HarmonyPatch(typeof(Verb_MeleeAttackDamage))]
    [HarmonyPatch("ApplyMeleeDamageToTarget")]
    public static class Patch_Verb_MeleeAttack_ApplyMeleeDamageToTarget
    {
        public static void Postfix(Verb_MeleeAttack __instance, LocalTargetInfo target, ref DamageWorker.DamageResult __result)
        {
            if (__instance.EquipmentSource != null && target.Thing is Pawn targetPawn)
            {
                foreach (var item in __instance.EquipmentSource.GetComps<ThingCompExt>())
                {
                    if (item != null)
                    {
                        item.Notify_ApplyMeleeDamageToTarget(target, __result);
                    }
                }      
            }
        }
    }

    [HarmonyPatch(typeof(Pawn))]
    [HarmonyPatch("Notify_UsedVerb")]
    public static class Patch_Pawn_UsedVerb
    {
        public static void Postfix(Pawn __instance, Pawn pawn, Verb verb)
        {
            if (pawn != null && pawn.equipment != null && pawn.equipment.Primary != null)
            {
                foreach (var equipment in pawn.equipment.AllEquipmentListForReading)
                {
                    foreach (var item in equipment.GetComps<ThingCompExt>())
                    {
                        if (item != null)
                        {
                            item.Notify_EquipOwnerUsedVerb(pawn, verb);
                        }
                    }
                }
            }    
        }
    }

    public static class Patch_Pawn_Thinker_ThinkTrees
    {
        [HarmonyPatch(typeof(Pawn_Thinker), "MainThinkTree", MethodType.Getter)]
        public static class Patch_MainThinkTree
        {
            public static bool Prefix(Pawn_Thinker __instance, ref ThinkTreeDef __result)
            {
                return HandleThinkTreePatch(__instance, ref __result, isConstant: false);
            }
        }

        [HarmonyPatch(typeof(Pawn_Thinker), "ConstantThinkTree", MethodType.Getter)]
        public static class Patch_ConstantThinkTree
        {
            public static bool Prefix(Pawn_Thinker __instance, ref ThinkTreeDef __result)
            {
                return HandleThinkTreePatch(__instance, ref __result, isConstant: true);
            }
        }

        private static bool HandleThinkTreePatch(Pawn_Thinker __instance, ref ThinkTreeDef __result, bool isConstant)
        {
            Pawn pawn = __instance.pawn;
            if (pawn == null) return true;


            if (pawn.health.hediffSet.GetFirstHediffOfDef(JJKDefOf.JJK_ZombieWorkSlaveHediff) != null)
            {
                __result = JJKDefOf.ZombieWorkSlave;
                //Log.Message($"Overriding {(isConstant ? "Constant" : " ")} ThinkTree {pawn.LabelShort} {pawn.ThingID}");
                return false;
            }

            //if (JJKUtility.IsSummonedCreature(pawn))
            //{
            //    __result = isConstant ? JJKDefOf.JJK_EmptyConstantThinkTree : JJKDefOf.JJK_SummonedCreature;
            //    return false;
            //}

            //if (JJKUtility.IsAbsorbedCreature(pawn))
            //{
            //    __result = isConstant ? JJKDefOf.JJK_EmptyConstantThinkTree : JJKDefOf.JJK_SummonedCreature;
            //    //Log.Message($"Overriding {(isConstant ? "Constant" : " ")} ThinkTree {pawn.LabelShort} {pawn.ThingID}");
            //    return false;
            //}

            //Log.Message($"JJK: Using default {(isConstant ? "Constant" : "Main")} ThinkTree for {pawn.LabelShort} (ThingID: {pawn.ThingID})");

            return true; // Run the original method
        }
    }
}
    

