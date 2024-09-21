using JJK;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace JJK
{
    public class CompProperties_SummonCreature : CompProperties_CursedAbilityProps
    {
        public CompProperties_SummonCreature()
        {
            compClass = typeof(CompAbilityEffect_SummonCreature);
        }
    }

    public class CompAbilityEffect_SummonCreature : BaseCursedEnergyAbility
    {
        public new CompProperties_SummonCreature Props => (CompProperties_SummonCreature)props;

        public override void ApplyAbility(LocalTargetInfo target, LocalTargetInfo dest)
        {
            // This method is empty as the functionality is handled by button gizmos
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            Hediff_CursedSpiritManipulator hediff = parent.pawn.GetCursedSpiritManipulator();
            if (hediff == null)
            {
                Log.Error($"JJK: No Hediff_CursedSpiritManipulator found for {parent.pawn.LabelShort} {parent.pawn.ThingID}");
                yield break;
            }

            List<PawnKindDef> absorbedCreatures = hediff.GetAbsorbedCreatures();
            if (absorbedCreatures.Count == 0)
            {
                yield break;
            }

            var options = new List<Gizmo_MultiOption>();
            foreach (var creature in absorbedCreatures)
            {
                options.Add(new Gizmo_MultiOption(
                    creature.defName,
                    creature.race.uiIcon,
                    () => ToggleSummon(hediff, creature),
                    () => DeleteAbsorbedCreature(hediff, creature)
                ));
            }

            yield return new Gizmo_MultiImageButton(options);
        }

        private void ToggleSummon(Hediff_CursedSpiritManipulator hediff, PawnKindDef creature)
        {
            if (hediff.IsCreatureActive(creature))
            {
                UnsummonCreature(hediff, creature);
            }
            else
            {
                SummonCreature(hediff, creature);
            }
        }

        private void UnsummonCreature(Hediff_CursedSpiritManipulator hediff, PawnKindDef creature)
        {
            Pawn activeSummon = hediff.GetActiveSummonOfKind(creature);
            if (activeSummon != null)
            {
                if (activeSummon.DeadOrDowned || activeSummon.Destroyed)
                {
                    hediff.RemoveSummon(activeSummon, true);
                }
                else
                {
                    if (hediff.UnsummonCreature(activeSummon))
                    {
                        Messages.Message($"{parent.pawn.LabelShort} has unsummoned {activeSummon.LabelShort}.", MessageTypeDefOf.PositiveEvent);
                    }
                }

            }
        }

        private void SummonCreature(Hediff_CursedSpiritManipulator hediff, PawnKindDef creature)
        {
            if (hediff.SummonCreature(creature))
            {
                parent.pawn.GetCursedEnergy()?.ConsumeCursedEnergy(Props.cursedEnergyCost);
                Messages.Message($"{parent.pawn.LabelShort} has summoned a {creature.label}.", MessageTypeDefOf.PositiveEvent);
            }
        }

        private void DeleteAbsorbedCreature(Hediff_CursedSpiritManipulator hediff, PawnKindDef creature)
        {
            if (hediff.HasAbsorbedCreatureKind(creature))
            {
                if (hediff.IsCreatureActive(creature))
                {
                   Pawn activeSummon = hediff.GetActiveSummonOfKind(creature);
                    if (activeSummon != null)
                    {
                        hediff.UnsummonCreature(activeSummon);
                    }
                }

                hediff.DeleteAbsorbedCreature(creature);
                Messages.Message($"{parent.pawn.LabelShort} has forgotten how to summon {creature.label}.", MessageTypeDefOf.NeutralEvent);
            }
        }
    }
}
