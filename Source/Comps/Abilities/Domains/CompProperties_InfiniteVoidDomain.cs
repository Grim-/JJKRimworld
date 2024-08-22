using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace JJK
{
    public class CompProperties_InfiniteVoidDomain : CompProperties_DomainComp
    {
        public CompProperties_InfiniteVoidDomain()
        {
            compClass = typeof(CompInfiniteVoidDomain);
        }
    }


    public class CompInfiniteVoidDomain : CompDomainEffect
    {
        public override void ApplyDomainEffects()
        {
            foreach (var item in GetPawnsInDomain())
            {
                if (!item.IsImmuneToDomainSureHit() && _DomainCaster != item)
                {
                    Hediff hediff = item.health.GetOrAddHediff(JJKDefOf.JJK_InfiniteDomainComa);
                    hediff.Severity += 0.04f;
                    UpdateComaDuration(hediff);
                }
            }
        }

        public override void OnTick()
        {
            base.OnTick();

            foreach (var targetPawn in GetPawnsInDomain())
            {
                if (!targetPawn.IsImmuneToDomainSureHit() && _DomainCaster != targetPawn)
                {
                    Hediff hediff = targetPawn.health.GetOrAddHediff(JJKDefOf.JJK_InfiniteDomainComa);
                    hediff.Severity += 0.04f;
                    UpdateComaDuration(hediff);
                }
            }
        }
        private void UpdateComaDuration(Hediff hediff)
        {
            if (hediff.TryGetComp<HediffComp_Disappears>() is HediffComp_Disappears disappearsComp)
            {
                // Calculate new duration based on severity
                int baseDuration = 5000; // 5000 ticks = about 1.4 in-game hours
                int maxAdditionalDuration = 45000; // 45000 ticks = about 12.5 in-game hours
                int newDuration = baseDuration + (int)(hediff.Severity * maxAdditionalDuration);

                // Update the disappears comp with the new duration
                disappearsComp.ticksToDisappear = newDuration;
            }
        }
        public override void RemoveDomainEffects()
        {

        }
    }


}