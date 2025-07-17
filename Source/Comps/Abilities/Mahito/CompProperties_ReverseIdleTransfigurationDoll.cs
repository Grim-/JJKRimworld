using RimWorld;
using System.Linq;
using Verse;

namespace JJK
{
    public class CompProperties_ReverseIdleTransfigurationDoll : CompProperties_CursedAbilityProps
    {
        public float CostPerMass = 0.05f;

        public CompProperties_ReverseIdleTransfigurationDoll()
        {
            compClass = typeof(CompAbilityEffect_ReverseIdleTransfigurationDoll);
        }
    }

    public class CompAbilityEffect_ReverseIdleTransfigurationDoll : BaseCursedEnergyAbility
    {
        public new CompProperties_ReverseIdleTransfigurationDoll Props => (CompProperties_ReverseIdleTransfigurationDoll)props;

        public override void ApplyAbility(LocalTargetInfo target, LocalTargetInfo dest)
        {
            if (!(target.Thing is ThingWithComps targetThing))
                return;

            CompStoredPawn storedPawnComp = targetThing.GetComp<CompStoredPawn>();
            if (storedPawnComp == null || storedPawnComp.Pawn == null)
                return;

            Gene_CursedEnergy cursedEnergy = parent.pawn.GetCursedEnergy();

            if (cursedEnergy == null)
            {
                return;
            }



            Pawn storedPawn = storedPawnComp.Pawn;
            float pawnMass = storedPawn.GetStatValue(StatDefOf.Mass);
            float cost = pawnMass * Props.CostPerMass;


            if (cursedEnergy.Value >= cost)
            {
                parent.pawn.GetCursedEnergy()?.ConsumeCursedEnergy(cost);
                GenSpawn.Spawn(storedPawn, targetThing.Position, targetThing.Map);

                // Restore the pawn's faction if it had one
                if (parent.pawn.Faction != null)
                {
                    storedPawn.SetFaction(parent.pawn.Faction);
                }

                DollTransformationWorldComp dollManager = Find.World.GetComponent<DollTransformationWorldComp>();

                if (dollManager != null)
                {
                    dollManager.RemovePawn(storedPawn);
                }

                targetThing.Destroy();

                Messages.Message($"{storedPawn.LabelShort} has been restored from transfiguration.", MessageTypeDefOf.PositiveEvent);
            }
            else
            {
                Messages.Message($"{storedPawn.LabelShort} requires {cost} to restore from transfiguration.", MessageTypeDefOf.NegativeEvent);
            }



        }

        public override bool AICanTargetNow(LocalTargetInfo target)
        {
            return false;
        }
    }

}