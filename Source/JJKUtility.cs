using JetBrains.Annotations;
using LudeonTK;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;
using Verse.AI.Group;

namespace JJK
{
    public class CursedEnergyGeneExtension : DefModExtension
    {
        public int priority = 0;
    }
    public static class JJKUtility
    {
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


        public static bool IsLimitlessUser(this Pawn pawn)
        {
            if (pawn?.genes == null)
            {
                // If pawn or its genes are null, it cannot be a blue user
                return false;
            }

            return pawn.genes.HasActiveGene(JJKDefOf.Gene_JJKLimitless);
        }

        public static bool HasSixEyes(this Pawn pawn)
        {
            if (pawn?.genes == null)
            {
                // If pawn or its genes are null, it cannot be a blue user
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
    }

    public static class ZombieUtility
    {
        public static List<WorkTypeDef> WantedWorkTypes = new List<WorkTypeDef>()
        {
            WorkTypeDefOf.Mining,
            WorkTypeDefOf.Growing,
            WorkTypeDefOf.Construction,
            WorkTypeDefOf.Hunting,
            WorkTypeDefOf.Hauling,
            WorkTypeDefOf.Cleaning,
            WorkTypeDefOf.PlantCutting,
            WorkTypeDefOf.Smithing

        };


        public static void ForceIntoZombieSlave(Pawn Pawn, Pawn Slaver, int SkillLevels = 5, BackstoryDef ChildHoodBackStoryOverWrite = null, BackstoryDef AdultHoodBackStoryOverwrite = null)
        {
            if (Pawn == null)
            {
                return;
            }

            if (Slaver == null)
            {
                return;
            }

            Pawn.story.Childhood = ChildHoodBackStoryOverWrite == null ? JJKDefOf.ZombieChildhoodStory : ChildHoodBackStoryOverWrite;
            Pawn.story.Adulthood = AdultHoodBackStoryOverwrite == null ? JJKDefOf.ZombieAdulthoodStory : AdultHoodBackStoryOverwrite;


            Pawn.jobs.ClearQueuedJobs(false);
            Pawn.guest.SetGuestStatus(Slaver.Faction, GuestStatus.Slave);

            SetSkillLevels(Pawn, SkillLevels);

            foreach (var item in Pawn.story.traits.allTraits.ToList())
            {
                Pawn.story.traits.RemoveTrait(item);
            }

            SetWorkTypePriority(Pawn, WorkTypeDefOf.Mining, Pawn_WorkSettings.DefaultPriority);
            SetWorkTypePriority(Pawn, WorkTypeDefOf.Growing, Pawn_WorkSettings.DefaultPriority);
            SetWorkTypePriority(Pawn, WorkTypeDefOf.Construction, Pawn_WorkSettings.DefaultPriority);
            SetWorkTypePriority(Pawn, WorkTypeDefOf.Handling, Pawn_WorkSettings.DefaultPriority);
            SetWorkTypePriority(Pawn, WorkTypeDefOf.Crafting, Pawn_WorkSettings.DefaultPriority);
            SetWorkTypePriority(Pawn, WorkTypeDefOf.Hauling, Pawn_WorkSettings.DefaultPriority);
            SetWorkTypePriority(Pawn, WorkTypeDefOf.Cleaning, Pawn_WorkSettings.DefaultPriority);
            SetWorkTypePriority(Pawn, WorkTypeDefOf.PlantCutting, Pawn_WorkSettings.DefaultPriority);
            SetWorkTypePriority(Pawn, WorkTypeDefOf.Smithing, Pawn_WorkSettings.DefaultPriority);

            SetWorkTypePriority(Pawn, WorkTypeDefOf.Childcare, 0);
            SetWorkTypePriority(Pawn, WorkTypeDefOf.Warden, 0);
            SetWorkTypePriority(Pawn, WorkTypeDefOf.Research, 0);
            SetWorkTypePriority(Pawn, WorkTypeDefOf.DarkStudy, 0);
            SetWorkTypePriority(Pawn, WorkTypeDefOf.Firefighter, 0);
            SetWorkTypePriority(Pawn, WorkTypeDefOf.Handling, 0);
            SetWorkTypePriority(Pawn, WorkTypeDefOf.DarkStudy, 0);
            SetWorkTypePriority(Pawn, WorkTypeDefOf.Doctor, 0);
            SetWorkTypePriority(Pawn, WorkTypeDefOf.Hunting, 0);



            if (Slaver.GetLord() != null)
            {
                Slaver.GetLord().AddPawn(Pawn);
            }

            if (Pawn.playerSettings != null)
            {
                // Disable health care
                Pawn.playerSettings.medCare = MedicalCareCategory.NoCare;
            }


            if (Pawn.mindState != null)
            {
                // Modify behavior to focus on work
                Pawn.mindState.mentalStateHandler.Reset();
            }
        }

        public static void SetSkillLevels(Pawn pawn, int Level)
        {
            foreach (SkillRecord skill in pawn.skills.skills)
            {
                skill.Level = Level; // Set to a moderate skill level
                skill.passion = Passion.None;
            }
        }

        public static void SetWorkTypePriority(Pawn pawn, WorkTypeDef workType, int prio = 1)
        {
            if (workType != null)
            {
                int CurrentPrio = pawn.workSettings.GetPriority(workType);
                if (CurrentPrio > 0)
                {
                    pawn.workSettings.SetPriority(workType, prio);
                }
            }
        }
    }

    public static class PawnHealingUtility
    {
        public static bool RestoreMissingPart(Pawn target)
        {
            List<Hediff_MissingPart> missingParts = GetMissingPartsPrioritized(target);
            if (missingParts.Count > 0)
            {
                Hediff_MissingPart highestPrio = missingParts.First();
                HealthUtility.Cure(highestPrio);
                return true;
            }
            return false;
        }

        public static void HealHealthProblem(Pawn target)
        {
            Hediff problem = GetMostSevereHealthProblem(target);
            if (problem != null)
            {
                Hediff problemToHeal = target.health.hediffSet.hediffs
                  .Where(x => !(x is Hediff_MissingPart) && x.Visible && x.def != JJKDefOf.ZombieWorkSlaveHediff)
                  .OrderByDescending(x => x.Severity)
                  .FirstOrDefault();

                if (problemToHeal != null)
                {
                    HealthUtility.Cure(problemToHeal);
                }
            }
        }
        public static Hediff GetMostSevereHealthProblem(Pawn pawn)
        {
            return pawn.health.hediffSet.hediffs
                .Where(x => !(x is Hediff_MissingPart) && x.Visible)
                .OrderByDescending(x => x.Severity)
                .FirstOrDefault();
        }

        public static List<Hediff_MissingPart> GetMissingPartsPrioritized(Pawn pawn)
        {
            return pawn.health.hediffSet.hediffs
                .OfType<Hediff_MissingPart>()
                .OrderByDescending(x => x.Part.def.hitPoints)
                .ThenByDescending(x => x.Part.def.GetMaxHealth(pawn))
                .ToList();
        }
    }



}

