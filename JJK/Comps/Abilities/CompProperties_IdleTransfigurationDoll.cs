using RimWorld;
using System;
using Verse;
using Verse.AI.Group;

namespace JJK
{
    public class CompProperties_IdleTransfigurationDoll : CompProperties_CursedAbilityProps
    {
        public float CostPerMass = 0.01f;
        public string TargetThingDefName = "JJK_idleTransfigurationDoll";


        public CompProperties_IdleTransfigurationDoll()
        {
            compClass = typeof(CompAbilityEffect_IdleTransfigurationDoll);
        }
    }

    public class CompAbilityEffect_IdleTransfigurationDoll : BaseCursedEnergyAbility
    {
        public new CompProperties_IdleTransfigurationDoll Props
        {
            get
            {
                return (CompProperties_IdleTransfigurationDoll)this.props;
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
            float PawnMass = target.Pawn.GetStatValue(StatDefOf.Mass);
            float Cost = PawnMass * Props.CostPerMass;
            Thing NewItem = ThingMaker.MakeThing(JJKDefOf.JJK_idleTransfigurationDoll);

            this.parent.pawn.inventory.TryAddAndUnforbid(NewItem);

            CompStoredPawn compStoredPawn = NewItem.TryGetComp<CompStoredPawn>();
            if (compStoredPawn != null)
            {
                compStoredPawn.StorePawn(TargetPawn);
            }
            else
            {
                Log.Error("CompStoredPawn not found on JJK_idleTransfigurationDoll. Make sure it's defined in the ThingDef.");
                return;
            }


            Find.World.GetComponent<DollTransformationWorldComponent>().StorePawn(TargetPawn);
            RemovePawnFromMap(TargetPawn);

            parent.pawn.GetCursedEnergy()?.ConsumeCursedEnergy(parent.pawn, Cost);
        }

        private void RemovePawnFromMap(Pawn Pawn)
        {
            if (Pawn.Spawned)
            {
                Pawn.DeSpawn(DestroyMode.Vanish);
            }

            if (Pawn.Faction != null)
            {
                Pawn.SetFaction(null);
            }

            if (Pawn.GetLord() != null)
            {
                Pawn.GetLord().Notify_PawnLost(Pawn, PawnLostCondition.Vanished);
            }

            if (Find.WorldPawns.Contains(Pawn))
            {
                Find.WorldPawns.RemovePawn(Pawn);
            }

        }
    }


}