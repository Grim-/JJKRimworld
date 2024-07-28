using JetBrains.Annotations;
using LudeonTK;
using RimWorld;
using System;
using System.Linq;
using System.Reflection;
using Verse;

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

        public static void ApplyRCTHediffTo(this Pawn pawn)
        {
            Hediff NewRCT = HediffMaker.MakeHediff(JJK.JJKDefOf.RCTRegenHediff, pawn);

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
            Hediff CurrentRCTHediff = pawn.health.hediffSet.GetFirstHediffOfDef(JJK.JJKDefOf.RCTRegenHediff);
            return CurrentRCTHediff != null;
        }

        public static void RemoveRCTHediff(this Pawn pawn)
        {
            Hediff CurrentRCTHediff = pawn.health.hediffSet.GetFirstHediffOfDef(JJK.JJKDefOf.RCTRegenHediff);
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
    }



}

