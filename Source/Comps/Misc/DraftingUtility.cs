﻿using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace JJK
{
    public static class DraftingUtility
    {
        private static HashSet<Pawn> draftableCreatures = new HashSet<Pawn>();

        public static void RegisterDraftableCreature(Pawn pawn)
        {
            if (pawn != null && !draftableCreatures.Contains(pawn))
            {
                draftableCreatures.Add(pawn);
                EnsureDraftComponents(pawn);
            }
        }

        public static void UnregisterDraftableCreature(Pawn pawn)
        {
            if (pawn != null)
            {
                draftableCreatures.Remove(pawn);
            }
        }

        public static bool IsDraftableCreature(Pawn pawn)
        {
            return pawn != null && draftableCreatures.Contains(pawn);
        }

        private static void EnsureDraftComponents(Pawn pawn)
        {
            if (pawn.drafter == null)
            {
                pawn.drafter = new Pawn_DraftController(pawn);
            }
            if (pawn.equipment == null)
            {
                pawn.equipment = new Pawn_EquipmentTracker(pawn);
            }
        }


    }

    // Harmony patches
    [HarmonyPatch(typeof(FloatMenuMakerMap), "CanTakeOrder")]
    public static class FloatMenuMakerMap_CanTakeOrder_Patch
    {
        [HarmonyPostfix]
        public static void MakePawnControllable(Pawn pawn, ref bool __result)
        {
            if (DraftingUtility.IsDraftableCreature(pawn) && pawn.Faction?.IsPlayer == true)
            {
                __result = true;
            }
        }
    }

    [HarmonyPatch(typeof(Pawn), "WorkTypeIsDisabled")]
    public static class Pawn_WorkTypeIsDisabled_Patch
    {
        [HarmonyPostfix]
        public static void DisableDoctorWork(WorkTypeDef w, Pawn __instance, ref bool __result)
        {
            if (w == WorkTypeDefOf.Doctor && DraftingUtility.IsDraftableCreature(__instance)
                && __instance.RaceProps.IsMechanoid == false)
            {
                __result = true;
            }
        }
    }

    [HarmonyPatch(typeof(SchoolUtility), "CanTeachNow")]
    public static class SchoolUtility_CanTeachNow_Patch
    {
        [HarmonyPrefix]
        public static bool PreventTeaching(Pawn teacher, ref bool __result)
        {
            if (DraftingUtility.IsDraftableCreature(teacher))
            {
                __result = false;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Pawn), "IsColonistPlayerControlled", MethodType.Getter)]
    public static class Pawn_IsColonistPlayerControlled_Patch
    {
        [HarmonyPostfix]
        public static void AddDraftableCreatureAsColonist(Pawn __instance, ref bool __result)
        {
            if (DraftingUtility.IsDraftableCreature(__instance))
            {
                __result = __instance.Spawned && __instance.HostFaction == null && __instance.Faction == Faction.OfPlayer;
            }
        }
    }
}
    

