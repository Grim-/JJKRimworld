using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace JJK
{
    //public class CompProperties_AbsorbCreature : CompProperties_CursedAbilityProps
    //{
    //    public float BaseBodySizeCost = 25;

    //    public CompProperties_AbsorbCreature()
    //    {
    //        compClass = typeof(CompAbilityEffect_AbsorbCreature);
    //    }
    //}

    //public class CompAbilityEffect_AbsorbCreature : BaseCursedEnergyAbility
    //{
    //    public new CompProperties_AbsorbCreature Props => (CompProperties_AbsorbCreature)props;

    //    public override void ApplyAbility(LocalTargetInfo target, LocalTargetInfo dest)
    //    {
    //        if (target.Pawn == null || !target.Pawn.Downed)
    //        {
    //            Messages.Message("Cannot absorb. Target must be a downed creature.", MessageTypeDefOf.RejectInput);
    //            return;
    //        }

    //        if (target.Pawn.RaceProps.Humanlike)
    //        {
    //            Messages.Message("Cannot absorb. Target must be a downed creature.", MessageTypeDefOf.RejectInput);
    //            return;
    //        }


    //        Hediff_CursedSpiritManipulator summonHediff = null;


    //        if (parent.pawn.health.hediffSet.GetFirstHediffOfDef(JJKDefOf.JJK_CursedSpiritManipulator) != null && target.Pawn.health.hediffSet.GetFirstHediffOfDef(JJKDefOf.JJK_CursedSpiritManipulator) is Hediff_CursedSpiritManipulator SummonerHediff)
    //        {
    //            summonHediff = SummonerHediff;
    //        }


    //        if (!summonHediff.CanAsborbNewSummon(target.Pawn.kindDef))
    //        {
    //            Messages.Message("You can only absorb 5 creatures, you must delete one by right clicking its button.", MessageTypeDefOf.RejectInput);
    //            return;
    //        }

    //        Pawn caster = parent.pawn;
    //        Pawn targetPawn = target.Pawn;


    //        if (!summonHediff.HasAbsorbedCreatureKind(targetPawn.kindDef))
    //        {
    //            Gene_CursedEnergy cursedEnergy = caster.GetCursedEnergy();
    //            float bodySizeFactor = targetPawn.BodySize;
    //            int ceCost = Mathf.RoundToInt(Props.BaseBodySizeCost * bodySizeFactor);

    //            if (cursedEnergy.Value >= ceCost)
    //            {
    //                summonHediff.AbsorbCreature(targetPawn.kindDef);
    //                targetPawn.Destroy();
    //                parent.pawn.GetCursedEnergy()?.ConsumeCursedEnergy(ceCost);
    //                Messages.Message($"{caster.LabelShort} has absorbed {targetPawn.LabelShort}.", MessageTypeDefOf.PositiveEvent);
    //            }
    //            else
    //            {
    //                Messages.Message($"{caster.LabelShort} requires atleast {ceCost} Cursed Energy to absorb {targetPawn.LabelShort}. Base Cost {Props.BaseBodySizeCost} * {bodySizeFactor}", MessageTypeDefOf.NegativeEvent);
    //            }
    //        }
    //        else
    //        {
    //            //already has type
    //        }
    //    }
    //}

    public class CompProperties_AbsorbCreature : CompProperties_CursedAbilityProps
    {
        public float BaseBodySizeCost = 25f;
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
            if (!parent.pawn.IsCursedSpiritManipulator())
            {
                Messages.Message("Requires Geto Gene.", MessageTypeDefOf.RejectInput);
                return;
            }

            if (target.Pawn == null || !target.Pawn.Downed || target.Pawn.RaceProps.Humanlike)
            {
                Messages.Message("Cannot absorb. Target must be a downed, non-humanlike creature.", MessageTypeDefOf.RejectInput);
                return;
            }

            Hediff_CursedSpiritManipulator hediff = parent.pawn.GetCursedSpiritManipulator();
            if (hediff == null)
            {
                Messages.Message("Caster does not have the Cursed Spirit Manipulator ability.", MessageTypeDefOf.RejectInput);
                return;
            }

            Pawn targetPawn = target.Pawn;
            if (!hediff.CanAbsorbNewSummon(targetPawn.kindDef))
            {
                Messages.Message("You can only absorb 5 different types of creatures. You must delete one before absorbing a new type.", MessageTypeDefOf.RejectInput);
                return;
            }

            Gene_CursedEnergy cursedEnergy = parent.pawn.GetCursedEnergy();
            float bodySizeFactor = targetPawn.BodySize;
            int ceCost = Mathf.RoundToInt(Props.BaseBodySizeCost * bodySizeFactor);

            if (cursedEnergy.Value >= ceCost)
            {
                hediff.AbsorbCreature(targetPawn.kindDef, targetPawn);
                targetPawn.Destroy();
                cursedEnergy.ConsumeCursedEnergy(ceCost);
                Messages.Message($"{parent.pawn.LabelShort} has absorbed {targetPawn.LabelShort}.", MessageTypeDefOf.PositiveEvent);
            }
            else
            {
                Messages.Message($"{parent.pawn.LabelShort} requires at least {ceCost} Cursed Energy to absorb {targetPawn.LabelShort}. (Base Cost {Props.BaseBodySizeCost} * {bodySizeFactor})", MessageTypeDefOf.NegativeEvent);
            }
        }
    }
}