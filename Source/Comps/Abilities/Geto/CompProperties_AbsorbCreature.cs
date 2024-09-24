using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace JJK
{
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

            if (!IsCursedSpirit(targetPawn))
            {
                Messages.Message("You can only absorb cursed spirits.", MessageTypeDefOf.RejectInput);
                return;
            }

            Pawn currentTargetMaster = targetPawn.GetMaster();

            if (currentTargetMaster != null && !currentTargetMaster.Dead)
            {
                Messages.Message("You cannot absorb spirits bound to a master.", MessageTypeDefOf.RejectInput);
                return;
            }


            if (!hediff.CanAbsorbNewCreature())
            {
                Messages.Message("You can only absorb 5 creatures. You must delete one before absorbing a new one.", MessageTypeDefOf.RejectInput);
                return;
            }

            Gene_CursedEnergy cursedEnergy = parent.pawn.GetCursedEnergy();
            float bodySizeFactor = targetPawn.BodySize;
            int ceCost = Mathf.RoundToInt(Props.BaseBodySizeCost * bodySizeFactor);

            if (cursedEnergy.Value >= ceCost)
            {
                Thing orb = ThingMaker.MakeThing(JJKDefOf.JJK_SealedCursedSpiritOrb);
                Comp_CursedSpiritOrb comp = orb.TryGetComp<Comp_CursedSpiritOrb>();
                if (comp != null)
                {
                    comp.StoreSpirit(targetPawn);
                    GenSpawn.Spawn(orb, parent.pawn.Position, parent.pawn.Map);
                    HealthUtility.HealNonPermanentInjuriesAndRestoreLegs(targetPawn);
                    targetPawn.DeSpawn();
                    cursedEnergy.ConsumeCursedEnergy(ceCost);
                    //Messages.Message($"{parent.pawn.LabelShort} has sealed {targetPawn.LabelShort} into an orb.", MessageTypeDefOf.PositiveEvent);
                }
                else
                {
                    Messages.Message("Failed to create Sealed Cursed Spirit Orb.", MessageTypeDefOf.NegativeEvent);
                }

            }
            else
            {
                Messages.Message($"{parent.pawn.LabelShort} requires at least {ceCost} Cursed Energy to absorb {targetPawn.LabelShort}. (Base Cost {Props.BaseBodySizeCost} * {bodySizeFactor})", MessageTypeDefOf.NegativeEvent);
            }
        }


        private bool IsCursedSpirit(Pawn Pawn)
        {
            CursedSpiritExtension cursedSpiritExtension = Pawn.kindDef.GetModExtension<CursedSpiritExtension>();
            return cursedSpiritExtension != null;
        }
    }
}