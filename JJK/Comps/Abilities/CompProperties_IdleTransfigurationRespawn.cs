using RimWorld;
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
            Thing dollStack = parent.pawn.inventory.innerContainer
                .FirstOrDefault(thing => thing.def == JJKDefOf.JJK_idleTransfigurationDoll);

            if (dollStack == null)
            {
                Messages.Message("No Idle Transfiguration Doll found in inventory.", MessageTypeDefOf.RejectInput);
                return;
            }

            CompStoredPawn storedPawnComp = dollStack.TryGetComp<CompStoredPawn>();
            if (storedPawnComp == null || storedPawnComp.Pawn == null)
            {
                Messages.Message("The doll doesn't contain a stored pawn.", MessageTypeDefOf.RejectInput);
                return;
            }

            if (!target.Cell.Walkable(parent.pawn.Map))
            {
                Messages.Message("Cannot spawn pawn at the target location. Please choose a walkable cell.", MessageTypeDefOf.RejectInput);
                return;
            }

            Pawn storedPawn = storedPawnComp.ReleasePawn();
            if (storedPawn == null)
            {
                Messages.Message("Failed to retrieve stored pawn.", MessageTypeDefOf.RejectInput);
                return;
            }

            float pawnMass = storedPawn.GetStatValue(StatDefOf.Mass);
            float cost = pawnMass * Props.CostPerMass;
            parent.pawn.GetCursedEnergy()?.ConsumeCursedEnergy(parent.pawn, cost);

            GenSpawn.Spawn(storedPawn, target.Cell, parent.pawn.Map);

            if (parent.pawn.Faction != null)
            {
                storedPawn.SetFaction(parent.pawn.Faction);
            }

            DollTransformationWorldComponent dollManager = Find.World.GetComponent<DollTransformationWorldComponent>();
            if (dollManager != null)
            {
                dollManager.RemovePawn(storedPawn);
            }

            if (dollStack.stackCount <= 1)
            {
                parent.pawn.inventory.innerContainer.Remove(dollStack);
                dollStack.Destroy();
            }
            else
            {
                dollStack.stackCount--;
            }

            Messages.Message($"{storedPawn.LabelShort} has been restored from transfiguration.", MessageTypeDefOf.PositiveEvent);
        }

        public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
        {
            return base.CanApplyOn(target, dest) &&
                   parent.pawn.inventory.innerContainer.Any(thing => thing.def == JJKDefOf.JJK_idleTransfigurationDoll);
        }

        public override bool AICanTargetNow(LocalTargetInfo target)
        {
            return false;
        }
    }
}