using RimWorld;
using Verse;

namespace JJK
{
    public class CompProperties_AbilityCarryFly : CompProperties_AbilityEffect
    {
        public CompProperties_AbilityCarryFly()
        {
            compClass = typeof(CompAbilityEffect_CarryFly);
        }
    }
    public class CompAbilityEffect_CarryFly : CompAbilityEffect
    {
        private Pawn targetPawn;
        private IntVec3 destination;
        private DelegateFlyer casterFlyer;
        private DelegateFlyer carriedFlyer;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            Map map = parent.pawn.Map;

            if (target.Pawn != null)
            {
                // Handle pawn targeting case - two step process
                targetPawn = target.Pawn;
                Find.Targeter.BeginTargeting(
                    new TargetingParameters
                    {
                        canTargetLocations = true,
                    },
                    (LocalTargetInfo locationdest) =>
                    {
                        destination = locationdest.Cell;
                        FlyToTarget();
                    },
                    null,
                    (LocalTargetInfo validateTarget) =>
                    {
                        return !validateTarget.Cell.Roofed(targetPawn.Map);
                    });
                return;
            }
        }

        private void FlyToTarget()
        {
            Map map = parent.pawn.Map;
            casterFlyer = (DelegateFlyer)PawnFlyer.MakeFlyer(JJKDefOf.JJK_GenericFlyer, parent.pawn, targetPawn.Position, null, null);
            GenSpawn.Spawn(casterFlyer, targetPawn.Position, map);
            casterFlyer.OnRespawnPawn += CasterReachedTarget;
        }
        private void CasterReachedTarget(Pawn pawn, PawnFlyer flyer, Map map)
        {
            casterFlyer.OnRespawnPawn -= CasterReachedTarget;
            FlyBothToDestination();
        }
        private void FlyBothToDestination()
        {
            Map map = parent.pawn.Map;
            casterFlyer = (DelegateFlyer)PawnFlyer.MakeFlyer(JJKDefOf.JJK_GenericFlyer, parent.pawn, destination, null, null);
            carriedFlyer = (DelegateFlyer)PawnFlyer.MakeFlyer(JJKDefOf.JJK_GenericFlyer, targetPawn, destination, null, null);
            GenSpawn.Spawn(casterFlyer, parent.pawn.Position, map);
            GenSpawn.Spawn(carriedFlyer, targetPawn.Position, map);
            targetPawn = null;
        }
    }
}

