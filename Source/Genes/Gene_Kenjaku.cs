using RimWorld;
using System.Collections.Generic;
using Verse;

namespace JJK
{
    using RimWorld;
    using System.Collections.Generic;
    using Verse;

    public class Gene_Kenjaku : Gene
    {
        public Faction entityFaction;
        public List<TraitDef> entityTraitDefs;
        public Pawn possessedPawn;

        public new KenjakuGeneDef def => base.def as KenjakuGeneDef;

        public override void PostAdd()
        {
            base.PostAdd();
            GrantPossessionAbility();
        }

        private void GrantPossessionAbility()
        {
            if (pawn.abilities.GetAbility(JJKDefOf.JJK_KenjakuPosess) == null)
            {
                pawn.abilities.GainAbility(JJKDefOf.JJK_KenjakuPosess);
            } 
        }

        public bool PossessPawn(Pawn CurrentPawn, Pawn targetPawn)
        {
            if (CurrentPawn.Faction != Faction.OfPlayer)
            {
                Log.Warning("Attempted to use Kenjaku possession ability with a non-player pawn. This is not allowed.");
                return false;
            }

            if (targetPawn == null)
            {
                Log.Warning("Attempted to use Kenjaku possession ability with a null target. This is not allowed.");
                return false;
            }

            if (possessedPawn != null)
            {
                RemovePossession(possessedPawn);
            }
            ApplyPossession(CurrentPawn, targetPawn);
            possessedPawn = targetPawn;

            return true;
        }
        private void ApplyPossession(Pawn CurrentPawn, Pawn TargetPawn)
        {
            Log.Message("JJK: Entering ApplyPossession");

            if (CurrentPawn == null)
            {
                Log.Warning("ApplyPossession current pawn null");
                return;
            }
            if (TargetPawn == null)
            {
                Log.Warning("ApplyPossession target pawn null");
                return;
            }

            TargetPawn.SetFaction(Faction.OfPlayer);

            if (CurrentPawn.ideo?.Ideo != null)
            {
                if (TargetPawn.ideo != null)
                {
                    TargetPawn.ideo.SetIdeo(CurrentPawn.ideo.Ideo);
                }
                else
                {
                    Log.Warning($"JJK: TargetPawn {TargetPawn.LabelShort} ideo is null");
                }
            }


            if (def != null && def.guaranteedTraits != null)
            {
                entityTraitDefs = new List<TraitDef>(def.guaranteedTraits);

                foreach (TraitDef traitDef in entityTraitDefs)
                {
                    if (TargetPawn.story != null && TargetPawn.story.traits != null)
                    {
                        if (!TargetPawn.story.traits.HasTrait(traitDef))
                        {
                            TargetPawn.story.traits.GainTrait(new Trait(traitDef, 0));
                        }
                    }
                    else
                    {
                        Log.Warning($"JJK: TargetPawn {TargetPawn.LabelShort} story or traits is null");
                        break;
                    }
                }
            }



            //Log.Message("JJK: Removing violence incapability");
            JJKUtility.RemoveViolenceIncapability(TargetPawn);

           // Log.Message("JJK: Adding Kenjaku possession hediff");
            if (JJKDefOf.JJK_KenjakuPossesion != null)
            {
                if (TargetPawn.health != null)
                {
                    TargetPawn.health.AddHediff(JJKDefOf.JJK_KenjakuPossesion);
                }
                else
                {
                   // Log.Warning($"JJK: TargetPawn {TargetPawn.LabelShort} health is null");
                }
            }
            else
            {
               // Log.Warning("JJK: JJK_KenjakuPossesion is null");
            }

            TargetPawn.story.birthLastName = CurrentPawn.story.birthLastName;

            NameTriple originalName = (NameTriple)TargetPawn.Name;

            TargetPawn.Name = new NameTriple(
                originalName.First,
                originalName.Nick,
                originalName.Last
            );

            // Log.Message("JJK: Transferring genes and abilities");
            JJKUtility.GiveCursedEnergy(TargetPawn);
            JJKUtility.TransferGenes(CurrentPawn, TargetPawn, JJKDefOf.Gene_Kenjaku);
            JJKUtility.TransferAbilities(CurrentPawn, TargetPawn);
            JJKUtility.TransferCursedEnergyGenes(CurrentPawn, TargetPawn);
        }

        private void RemovePossession(Pawn pawn)
        {
            pawn.SetFaction(null);
            foreach (TraitDef traitDef in entityTraitDefs)
            {
                pawn.story.traits.allTraits.RemoveAll(t => t.def == traitDef);
            }
            Hediff possessionHediff = pawn.health.hediffSet.GetFirstHediffOfDef(def.possessionHediff);
            if (possessionHediff != null)
            {
                pawn.health.RemoveHediff(possessionHediff);
            }
        }

   
    }


    public class KenjakuGeneDef : GeneDef
    {
        public HediffDef possessionHediff;
        public List<TraitDef> guaranteedTraits;
        public List<TraitDef> randomTraits;
        public float randomTraitChance = 0.5f;
    }
}
