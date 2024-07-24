using RimWorld;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using Verse;

namespace JJK
{
    public class CompProperties_IdleTransfigurationShape : CompProperties_CursedAbilityProps
    {
        public float CostPerMass = 1.2f;
        [XmlElement("transfigurationOptions")]
        public List<TransfigurationOption> transfigurationOptions;

        public CompProperties_IdleTransfigurationShape()
        {
            compClass = typeof(CompAbilityEffect_IdleTransfigurationShape);

        }
    }

    public class CompAbilityEffect_IdleTransfigurationShape : BaseCursedEnergyAbility
    {
        public new CompProperties_IdleTransfigurationShape Props => (CompProperties_IdleTransfigurationShape)props;

        public override void ApplyAbility(LocalTargetInfo target, LocalTargetInfo dest)
        {
            if (target.Pawn == null) return;

            ApplyTransfiguration(parent.pawn, target.Pawn);
        }

        public void ApplyTransfiguration(Pawn caster, Pawn target)
        {
            if (target.health.hediffSet.HasHediff(JJKDefOf.JJK_IdleTransfigurationCooldown))
            {
                Messages.Message($"Cannot apply Idle Transfiguration to {target.LabelShort}. Still on cooldown.", MessageTypeDefOf.RejectInput);
                return;
            }

            if (Props.transfigurationOptions.NullOrEmpty())
            {
                Log.Error("No transfiguration options defined for Idle Transfiguration: Shape ability.");
                return;
            }

            TransfigurationOption randomOption = Props.transfigurationOptions.RandomElement();
            BodyPartRecord targetPart = target.RaceProps.body.GetPartsWithDef(randomOption.BodyPartDef).RandomElementWithFallback();

            if (targetPart != null)
            {
                Hediff existingPart = target.health.hediffSet.hediffs.FirstOrDefault(h => h.Part == targetPart);
                if (existingPart != null)
                {
                    target.health.RemoveHediff(existingPart);
                }

                Hediff newPart = HediffMaker.MakeHediff(randomOption.HediffDef, target, targetPart);
                target.health.AddHediff(newPart);

                Hediff cooldown = HediffMaker.MakeHediff(JJKDefOf.JJK_IdleTransfigurationCooldown, target);
                target.health.AddHediff(cooldown);

                Messages.Message($"{caster.LabelShort} has transfigured {target.LabelShort}'s {targetPart.Label} into a {newPart.Label}!", MessageTypeDefOf.PositiveEvent);
            }
            else
            {
                Messages.Message($"Failed to find a suitable body part on {target.LabelShort} for transfiguration.", MessageTypeDefOf.RejectInput);
            }
        }
    }
}