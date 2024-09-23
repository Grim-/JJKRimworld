namespace JJK
{
    using RimWorld;
    using System.Collections.Generic;
    using Verse;

    public static class KenjakuUtil
    {
        private static Pawn possessedPawn;
        private static List<TraitDef> entityTraitDefs;

        public static void GrantPossessionAbility(Pawn pawn)
        {
            if (pawn.abilities.GetAbility(JJKDefOf.JJK_KenjakuPossess) == null)
            {
                pawn.abilities.GainAbility(JJKDefOf.JJK_KenjakuPossess);
            }
        }

        public static bool PossessPawn(Pawn currentPawn, Pawn targetPawn, List<TraitDef> Traits)
        {
            if (currentPawn.Faction != Faction.OfPlayer)
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
            ApplyPossession(currentPawn, targetPawn, Traits);
            possessedPawn = targetPawn;

            return true;
        }

        public static void ApplyPossession(Pawn currentPawn, Pawn targetPawn, List<TraitDef> Traits)
        {
            Log.Message("JJK: Entering ApplyPossession");

            if (currentPawn == null || targetPawn == null)
            {
                Log.Warning("ApplyPossession current pawn or target pawn is null");
                return;
            }

            targetPawn.SetFaction(Faction.OfPlayer);

            if (currentPawn.ideo?.Ideo != null && targetPawn.ideo != null)
            {
                targetPawn.ideo.SetIdeo(currentPawn.ideo.Ideo);
            }

            ApplyTraits(targetPawn, Traits);

            JJKUtility.RemoveViolenceIncapability(targetPawn);

            HediffComp_KenjakuPossession currentKenjaku = GetOrAddPossessionHediff(currentPawn);
            HediffComp_KenjakuPossession newKenjaku =  GetOrAddPossessionHediff(targetPawn);

            if (currentKenjaku.OriginalPawn != null)
            {
                newKenjaku.InitializeOriginalPawn(currentKenjaku.OriginalPawn);
                newKenjaku.AddPossessedPawn(currentPawn);
            }

            UpdateTargetName(currentPawn, targetPawn);

            JJKGeneUtil.GiveCursedEnergy(targetPawn);
            JJKUtility.TransferAbilities(currentPawn, targetPawn);
            JJKGeneUtil.TransferGenes(currentPawn, targetPawn, JJKDefOf.Gene_Kenjaku);
            JJKGeneUtil.TransferCursedEnergyGenes(currentPawn, targetPawn);
        }

        private static void ApplyTraits(Pawn targetPawn, List<TraitDef> traits)
        {
            if (traits != null)
            {
                entityTraitDefs = new List<TraitDef>(traits);

                foreach (TraitDef traitDef in entityTraitDefs)
                {
                    if (targetPawn.story?.traits != null && !targetPawn.story.traits.HasTrait(traitDef))
                    {
                        targetPawn.story.traits.GainTrait(new Trait(traitDef, 0));
                    }
                }
            }
        }
        public static HediffComp_KenjakuPossession GetOrAddPossessionHediff(Pawn targetPawn)
        {
            Hediff kenjakuHediff = targetPawn.health.GetOrAddHediff(JJKDefOf.JJK_KenjakuPossession);
            HediffComp_KenjakuPossession kenjakuComp = kenjakuHediff.TryGetComp<HediffComp_KenjakuPossession>();
            return kenjakuComp;
        }

        private static void UpdateTargetName(Pawn currentPawn, Pawn targetPawn)
        {
            targetPawn.story.birthLastName = currentPawn.story.birthLastName;
            NameTriple originalName = (NameTriple)targetPawn.Name;
            targetPawn.Name = new NameTriple(originalName.First, "Kenjaku", originalName.Last);
        }

        public static void RemovePossession(Pawn pawn)
        {
            pawn.SetFaction(null);
            if (entityTraitDefs != null)
            {
                foreach (TraitDef traitDef in entityTraitDefs)
                {
                    pawn.story.traits.allTraits.RemoveAll(t => t.def == traitDef);
                }
            }
            Hediff possessionHediff = pawn.health.hediffSet.GetFirstHediffOfDef(JJKDefOf.JJK_KenjakuPossession);
            if (possessionHediff != null)
            {
                pawn.health.RemoveHediff(possessionHediff);
            }
        }
    }
}
