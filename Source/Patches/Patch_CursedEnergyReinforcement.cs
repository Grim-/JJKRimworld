using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace JJK.Patches
{
    [HarmonyPatch(typeof(Verb_MeleeAttackDamage))]
    [HarmonyPatch("DamageInfosToApply")]
    public static class Patch_Verb_MeleeAttack_DamageInfosToApply
    {
        public static void Postfix(Verb_MeleeAttack __instance, LocalTargetInfo target, ref IEnumerable<DamageInfo> __result)
        {
            if (!__instance.CasterIsPawn || __instance.CasterPawn == null) 
                return;
            if (!__instance.CasterPawn.health.hediffSet.HasHediff(JJKDefOf.JJK_CursedReinforcementHediff))
                return;
            if (__instance.EquipmentSource != null) 
                return;

            DamageDef customDamageType = JJKUtility.GetCursedEnergyDamageType(__instance.CasterPawn);
            if (customDamageType == null) 
                return;

            List<DamageInfo> modifiedDamageInfos = ModifyDamageType(customDamageType, ref __result);
            __result = modifiedDamageInfos;
        }

        private static List<DamageInfo> ModifyDamageType(DamageDef Damage, ref IEnumerable<DamageInfo> __result)
        {
            List<DamageInfo> modifiedDamageInfos = new List<DamageInfo>();

            foreach (DamageInfo damageInfo in __result)
            {
                DamageInfo modifiedDamageInfo = new DamageInfo(
                    Damage,
                    damageInfo.Amount,
                    damageInfo.ArmorPenetrationInt,
                    damageInfo.Angle,
                    damageInfo.Instigator,
                    damageInfo.HitPart,
                    damageInfo.Weapon,
                    damageInfo.Category,
                    damageInfo.IntendedTarget
                );

                modifiedDamageInfos.Add(modifiedDamageInfo);
            }

            return modifiedDamageInfos;
        }
    }
}
