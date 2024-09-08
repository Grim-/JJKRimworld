﻿using JetBrains.Annotations;
using LudeonTK;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;

namespace JJK
{
    public class CursedEnergyGeneExtension : DefModExtension
    {
        public int priority = 0;
    }

    public class CursedTechniqueGeneExtension : DefModExtension
    {
        public int priority = 0;
    }
    public static class JJKUtility
    {

        private static DollTransformationWorldComponent _DollTransformationWorldComponent;
        public static DollTransformationWorldComponent DollTransformationWorldComponent
        {
            get
            {
                if (_DollTransformationWorldComponent == null)
                {
                    _DollTransformationWorldComponent = Find.World.GetComponent<DollTransformationWorldComponent>();
                }

                return _DollTransformationWorldComponent;
            }
        }

        private static SummonedCreatureManager _SummonedCreatureManager;
        public static SummonedCreatureManager SummonedCreatureManager
        {
            get
            {
                if (_SummonedCreatureManager == null)
                {
                    _SummonedCreatureManager = Find.World.GetComponent<SummonedCreatureManager>();
                }

                return _SummonedCreatureManager;
            }
        }


        //maybe the lazy init is causing the save/load issues? I dont fuckin know at this point.
      //  private static AbsorbedCreatureManager _AbsorbedCreatureManager;
      //  public static AbsorbedCreatureManager AbsorbedCreatureManager = Current.Game.GetComponent<AbsorbedCreatureManager>();
        //{
        //    get
        //    {
        //        if (_AbsorbedCreatureManager == null)
        //        {
        //            _AbsorbedCreatureManager = Current.Game.GetComponent<AbsorbedCreatureManager>();
        //        }

        //        return _AbsorbedCreatureManager;
        //    }
        //}


        [DebugAction("JJK", "Restore Pawn CE", actionType = DebugActionType.ToolMapForPawns, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        public static void MyDebugAction(Pawn p)
        {
            Gene_CursedEnergy cursedEnergy = p.GetCursedEnergy();

            if (cursedEnergy != null)
            {
                cursedEnergy.RestoreCursedEnergy(1000);
            }
        }
        public static void TransferCursedEnergyGenes(Pawn sourcePawn, Pawn targetPawn)
        {
            var sourceGenes = sourcePawn.genes.GenesListForReading
                .Where(g => g.def.HasModExtension<CursedEnergyGeneExtension>())
                .ToList();

            foreach (Gene sourceGene in sourceGenes)
            {
                var extension = sourceGene.def.GetModExtension<CursedEnergyGeneExtension>();
                Gene existingGene = targetPawn.genes.GetGene(sourceGene.def);

                if (existingGene == null)
                {
                    targetPawn.genes.AddGene(sourceGene.def, true);
                }
                else
                {
                    var existingExtension = existingGene.def.GetModExtension<CursedEnergyGeneExtension>();
                    if (extension.priority > existingExtension.priority)
                    {
                        targetPawn.genes.RemoveGene(existingGene);
                        targetPawn.genes.AddGene(sourceGene.def, true);
                    }
                }
            }
        }

        public static void ForceSorcererGradeGene(Pawn targetPawn, GeneDef GeneToForce)
        {
            CursedEnergyGeneExtension geneToForceExtension = GeneToForce.GetModExtension<CursedEnergyGeneExtension>();
            if (geneToForceExtension == null)
            {
                Log.Message($"{GeneToForce} has no CursedEnergyGeneExtension.");
                return;
            }

            var allSorcererGenes = targetPawn.genes.GenesListForReading
                .Where(g => g.def.HasModExtension<CursedEnergyGeneExtension>())
                .ToList();

            foreach (Gene sourceGene in allSorcererGenes)
            {
                targetPawn.genes.RemoveGene(sourceGene);
            }

            targetPawn.genes.AddGene(GeneToForce, true);
        }

        public static void GiveRandomSorcererGrade(Pawn targetPawn)
        {
            // Find all GeneDefs with CursedEnergyGeneExtension
            List<GeneDef> sorcererGeneDefs = DefDatabase<GeneDef>.AllDefs
                .Where(geneDef => geneDef.HasModExtension<CursedEnergyGeneExtension>())
                .ToList();

            if (sorcererGeneDefs.Count == 0)
            {
                Log.Warning("No GeneDefs found with CursedEnergyGeneExtension.");
                return;
            }

            // Remove existing sorcerer genes
            var existingSorcererGenes = targetPawn.genes.GenesListForReading
                .Where(g => g.def.HasModExtension<CursedEnergyGeneExtension>())
                .ToList();
            foreach (Gene sourceGene in existingSorcererGenes)
            {
                targetPawn.genes.RemoveGene(sourceGene);
            }

            // Select a random sorcerer GeneDef
            GeneDef randomSorcererGeneDef = sorcererGeneDefs.RandomElement();

            // Add the randomly selected gene to the pawn
            targetPawn.genes.AddGene(randomSorcererGeneDef, true);
        }
        public static void GiveRandomSorcererGene(Pawn targetPawn, bool RemoveExisting = false)
        {
            // Find all GeneDefs with CursedEnergyGeneExtension
            List<GeneDef> sorcererGeneDefs = DefDatabase<GeneDef>.AllDefs
                .Where(geneDef => geneDef.HasModExtension<CursedTechniqueGeneExtension>())
                .ToList();

            if (sorcererGeneDefs.Count == 0)
            {
                Log.Warning("No GeneDefs found with CursedTechniqueGeneExtension.");
                return;
            }
            if (RemoveExisting)
            {
                // Remove existing sorcerer genes
                var existingSorcererGenes = targetPawn.genes.GenesListForReading
                    .Where(g => g.def.HasModExtension<CursedEnergyGeneExtension>())
                    .ToList();
                foreach (Gene sourceGene in existingSorcererGenes)
                {
                    targetPawn.genes.RemoveGene(sourceGene);
                }
            }

            // Select a random sorcerer GeneDef
            GeneDef randomSorcererGeneDef = sorcererGeneDefs.RandomElement();
            // Add the randomly selected gene to the pawn
            targetPawn.genes.AddGene(randomSorcererGeneDef, true);
        }

        public static void AddTraitIfNotExist(Pawn Pawn, TraitDef TraitDef, int degree = 0, bool force = false)
        { 
            if (TraitDef != null && !Pawn.story.traits.HasTrait(TraitDef))
            {
                Pawn.story.traits.GainTrait(new Trait(TraitDef, degree, force));
            }
        }


        public static void CreateLightningStrike(Map Map, IntVec3 Position, Thing Instigator, DamageDef DamageDef, int DamageAmount = 10, float Arp = -1, float radius = 1f)
        {
            WeatherEvent_LightningStrike lightningStrike = new WeatherEvent_LightningStrike(Map, Position);
            lightningStrike.FireEvent();

            // Apply additional effects
            if (radius > 0f)
            {
                GenExplosion.DoExplosion(
                    Position,
                    Map,
                    radius,
                    DamageDefOf.Bomb,
                    Instigator,
                    DamageAmount,
                    Arp
                );
            }
        }



        public static Pawn SpawnShikigami(PawnKindDef pawnKindDef, Pawn Master, Map Map, IntVec3 Position)
        {
            if (pawnKindDef == null || Map == null)
            {
                return null;
            }

            Pawn shikigami = PawnGenerator.GeneratePawn(pawnKindDef, Master.Faction);
            GenSpawn.Spawn(shikigami, Position, Map);
            Hediff_Shikigami summon = (Hediff_Shikigami)shikigami.health.GetOrAddHediff(JJKDefOf.JJK_Shikigami);

            shikigami.ageTracker.DebugSetAge(3444444);

            if (summon != null)
            {
                summon.SetMaster(Master);
            }

            TrainPawn(shikigami, Master);
            return shikigami;
        }


        public static void TrainPawn(Pawn PawnToTrain, Pawn Trainer = null)
        {
            if (PawnToTrain.training != null)
            {
                foreach (var item in DefDatabase<TrainableDef>.AllDefsListForReading)
                {
                    if (PawnToTrain.training.CanAssignToTrain(item).Accepted)
                    {
                        PawnToTrain.training.SetWantedRecursive(item, true);
                        PawnToTrain.training.Train(item, Trainer, true);
                    }

                }
            }
        }


        public static bool IsShikigami(this Pawn pawn)
        {
            return pawn.health.hediffSet.HasHediff(JJKDefOf.JJK_Shikigami);
        }

        public static Pawn FindPawnById(string thingId)
        {
            return Find.WorldPawns.AllPawnsAlive.FirstOrDefault(p => p.ThingID == thingId) ??
                   Find.CurrentMap?.mapPawns.AllPawns.FirstOrDefault(p => p.ThingID == thingId);
        }

        public static void GiveCursedEnergy(Pawn targetPawn)
        {
            if (targetPawn == null || targetPawn.genes == null) return;

            if (!targetPawn.genes.HasActiveGene(JJKDefOf.Gene_JJKCursedEnergy))
            {
                targetPawn.genes?.AddGene(JJKDefOf.Gene_JJKCursedEnergy, true);
            }       
        }


        public static IEnumerable<Pawn> GetEnemyPawnsInRange(IntVec3 center, Map map, float radius)
        {
            return GenRadial.RadialCellsAround(center, radius, true)
                .SelectMany(c => c.GetThingList(map))
                .OfType<Pawn>()
                .Where(p => p.Faction != null && p.Faction != Faction.OfPlayer);
        }

        public static void TransferGenes(Pawn sourcePawn, Pawn targetPawn, GeneDef GeneDefToTransfer, bool IsXenogerm = true)
        {
            if (sourcePawn == null || targetPawn == null || sourcePawn.genes == null || targetPawn.genes == null) return;

            if (sourcePawn.genes.HasActiveGene(GeneDefToTransfer))
            {
                if (!targetPawn.genes.HasActiveGene(GeneDefToTransfer))
                {
                    Gene SourceGene = sourcePawn.genes.GetGene(GeneDefToTransfer);
                    sourcePawn.genes.RemoveGene(SourceGene);
                    targetPawn.genes.AddGene(GeneDefToTransfer, IsXenogerm);

                }
            }
        }

        public static void RemoveViolenceIncapability(Pawn pawn)
        {
            //hopefully remove all traits that stop a being violent.
            List<Trait> traitsToRemove = new List<Trait>();
            foreach (Trait trait in pawn.story.traits.allTraits)
            {
                if ((trait.def.disabledWorkTags & WorkTags.Violent) != 0)
                {
                    traitsToRemove.Add(trait);
                }
            }

            foreach (Trait trait in traitsToRemove)
            {
                pawn.story.traits.allTraits.Remove(trait);
            }

            // same for genes.
            if (pawn.genes != null)
            {
                List<Gene> genesToRemove = new List<Gene>();
                foreach (Gene gene in pawn.genes.GenesListForReading)
                {
                    if ((gene.def.disabledWorkTags & WorkTags.Violent) != 0)
                    {
                        genesToRemove.Add(gene);
                    }
                }

                foreach (Gene gene in genesToRemove)
                {
                    pawn.genes.RemoveGene(gene);
                }
            }

            
            pawn.workSettings?.Notify_DisabledWorkTypesChanged();

            // ooooh yeah reflection baby, forces the cache clear after the chicanery above.
            typeof(Pawn_StoryTracker).GetField("cachedDisabledWorkTagsBackstoryAndTraits", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(pawn.story, null);
            typeof(Pawn_StoryTracker).GetField("cachedDisabledWorkTagsBackstoryTraitsAndGenes", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(pawn.story, null);
        }
        public static void TransferAbilities(Pawn sourcePawn, Pawn targetPawn)
        {
            if (sourcePawn.abilities == null || targetPawn.abilities == null)
            {
                Log.Error("AbilityTransferUtility: One or both pawns do not have an AbilityTracker.");
                return;
            }


            // Transfer each ability from the source pawn to the target pawn
            foreach (Ability sourceAbility in sourcePawn.abilities.abilities)
            {
                //only add abilities that they dont have.
                if (targetPawn.abilities.GetAbility(sourceAbility.def) == null)
                {
                    targetPawn.abilities.GainAbility(sourceAbility.def);

                    Ability newAbility = targetPawn.abilities.GetAbility(sourceAbility.def);

                    if (newAbility != null)
                    {
                        if (sourceAbility.CooldownTicksRemaining > 0)
                        {
                            FieldInfo cooldownTicksField = typeof(Ability).GetField("cooldownTicks", BindingFlags.NonPublic | BindingFlags.Instance);
                            cooldownTicksField?.SetValue(newAbility, sourceAbility.CooldownTicksRemaining);
                        }
                    }
                }
            }
        }

        public static ZanpaktoWeapon GetZanpaktoWeapon(this Pawn pawn)
        {
            if (pawn.equipment?.Primary is ZanpaktoWeapon zanpaktoWeapon)
            {
                return zanpaktoWeapon;
            }

            return null;
        }

        public static bool IsLimitlessUser(this Pawn pawn)
        {
            if (pawn?.genes == null)
            {
                return false;
            }

            return pawn.genes.HasActiveGene(JJKDefOf.Gene_JJKLimitless);
        }
        public static bool IsImmuneToDomainSureHit(this Pawn pawn)
        {
            return pawn.health.hediffSet.HasHediff(JJKDefOf.JJK_HollowWickerBasket) || pawn.health.hediffSet.HasHediff(JJKDefOf.JJK_SimpleShadowDomain);
        }
        public static bool HasSixEyes(this Pawn pawn)
        {
            if (pawn?.genes == null)
            {
                return false;
            }

            return pawn.genes.HasActiveGene(JJKDefOf.Gene_JJKSixEyes);
        }
        public static bool HasAbility(this Pawn pawn, AbilityDef AbiityDef)
        {
            if (pawn?.abilities == null)
            {
                return false;
            }

            return pawn.abilities.AllAbilitiesForReading.Find(x=> x.def == AbiityDef) != null;
        }

        public static Gene_CursedEnergy GetCursedEnergy(this Pawn pawn)
        {
            return pawn.genes?.GetFirstGeneOfType<Gene_CursedEnergy>();
        }
        public static Gene_Kenjaku GetKenjakuGene(this Pawn pawn)
        {
            return pawn.genes?.GetFirstGeneOfType<Gene_Kenjaku>();
        }

        public static void ApplyRCTHediffTo(this Pawn pawn)
        {
            Hediff NewRCT = HediffMaker.MakeHediff(JJK.JJKDefOf.JJK_RCTRegenHediff, pawn);

            // Get the brain part
            BodyPartRecord brainPart = pawn.health.hediffSet.GetBrain();

            // If brain is not found, try to get the head
            if (brainPart == null)
            {
                brainPart = pawn.health.hediffSet.GetNotMissingParts()
                    .FirstOrDefault(x => x.def == BodyPartDefOf.Head);
            }

            // Add the hediff to the brain or head
            if (brainPart != null)
            {
                pawn.health.AddHediff(NewRCT, brainPart);
            }
            else
            {
                Messages.Message("RCT cannot be applied to " + pawn.LabelShort + ". RCT stems from the brain, which is missing.",
                      pawn, MessageTypeDefOf.NegativeEvent);
            }
        }

        public static bool HasRCTActive(this Pawn pawn)
        {
            Hediff CurrentRCTHediff = pawn.health.hediffSet.GetFirstHediffOfDef(JJK.JJKDefOf.JJK_RCTRegenHediff);
            return CurrentRCTHediff != null;
        }

        public static void RemoveRCTHediff(this Pawn pawn)
        {
            Hediff CurrentRCTHediff = pawn.health.hediffSet.GetFirstHediffOfDef(JJK.JJKDefOf.JJK_RCTRegenHediff);
            if (CurrentRCTHediff != null)
            {
                pawn.health.RemoveHediff(CurrentRCTHediff);
            }
        }



        public static Thing CreateDollFromPawn(Pawn TargetPawn)
        {
            if (TargetPawn == null)
            {
                Log.Error("Can't create doll from Pawn : TargetPawn is null.");
                return null;
            }

            Thing NewItem = ThingMaker.MakeThing(JJKDefOf.JJK_idleTransfigurationDoll);        
            CompStoredPawn compStoredPawn = NewItem.TryGetComp<CompStoredPawn>();
            if (compStoredPawn != null)
            {
                compStoredPawn.StorePawn(TargetPawn);
            }
            else
            {
                Log.Error("CompStoredPawn not found on JJK_idleTransfigurationDoll. Make sure it's defined in the ThingDef.");
                return null;
            }


            return NewItem;
        }


        /// <summary>
        /// Returns a minimum of 0.5 if the target has more cursed energy than the caster, 
        /// </summary>
        /// <param name="casterPawn"></param>
        /// <param name="targetPawn"></param>
        /// <returns></returns>
        public static float CalcCursedEnergyScalingFactor(Pawn casterPawn, Pawn targetPawn, float min = 0.5f, float max = 1.5f)
        {
            float casterCursedEnergyReserves = casterPawn.GetStatValue(JJKDefOf.JJK_CursedEnergy);
            float targetCursedEnergyReserves = targetPawn.GetStatValue(JJKDefOf.JJK_CursedEnergy);
            return Mathf.Lerp(min, max, casterCursedEnergyReserves / targetCursedEnergyReserves);
        }

        public static BodyPartRecord GetRandomLimb(Pawn pawn)
        {
            List<BodyPartRecord> limbs = pawn.health.hediffSet.GetNotMissingParts()
                .Where(part => part.def.tags.Contains(BodyPartTagDefOf.MovingLimbCore))
                .ToList();

            return limbs.RandomElementWithFallback();
        }
    }



}

