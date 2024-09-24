using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace JJK
{
    public class CompProperties_IdleTransfiguration : CompProperties_CursedAbilityProps
    {
        public float CostPerMass = 1.2f;
        public List<HediffDef> hediffsToApply;


        public CompProperties_IdleTransfiguration()
        {
            compClass = typeof(CompAbilityEffect_IdleTransfiguration);
        }
    }


    public class CompAbilityEffect_IdleTransfiguration : BaseCursedEnergyAbility
    {
        public new CompProperties_IdleTransfiguration Props => (CompProperties_IdleTransfiguration)this.props;

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

            if (Props.hediffsToApply != null)
            {
                foreach (var hediff in Props.hediffsToApply)
                {
                    if (hediff == null)
                    {
                        continue;
                    }

                    FleshBeast.health.AddHediff(hediff);
                }
            }


            JJKUtility.TrainPawn(FleshBeast, TargetPawn);
            DraftingUtility.MakeDraftable(FleshBeast);

            FleshBeast.SetFaction(this.parent.pawn.Faction, this.parent.pawn);
            parent.pawn.GetCursedEnergy()?.ConsumeCursedEnergy(Cost);      
        }
    }
}