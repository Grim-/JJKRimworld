using RimWorld;
using System.Collections.Generic;
using Verse;

namespace JJK
{
    public class CompProperties_AbsorbCreature : CompProperties_CursedAbilityProps
    {
        public CompProperties_AbsorbCreature()
        {
            compClass = typeof(CompAbilityEffect_AbsorbCreature);
        }
    }

    public class CompAbilityEffect_AbsorbCreature : BaseCursedEnergyAbility
    {
        public new CompProperties_AbsorbCreature Props => (CompProperties_AbsorbCreature)props;

        public override void ApplyAbility(LocalTargetInfo target, LocalTargetInfo dest)
        {
            if (target.Pawn == null || !target.Pawn.Downed)
            {
                Messages.Message("Cannot absorb. Target must be a downed creature.", MessageTypeDefOf.RejectInput);
                return;
            }

            if (target.Pawn.RaceProps.Humanlike)
            {
                Messages.Message("Cannot absorb. Target must be a downed creature.", MessageTypeDefOf.RejectInput);
                return;
            }

            AbsorbedCreatureManager AbsorbedCreatureManager = Find.World.GetComponent<AbsorbedCreatureManager>();

            Pawn caster = parent.pawn;
            Pawn targetPawn = target.Pawn;

            if (AbsorbedCreatureManager == null)
            {
                return;
            }

            if (!AbsorbedCreatureManager.HasAbsorbedCreatue(caster, targetPawn))
            {
                // Use the global manager to absorb the creature
                AbsorbedCreatureManager.AbsorbCreature(caster, targetPawn);
                // Remove the absorbed creature from the map
                targetPawn.Destroy();
                Messages.Message($"{caster.LabelShort} has absorbed {targetPawn.LabelShort}.", MessageTypeDefOf.PositiveEvent);
            }
        }
    }


}