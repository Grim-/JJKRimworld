using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace JJK
{
    public class JobDriver_ChannelDomain : JobDriver
    {
        private Ability_ExpandDomain abilityReference;

        public void SetAbilityReference(Ability_ExpandDomain ability)
        {
            abilityReference = ability;
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            Toil ChanneToil = new Toil();
            ChanneToil.defaultCompleteMode = ToilCompleteMode.Never;
            ChanneToil.tickAction = () =>
            {
                if (pawn.pather.MovingNow || pawn.stances.stunner.Stunned)
                {
                    Log.Message("Maintaing Domain channel was interrupted");
                    abilityReference?.DestroyActiveDomain();
                    this.EndJobWith(JobCondition.InterruptOptional);
                }
            };
            yield return ChanneToil;
        }
    }

    public class CantMoveStance : Stance
    {
        public override bool StanceBusy => true;
    }
}