using System.Collections.Generic;
using Verse.AI;
using Verse;

namespace JJK
{
    public class JobDriver_LuredMove : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed) => true;

        protected override IEnumerable<Toil> MakeNewToils()
        {
            Pawn caster = job.targetA.Pawn;

            yield return Toils_General.Do(() =>
            {
                if (caster == null || caster.Dead || caster.Downed)
                {
                    EndJobWith(JobCondition.Incompletable);
                }
            });

            yield return new Toil
            {
                tickAction = () =>
                {
                    if (caster == null || caster.Dead || caster.Downed)
                    {
                        EndJobWith(JobCondition.Incompletable);
                        return;
                    }

                    IntVec3 moveTowards = caster.Position.ClampInsideMap(pawn.Map);
                    pawn.pather.StartPath(moveTowards, PathEndMode.Touch);
                },
                defaultCompleteMode = ToilCompleteMode.Never
            };
        }
    }
}