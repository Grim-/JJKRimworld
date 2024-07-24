using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace JJK
{
    public class CompProperties_IdleTransfiguration : CompProperties_CursedAbilityProps
    {
        public float CostPerMass = 1.2f;



        public CompProperties_IdleTransfiguration()
        {
            compClass = typeof(CompAbilityEffect_IdleTransfiguration);
        }
    }


    public class CompAbilityEffect_IdleTransfiguration : BaseCursedEnergyAbility
    {
        public new CompProperties_IdleTransfiguration Props
        {
            get
            {
                return (CompProperties_IdleTransfiguration)this.props;
            }
        }

        public override bool AICanTargetNow(LocalTargetInfo target)
        {
            return false;
        }

        public override void ApplyAbility(LocalTargetInfo target, LocalTargetInfo dest)
        {
            if (target.Pawn == null)
            {
                return;
            }

            Pawn TargetPawn = target.Pawn;
            Pawn FleshBeast = FleshbeastUtility.SpawnFleshbeastFromPawn(TargetPawn, true, true, Array.Empty<PawnKindDef>());


            float PawnMass = target.Pawn.GetStatValue(StatDefOf.Mass);


            float Cost = PawnMass * Props.CostPerMass;


            FleshBeast.health.AddHediff(JJKDefOf.JJK_IdleTransfigurationBeastStatBoost);
            FleshBeast.SetFaction(this.parent.pawn.Faction, this.parent.pawn);
           // FleshBeast.training.SetWantedRecursive(TrainableDefOf.Release, true);
            CompAbilityEffect_GiveMentalState.TryGiveMentalState(JJKDefOf.TransfiguredState_Murderous, FleshBeast, this.parent.def, null, this.parent.pawn, true);
            parent.pawn.GetCursedEnergy()?.ConsumeCursedEnergy(parent.pawn, Cost);
            
        }
    }


    public class CompProperties_BlueSelfTeleport : CompProperties_CursedAbilityProps
    {
        public float MaxRange = 20f;

        public CompProperties_BlueSelfTeleport()
        {
            compClass = typeof(CompAbilityEffect_BlueSelfTeleport);
        }
    }

    public class CompAbilityEffect_BlueSelfTeleport : BaseCursedEnergyAbility
    {
        public new CompProperties_BlueSelfTeleport Props
        {
            get
            {
                return (CompProperties_BlueSelfTeleport)this.props;
            }
        }

        public override bool AICanTargetNow(LocalTargetInfo target)
        {
            return target.Cell.InBounds(parent.pawn.Map) && target.Cell.Walkable(parent.pawn.Map);
        }

        public override void ApplyAbility(LocalTargetInfo target, LocalTargetInfo dest)
        {
            if (!target.Cell.InBounds(parent.pawn.Map) || !target.Cell.Walkable(parent.pawn.Map))
            {
                return;
            }

            Pawn caster = parent.pawn;
            IntVec3 originalPosition = caster.Position;
            Map map = caster.Map;

            // Perform the teleportation
            caster.Position = target.Cell;
            caster.Notify_Teleported();

            // Visual effects (you can customize these)
            FleckMaker.ThrowDustPuff(originalPosition, map, 1.0f);
            FleckMaker.ThrowDustPuff(target.Cell, map, 1.0f);

            // Consume Cursed Energy
            float distanceTraveled = (target.Cell - originalPosition).LengthHorizontal;
            float energyCost = distanceTraveled * 0.01f; // Adjust this multiplier as needed
            caster.GetCursedEnergy()?.ConsumeCursedEnergy(caster, energyCost);
        }

        public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
        {
            return base.CanApplyOn(target, dest) &&
                   target.Cell.InBounds(parent.pawn.Map) &&
                   target.Cell.Walkable(parent.pawn.Map) &&
                   (target.Cell - parent.pawn.Position).LengthHorizontal <= Props.MaxRange;
        }
    }
}