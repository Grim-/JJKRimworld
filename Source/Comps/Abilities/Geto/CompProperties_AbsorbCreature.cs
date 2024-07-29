using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace JJK
{
    public class CompProperties_AbsorbCreature : CompProperties_CursedAbilityProps
    {
        public float BaseBodySizeCost = 25;

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

            AbsorbedCreatureManager AbsorbedCreatureManager = JJKUtility.AbsorbedCreatureManager;

            if (AbsorbedCreatureManager == null)
            {
                Messages.Message("Cannot absorb. Absorbed Creature Manager is null somehow, this should not happen.", MessageTypeDefOf.RejectInput);
                return;
            }

            AbsorbedData summoner = AbsorbedCreatureManager.GetAbsorbDataForPawn(parent.pawn);

            if (!summoner.CanAbsorbNewSummon())
            {
                Messages.Message("You can only absorb 5 creatures, you must delete one by right clicking its button.", MessageTypeDefOf.RejectInput);
                return;
            }

            Pawn caster = parent.pawn;
            Pawn targetPawn = target.Pawn;

            if (AbsorbedCreatureManager == null)
            {
                return;
            }

            if (!summoner.HasAbsorbedCreatureKind(targetPawn.kindDef))
            {
                Gene_CursedEnergy cursedEnergy = caster.GetCursedEnergy();
                float bodySizeFactor = targetPawn.BodySize;
                int ceCost = Mathf.RoundToInt(Props.BaseBodySizeCost * bodySizeFactor);

                if (cursedEnergy.Value >= ceCost)
                {
                    summoner.AbsorbCreature(targetPawn.kindDef);
                    targetPawn.Destroy();
                    parent.pawn.GetCursedEnergy()?.ConsumeCursedEnergy(ceCost);
                    Messages.Message($"{caster.LabelShort} has absorbed {targetPawn.LabelShort}.", MessageTypeDefOf.PositiveEvent);
                }
                else
                {
                    Messages.Message($"{caster.LabelShort} requires atleast {ceCost} Cursed Energy to absorb {targetPawn.LabelShort}. Base Cost {Props.BaseBodySizeCost} * {bodySizeFactor}", MessageTypeDefOf.NegativeEvent);
                }
            }
            else
            {
                //already has type
            }
        }
    }


}