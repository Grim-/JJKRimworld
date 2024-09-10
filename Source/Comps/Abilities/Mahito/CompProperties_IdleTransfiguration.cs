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

            //JJKUtility.SummonedCreatureManager.RegisterSummon(FleshBeast, parent.pawn);

            FleshBeast.health.AddHediff(JJKDefOf.JJK_IdleTransfigurationBeastStatBoost);
            FleshBeast.SetFaction(this.parent.pawn.Faction, this.parent.pawn);
            //CompAbilityEffect_GiveMentalState.TryGiveMentalState(JJKDefOf.TransfiguredState_Murderous, FleshBeast, this.parent.def, null, this.parent.pawn, true);
            parent.pawn.GetCursedEnergy()?.ConsumeCursedEnergy(Cost);
            
        }
    }


}