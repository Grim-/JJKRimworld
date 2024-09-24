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

    [HarmonyPatch(typeof(Pawn_AgeTracker))]
    [HarmonyPatch("BirthdayBiological")]
    public static partial class Patch_Pawn_AgeTracker_BirthdayBiological
    {
        public static void Postfix(Pawn_AgeTracker __instance, Pawn ___pawn, int birthdayAge)
        {
            Pawn pawn = ___pawn;
            Gene_CursedEnergy CursedEnergy = pawn.GetCursedEnergy();
            if (CursedEnergy == null)
            {
                return;
            }

            if (birthdayAge >= JJKMod.AgeAbiltiesAwaken && !___pawn.HasCursedTechnique())
            {
                if (!pawn.HasCursedTechnique())
                {
                    JJKGeneUtil.GiveRandomCursedTechnique(pawn);
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
                    if (pawn.equipment.Primary.TryGetComp<ThingCompExt>(out ThingCompExt thingCompExt))
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
                public static void PostFix(Pawn __instance, DamageInfo? dinfo, Hediff exactCulprit)
                {
                    if (__instance.IsShikigami())
                    {
                        //foreach (ThingComp thingComp in __instance.AllComps)
                        //{
                        //    thingComp.Notify_Killed(__instance.Map, dinfo);
                        //}

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
                        //return false;
                    }
                    //return true;
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


            [HarmonyPatch(typeof(Thing))]
            [HarmonyPatch("PreApplyDamage")]
            public static class Patch_Thing_PreApplyDamage
            {
                [HarmonyPrefix]
                public static bool Prefix(Thing __instance, ref DamageInfo dinfo, out bool absorbed)
                {
                    absorbed = false;
                    if (__instance is Pawn pawn)
                    {
                        var immunityComps = pawn.GetSelectiveDamageImmunityComps();
                        if (immunityComps.Any())
                        {
                            foreach (var comp in immunityComps)
                            {
                                if (!comp.IsVulnerableToDamage(dinfo.Def))
                                {
                                    absorbed = true;
                                    return false;
                                }
                                else
                                {
                                    dinfo = comp.ModifyDamage(dinfo);
                                }
                            }
                        }
                    }
                    return true;
                }

            }
        }

        [HarmonyPatch(typeof(ITab_Pawn_Gear), "IsVisible", MethodType.Getter)]
        public static class Patch_HideEquipTabOnSummons
        {
            public static bool Prefix(ITab_Pawn_Gear __instance, ref bool __result)
            {
                Pawn pawn = Traverse.Create(__instance).Property("SelPawn").GetValue<Pawn>();
                if (pawn != null && pawn.IsShikigami())
                {
                    __result = false;
                    return false;
                }
                return true;
            }
        }
    }
}
    

