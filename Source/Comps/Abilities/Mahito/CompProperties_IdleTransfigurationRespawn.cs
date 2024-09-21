using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace JJK
{
    public class CompProperties_IdleTransfigurationRespawn : CompProperties_CursedAbilityProps
    {
        public float CostPerMass = 0.05f; // Assuming it costs less to reverse the process
        public CompProperties_IdleTransfigurationRespawn()
        {
            compClass = typeof(CompAbilityEffect_IdleTransfigurationRespawn);
        }
    }


    public class CompAbilityEffect_IdleTransfigurationRespawn : BaseCursedEnergyAbility
    {
      public new CompProperties_IdleTransfigurationRespawn Props => (CompProperties_IdleTransfigurationRespawn)props;

        public override void ApplyAbility(LocalTargetInfo target, LocalTargetInfo dest)
        {
           // Log.Message(target.Thing);
            //Log.Message(dest.Thing);
            CompStoredPawn storedPawnComp = GetStoredPawnComp(target);

            if (storedPawnComp == null)
            {
                Messages.Message("No valid Idle Transfiguration Doll found.", MessageTypeDefOf.NegativeEvent);
                return;
            }

            Pawn storedPawn = storedPawnComp.ReleasePawn();
            if (storedPawn == null)
            {
                Messages.Message("Failed to retrieve stored pawn.", MessageTypeDefOf.NegativeEvent);
                return;
            }

            SpawnStoredPawn(storedPawn, target.Cell);
            ConsumeCursedEnergy(storedPawn);
            RemoveOrDecrementDoll(storedPawnComp.parent);

            Messages.Message($"{storedPawn.LabelShort} has been restored from transfiguration.", MessageTypeDefOf.PositiveEvent);
        }

        private CompStoredPawn GetStoredPawnComp(LocalTargetInfo target)
        {
            CompStoredPawn compOnThing = target.Cell.GetFirstThingWithComp<CompStoredPawn>(parent.pawn.MapHeld).TryGetComp<CompStoredPawn>();
           
            // Check if the target is a thing with CompStoredPawn
            if (compOnThing != null)
            {
                return compOnThing;
            }

            Thing inventoryDoll = parent.pawn.inventory.innerContainer
                .FirstOrDefault(thing => thing.def == JJKDefOf.JJK_idleTransfigurationDoll);

            if (inventoryDoll != null)
            {
                var comp = inventoryDoll.TryGetComp<CompStoredPawn>();
                if (comp != null && comp.Pawn != null)
                {
                    return comp;
                }
            }

            return null;
        }

        private void SpawnStoredPawn(Pawn storedPawn, IntVec3 cell)
        {
            GenSpawn.Spawn(storedPawn, cell, parent.pawn.Map);
            if (parent.pawn.Faction != null)
            {
                storedPawn.SetFaction(parent.pawn.Faction);
            }
        }

        private void ConsumeCursedEnergy(Pawn storedPawn)
        {
            float pawnMass = storedPawn.GetStatValue(StatDefOf.Mass);
            float cost = pawnMass * Props.CostPerMass;
            parent.pawn.GetCursedEnergy()?.ConsumeCursedEnergy(cost);
        }

        private void RemoveOrDecrementDoll(Thing doll)
        {
            if (doll.stackCount <= 1)
            {
                doll.holdingOwner?.Remove(doll);
                doll.Destroy();
            }
            else
            {
                doll.stackCount--;
            }
        }

        //public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
        //{
        //    return base.CanApplyOn(target, dest) && GetStoredPawnComp(target) != null;
        //}
    }



}