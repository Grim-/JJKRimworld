using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

    //[HarmonyPatch(typeof(MentalStateHandler), "TryStartMentalState")]
    //public static class Patch_MentalStateHandler_TryStartMentalState
    //{
    //    private static FieldInfo pawnField = AccessTools.Field(typeof(MentalStateHandler), "pawn");

    //    public static bool Prefix(
    //        MentalStateHandler __instance,
    //        MentalStateDef stateDef,
    //        string reason,
    //        bool forced,
    //        bool forceWake,
    //        bool causedByMood,
    //        Pawn otherPawn,
    //        bool transitionSilently,
    //        bool causedByDamage,
    //        bool causedByPsycast,
    //        ref bool __result)
    //    {
    //        Pawn pawn = (Pawn)pawnField.GetValue(__instance);
    //        if (pawn.def.defName == "JJK_DemonDogBlack" || pawn.def.defName == "JJK_DemonDogWhite")
    //        {
    //            __result = false;
    //            return false; // Skip the original method
    //        }
    //        return true;
    //    }
    //}

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

        private static float HeavenlyPactChance = 10f;
        private static float SixEyesChance = 10f;


        static void Postfix(Pawn_GeneTracker __instance, GeneDef geneDef, bool xenogene)
        {
            Pawn pawn = __instance.pawn;

            if (geneDef == JJKDefOf.Gene_JJKCursedEnergy)
            {
                if (!RollHeavenlyPact(pawn, geneDef))
                {
                    JJKUtility.GiveRandomSorcererGrade(pawn);
                    RollSixEyes(pawn, geneDef);
                }
            }

            if (CanGrantHollowPurple(pawn, geneDef))
            {
                if (!pawn.HasAbility(JJKDefOf.Gojo_HollowPurple))
                {
                    pawn.abilities.GainAbility(JJKDefOf.Gojo_HollowPurple);
                }
            }

        }

        private static bool RollHeavenlyPact(Pawn Pawn, GeneDef GeneAdded)
        {
            if (Rand.Range(0, 100) < HeavenlyPactChance)
            {
                if (Rand.Bool)
                {
                    if (Pawn.genes.HasActiveGene(JJKDefOf.Gene_JJKHeavenlyPact))
                    {
                        Pawn.genes.AddGene(JJKDefOf.Gene_JJKHeavenlyPact, true);
                        return true;
                    }
                }
                else
                {
                    if (Pawn.genes.HasActiveGene(JJKDefOf.Gene_JJKHeavenlyPactCursedEnergy))
                    {
                        Pawn.genes.AddGene(JJKDefOf.Gene_JJKHeavenlyPactCursedEnergy, true);
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool RollSixEyes(Pawn Pawn, GeneDef GeneAdded)
        {
            if (Pawn.HasHeavenlyPact())
            {
                return false;
            }

            if (Rand.Range(0, 100) < SixEyesChance)
            {
                if (!Pawn.genes.HasActiveGene(JJKDefOf.Gene_JJKSixEyes))
                {
                    Pawn.genes.AddGene(JJKDefOf.Gene_JJKSixEyes, true);
                    return true;
                }
            }
            return false;
        }

        private static bool CanGrantHollowPurple(Pawn pawn, GeneDef geneDef)
        {
            return geneDef == JJKDefOf.Gene_JJKLimitless && pawn.HasSixEyes() || geneDef == JJKDefOf.Gene_JJKSixEyes && pawn.IsLimitlessUser();
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

    [HarmonyPatch(typeof(LoadedModManager), nameof(LoadedModManager.LoadAllActiveMods), new System.Type[] { typeof(bool) })]
    public static class Patch_OnModsReady
    {
        [HarmonyPostfix]
        static void Postfix(bool hotReload = false)
        {
            Log.Message("WDWDWDW");
            LoadedModManager.GetMod<JJKMod>().SetupShaders();
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

                Gene_CursedEnergy _CursedEnergy = pawn.GetCursedEnergy();
                if (_CursedEnergy != null && _CursedEnergy.HasCursedEnergy(20f))
                {
                    pawn.GetCursedEnergy()?.ConsumeCursedEnergy(20f);
                    Messages.Message($"{pawn.LabelShort} resisted being downed due to Reversed Curse Technique!", MessageTypeDefOf.PositiveEvent);
                    __result = false;
                }

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
                if (pawn.equipment.Primary.TryGetComp<ThingCompExt>( out ThingCompExt thingCompExt))
                {
                    if (thingCompExt != null)
                    {
                        thingCompExt.Notify_EquipOwnerUsedVerb(pawn, verb);
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

        public static Dictionary<HediffDef, ThinkTreeDef> ThinkTreeMap = new Dictionary<HediffDef, ThinkTreeDef>()
        {
            { JJKDefOf.JJK_ZombieWorkSlaveHediff , JJKDefOf.ZombieWorkSlave},
            { JJKDefOf.JJK_Shikigami , JJKDefOf.JJK_SummonedCreature }
        };

        private static bool HandleThinkTreePatch(Pawn_Thinker __instance, ref ThinkTreeDef __result, bool isConstant)
        {
            Pawn pawn = __instance.pawn;
            if (pawn == null || ThinkTreeMap == null) return true;

            if (!isConstant)
            {
                foreach (var item in ThinkTreeMap)
                {
                    if (pawn == null || item.Key == null || item.Value == null)
                    {
                        continue;
                    }

                    if (pawn.health.hediffSet.GetFirstHediffOfDef(item.Key) != null)
                    {
                        //Log.Message($"overring {pawn.Label} ThinkTree with {item.Value.defName}");
                        __result = item.Value;
                        return false;
                    }
                }
            }


            return true; 
        }


        [HarmonyPatch(typeof(Pawn), nameof(Pawn.Kill))]
        public static class Patch_Pawn_Kill
        {
            public static bool Prefix(Pawn __instance, DamageInfo? dinfo, Hediff exactCulprit)
            {
                if (__instance.IsShikigami())
                {
                    foreach (ThingComp thingComp in __instance.AllComps)
                    {
                        thingComp.Notify_Killed(__instance.Map, dinfo);
                    }

                    // Prevent corpse creation
                    if (__instance.Corpse != null)
                    {
                        __instance.Corpse.Destroy(DestroyMode.Vanish);
                    }

                    // Remove from map
                    if (__instance.Spawned)
                        __instance.DeSpawn();

                    if (!__instance.Destroyed)
                        __instance.Destroy(DestroyMode.Vanish);

                    // Skip the original method
                    return false;
                }
                return true;
            }
        }

        #region Summon Fleeing
        /// <summary>
        /// patches to stop summons fleeing, because they are mostly animals and lots of damagedefs have the flag to cause animalkinds to flee.
        /// </summary>
        [HarmonyPatch(typeof(MentalStateHandler), "TryStartMentalState")]
        public static class Patch_MentalStateHandler_TryStartMentalState
        {
            public static bool Prefix(Pawn ___pawn, ref bool __result)
            {
                if (___pawn.IsShikigami())
                {
                    __result = false;
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(Pawn_MindState), "StartFleeingBecauseOfPawnAction")]
        public static class Patch_Pawn_MindState_StartFleeingBecauseOfPawnAction
        {
            public static bool Prefix(Pawn ___pawn)
            {
                return !___pawn.IsShikigami();
            }
        }

        [HarmonyPatch(typeof(JobGiver_ConfigurableHostilityResponse), "TryGetFleeJob")]
        public static class Patch_JobGiver_ConfigurableHostilityResponse_TryGetFleeJob
        {
            public static bool Prefix(Pawn pawn, ref Job __result)
            {
                if (pawn.IsShikigami())
                {
                    __result = null;
                    return false;
                }
                return true;
            }
        }

        #endregion
    }
}
    

