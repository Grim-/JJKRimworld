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

            //ChanneToil.AddPreTickAction(ShouldFail);
            ChanneToil.tickAction = OnChannelDomainTick;

            ChanneToil.AddFinishAction(OnChannelDomainFinish);
            yield return ChanneToil;
        }


        private void OnChannelDomainTick()
        {
            if (abilityReference != null)
            {
                abilityReference.IsDomainActive = true;
            }
        }

        private void ShouldFail()
        {
            if (abilityReference == null || abilityReference.DomainThing == null)
            {
                this.EndJobWith(JobCondition.InterruptForced);
            }
        }

        private void OnChannelDomainFinish()
        {
            if (abilityReference != null)
            {
                abilityReference.DestroyActiveDomain();
            }
        }
    }
}