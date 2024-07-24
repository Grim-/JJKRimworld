using Verse;

namespace JJK
{
    public class CompAbilityEffect_RestorePawnFromDoll : BaseCursedEnergyAbility
    {
        public override void ApplyAbility(LocalTargetInfo target, LocalTargetInfo dest)
        {
            if (!(target.Thing is Thing dollItem))
            {
                return;
            }

            CompStoredPawn compStoredPawn = dollItem.TryGetComp<CompStoredPawn>();
            if (compStoredPawn == null || compStoredPawn.Pawn == null)
            {
                Log.Error("No stored pawn found in the doll.");
                return;
            }

            Pawn storedPawn = compStoredPawn.Pawn;

            // Spawn the stored pawn back into the world
            GenSpawn.Spawn(storedPawn, dollItem.Position, dollItem.Map);

            // Remove the doll item
            dollItem.Destroy();

            // Consume cursed energy (if needed)
            // parent.pawn.GetCursedEnergy()?.ConsumeCursedEnergy(parent.pawn, Cost);
        }
    }
}