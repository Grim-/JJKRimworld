using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace JJK
{
    public class JobDriver_ToadCarryPawn : JobDriver
    {
        private const TargetIndex SelfPawnIndex = TargetIndex.A;
        private const TargetIndex TargetPawnIndex = TargetIndex.B;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return ReservationUtility.Reserve(TargetA.Pawn, TargetA, this.job);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            var targetPawn = TargetB.Pawn;
            if (targetPawn == null) 
                yield break;

            yield return Toils_Goto.GotoThing(TargetPawnIndex, PathEndMode.Touch)
                .FailOnDespawnedOrNull(TargetPawnIndex);

            Toil pickupToil = new Toil();
            pickupToil.initAction = () =>
            {
                Pawn actor = TargetA.Pawn;
                if (actor == null) return;

                var toadComp = actor.GetComp<Comp_TenShadowsToad>();
                if (toadComp == null) 
                    return;

                if (toadComp.TryPickupPawn(targetPawn))
                {
                    actor.jobs.EndCurrentJob(JobCondition.Succeeded);
                }
                else
                {
                    actor.jobs.EndCurrentJob(JobCondition.Incompletable);
                }
            };
            pickupToil.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return pickupToil;
        }
    }
}

    