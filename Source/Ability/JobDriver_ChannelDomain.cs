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
            //ChanneToil.AddFailCondition(() => { return pawn.pather.Moving; });

            ChanneToil.AddFinishAction(OnChannelDomainFinish);

            yield return ChanneToil;
        }

        private void OnChannelDomainFinish()
        {
            if (abilityReference != null && abilityReference.IsDomainActive)
            {
                abilityReference.DestroyActiveDomain();
            }
            else
                Log.Message("AbilityReference is null or domain is already inactive");
        }
    }
}