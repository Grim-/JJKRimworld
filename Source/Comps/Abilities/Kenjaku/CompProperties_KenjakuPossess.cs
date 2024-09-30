using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace JJK
{
    public class CompProperties_KenjakuPossess : CompProperties_CursedAbilityProps
    {
        public float CursedEnergyCost = 50f;
        public HediffDef possessionHediff;
        public List<TraitDef> guaranteedTraits;
        public List<TraitDef> randomTraits;
        public float randomTraitChance = 0.5f;

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
            if (target.Thing.GetType() != typeof(Corpse))
            {
                Log.Error("JJK: Target is not a corpse.");
                return;
            }

            Corpse targetCorpse = (Corpse)target.Thing;


            if (targetCorpse == null)
            {
                Log.Error("JJK: Target pawn is null");
                return;
            }

            if (targetCorpse.GetRotStage() != RotStage.Fresh)
            {
                Log.Error("JJK: Target corpse must be fresh.");
                return;
            }

            Pawn targetPawn = targetCorpse.InnerPawn;

            if (targetPawn == parent.pawn)
            {
                Messages.Message("Cannot possess yourself", MessageTypeDefOf.NegativeEvent);
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
                var cursedEnergyGene = caster.GetCursedEnergy();
                if (KenjakuUtil.PossessPawn(caster, targetPawn, Props.guaranteedTraits))
                {
                    // Kill the previous host (the caster)
                    caster.Kill(new DamageInfo(DamageDefOf.NerveStun, 9999, 1));

                    Messages.Message("JJK_KenjakuPossessionSuccessful".Translate(caster.LabelShort, targetPawn.LabelShort), MessageTypeDefOf.PositiveEvent);
                }


            }
            else
            {
                Log.Error($"JJK: Kenjaku Gene could not ressurct target for possesion {target.Pawn.Label}");
            }
        }
    }
}