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
            InitializeEntity();
            GrantPossessionAbility();
        }

        private void InitializeEntity()
        {
            entityFaction = FactionUtility.DefaultFactionFrom(FactionDefOf.PlayerColony);
            entityTraitDefs = new List<TraitDef>(def.guaranteedTraits);

            foreach (TraitDef randomTrait in def.randomTraits)
            {
                if (Rand.Chance(def.randomTraitChance))
                {
                    entityTraitDefs.Add(randomTrait);
                }
            }
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
            TargetPawn.SetFaction(entityFaction);
            TargetPawn.ideo.SetIdeo(CurrentPawn.ideo.Ideo);

            foreach (TraitDef traitDef in entityTraitDefs)
            {
                if (!TargetPawn.story.traits.HasTrait(traitDef))
                {
                    TargetPawn.story.traits.GainTrait(new Trait(traitDef, 0));
                }
            }

            RemoveViolenceIncapability(TargetPawn);

            TargetPawn.genes.AddGene(JJKDefOf.Gene_JJKCursedEnergy, true);
            TargetPawn.genes.AddGene(JJKDefOf.Gene_Kenjaku, true);
            TargetPawn.health.AddHediff(def.possessionHediff);
            //pawn extension, in JJKUtility

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

        private void RemoveViolenceIncapability(Pawn pawn)
        {
            // Remove traits that disable violent work
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

            // Remove any genes that might be disabling violent work
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

            // Recache the pawn's work settings
            pawn.workSettings?.Notify_DisabledWorkTypesChanged();

            // Reset the cache for the story's work tags
            typeof(Pawn_StoryTracker).GetField("cachedDisabledWorkTagsBackstoryAndTraits", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(pawn.story, null);
            typeof(Pawn_StoryTracker).GetField("cachedDisabledWorkTagsBackstoryTraitsAndGenes", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(pawn.story, null);
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
