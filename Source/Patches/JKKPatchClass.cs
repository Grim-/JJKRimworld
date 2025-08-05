using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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




	//[HarmonyPatch(typeof(FloatMenuMakerMap))]
	//[HarmonyPatch("GetOptions")]
	//public static class Patch_FloatMenuMakerMap_ChoicesAtFor
	//{
	//	public static void Postfix(Vector3 clickPos, List<Pawn> selectedPawns, bool suppressAutoTakeableGoto, ref List<FloatMenuOption> __result)
	//	{
	//		if (selectedPawns == null || selectedPawns.Count == 0)
	//			return;

	//		if (pawn.RaceProps.Humanlike || pawn.Drafted) 
	//			return;

	//		IntVec3 intVec = IntVec3.FromVector3(clickPos);

	//		if (!intVec.InBounds(pawn.Map)) return;

	//		var toadComp = pawn.GetComp<Comp_TenShadowsToad>();
	//		if (toadComp == null) 
	//			return;

 //           if (__result == null)
 //           {
	//			__result = new List<FloatMenuOption>();
 //           }

	//		foreach (FloatMenuOption option in toadComp.CompFloatMenuOptions(pawn))
	//		{
	//			__result.Add(option);
	//		}
	//	}
	//}///////////

	[HarmonyPatch(typeof(Pawn_AgeTracker))]
	[HarmonyPatch("BirthdayBiological")]
	public static class Patch_Pawn_AgeTracker_BirthdayBiological
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
					// behavior for brain damage
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

			//behavior if RCT is not active
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
			TryNotifyEquipment(__instance, target, ref __result);
		}


		public static void TryNotifyEquipment(Verb_MeleeAttack __instance, LocalTargetInfo target, ref DamageWorker.DamageResult __result)
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


	[HarmonyPatch(typeof(Pawn), nameof(Pawn.Kill), new Type[] { typeof(DamageInfo), typeof(Hediff)})]
	public static class Patch_Pawn_Kill
	{
		public static void Postfix(Pawn __instance, DamageInfo? dinfo, Hediff exactCulprit)
		{
            if (__instance.kindDef.HasModExtension<PawnExtension_NoCorpse>() || __instance.IsShikigami())
            {
				if (__instance.Corpse != null && !__instance.Corpse.Destroyed)
					__instance.Corpse.Destroy();
			}
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
			if (___pawn.Faction == Faction.OfPlayer && ___pawn.IsShikigami() || ___pawn.Faction == Faction.OfPlayer && ___pawn.kindDef.HasModExtension<PawnExtension_NoFleeing>())
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
			return !___pawn.IsShikigami() || ___pawn.kindDef.HasModExtension<PawnExtension_NoFleeing>();
		}
	}

	[HarmonyPatch(typeof(JobGiver_ConfigurableHostilityResponse), "TryGetFleeJob")]
	public static class Patch_JobGiver_ConfigurableHostilityResponse_TryGetFleeJob
	{
		public static bool Prefix(Pawn pawn, ref Job __result)
		{
			if (pawn.Faction == Faction.OfPlayer && pawn.IsShikigami() || pawn.Faction == Faction.OfPlayer && pawn.kindDef.HasModExtension<PawnExtension_NoFleeing>())
			{
				__result = null;
				return false;
			}
			return true;
		}
	}

	#endregion



	/// <summary>
	/// Handles immunities, __instance is thing being hit.
	/// </summary>
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

	//[HarmonyPatch(typeof(ITab_Pawn_Gear), "IsVisible", MethodType.Getter)]
	//public static class Patch_HideEquipTabOnSummons
	//{
	//	public static bool Prefix(ITab_Pawn_Gear __instance, ref bool __result)
	//	{
	//		Pawn pawn = Traverse.Create(__instance).Property("SelPawn").GetValue<Pawn>();
	//		if (pawn != null && pawn.IsShikigami())
	//		{
	//			__result = false;
	//			return false;
	//		}
	//		return true;
	//	}
	//}


	[HarmonyPatch(typeof(HediffWithParents))]
	[HarmonyPatch("SetParents")]
	public static class Patch_GeneInheritance
	{
		[HarmonyPrefix]
		public static bool Prefix(HediffWithParents __instance, ref Pawn mother, ref Pawn father, ref GeneSet geneSet)
		{
			if (mother != null && father != null)
			{
				Gene_CursedEnergy motherCE = mother.GetCursedEnergy();
				Gene_CursedEnergy fatherCE = father.GetCursedEnergy();

				if (motherCE != null || fatherCE != null)
				{
					// Give Cursed Energy gene to the child
					GeneDef cursedEnergyGeneDef = JJKDefOf.Gene_JJKCursedEnergy;
					geneSet.AddGene(cursedEnergyGeneDef);

					// Handle Cursed Technique inheritance
					Gene motherCT = mother.GetCursedTechniqueGene();
					Gene fatherCT = father.GetCursedTechniqueGene();
					GeneDef inheritedCT = null;

					int newCTChance = Rand.Range(0, 100);
					if (newCTChance < JJKMod.NewRandomCTGeneInheritanceChance)
					{
						// New random Cursed Technique
						List<GeneDef> ctGeneDefs = DefDatabase<GeneDef>.AllDefs
							.Where(g => g.HasModExtension<CursedTechniqueGeneExtension>())
							.ToList();
						inheritedCT = ctGeneDefs.RandomElement();
					}
					else
					{
						// Inherit from either mother or father
						inheritedCT = Rand.Value > 0.5f ? motherCT?.def : fatherCT?.def;
					}

					if (inheritedCT != null)
					{
						geneSet.AddGene(inheritedCT);
					}

					Log.Message($"Mother's CT: {motherCT?.def.LabelCap ?? "None"}, Father's CT: {fatherCT?.def.LabelCap ?? "None"}, Inherited CT: {inheritedCT?.LabelCap ?? "None"}");

					Gene motherGrade = mother.GetSorcererGradeGene();
					Gene fatherGrade = father.GetSorcererGradeGene();

					GeneDef bestParentGrade = JJKGeneUtil.GetHigherPriorityGrade(motherGrade?.def, fatherGrade?.def);
					GeneDef upgradedGrade = JJKGeneUtil.UpgradeGrade(bestParentGrade);

					if (upgradedGrade != null)
					{
						geneSet.AddGene(upgradedGrade);
					}

	 
					geneSet.GenesListForReading.RemoveAll(g => g == JJKDefOf.Gene_JJKCursedEnergy || g.HasModExtension<CursedEnergyGeneExtension>());
				}
			}

			return true; 
		}
	}
}