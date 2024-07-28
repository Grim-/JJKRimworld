using RimWorld;
using System.Linq;
using Verse;

namespace JJK
{
    public class CompProperties_KenjakuPossess : CompProperties_CursedAbilityProps
    {
        public float CursedEnergyCost = 50f; // Adjust as needed

        public CompProperties_KenjakuPossess()
        {
            compClass = typeof(CompAbilityEffect_KenjakuPossess);
        }
    }

    public class CompAbilityEffect_KenjakuPossess : BaseCursedEnergyAbility
    {
        public new CompProperties_KenjakuPossess Props => (CompProperties_KenjakuPossess)props;

        public override void ApplyAbility(LocalTargetInfo target, LocalTargetInfo dest)
        {
            Pawn targetPawn = null;

            if (target.Thing is Corpse corpse)
            {
                targetPawn = corpse.InnerPawn;
            }
            else if (target.Thing is Pawn pawn)
            {
                targetPawn = pawn;
            }

            if (targetPawn == null)
            {
                Log.Error("JJK: Target pawn is null");
                return;
            }

            if (targetPawn == parent.pawn)
            {
                Messages.Message("Cannot possess yourself", MessageTypeDefOf.NegativeEvent);
                return;
            }

            if (!targetPawn.Dead)
            {
                Messages.Message("Cannot posses pawns that are not dead.", MessageTypeDefOf.NegativeEvent);
                return;
            }

            if (!targetPawn.RaceProps.Humanlike)
            {
                Messages.Message("Cannot posses non-humans", MessageTypeDefOf.NegativeEvent);
                return;
            }

            if (targetPawn.genes.HasActiveGene(JJKDefOf.Gene_Kenjaku))
            {
                Messages.Message("Cannot posses another with Kenjaku Gene", MessageTypeDefOf.NegativeEvent);
                return;
            }


            if (ResurrectionUtility.TryResurrect(targetPawn, new ResurrectionParams()
            {
                removeDiedThoughts = true
            }))
            {

                Pawn caster = parent.pawn;
                //Pawn targetPawn = target.Pawn;

                // Detailed debug logging
                Log.Message($"JJK: Attempting possession. Caster: {caster.LabelShort}, Target: {targetPawn.LabelShort}");
                Log.Message($"JJK: Caster genes: {string.Join(", ", caster.genes.GenesListForReading.Select(g => g.def.defName))}");

                // Check for Cursed Energy gene (as a comparison)
                var cursedEnergyGene = caster.GetCursedEnergy();
                Log.Message($"JJK: Cursed Energy Gene found: {cursedEnergyGene != null}");

                // Check if the caster has the Gene_Kenjaku
                Gene_Kenjaku kenjakuGene = caster.GetKenjakuGene();
                Log.Message($"JJK: Kenjaku Gene found: {kenjakuGene != null}");

                // Additional checks
                var allGenes = caster.genes.GenesListForReading;
                Log.Message($"JJK: Total genes on caster: {allGenes.Count}");
                var kenjakuGeneFromList = allGenes.FirstOrDefault(g => g is Gene_Kenjaku);
                Log.Message($"JJK: Kenjaku Gene found in list: {kenjakuGeneFromList != null}");

                if (kenjakuGene == null)
                {
                    Log.Error($"JJK: Kenjaku Gene not found on caster {caster.LabelShort}");
                    Messages.Message("JJK_KenjakuPossessionFailed_NoGene".Translate(caster.LabelShort), MessageTypeDefOf.RejectInput);
                    return;
                }



                //Log.Message("JJK: Kenjaku Gene found, attempting possession");

                // Attempt possession
                kenjakuGene.PossessPawn(caster, targetPawn);

                // Kill the previous host (the caster)
                caster.Kill(new DamageInfo(DamageDefOf.NerveStun, 9999, 1));

                Messages.Message("JJK_KenjakuPossessionSuccessful".Translate(caster.LabelShort, targetPawn.LabelShort), MessageTypeDefOf.PositiveEvent);
            }
            else
            {
                Log.Error($"JJK: Kenjaku Gene could not ressurct target for possesion {target.Pawn.Label}");
            }


        }
    }
}