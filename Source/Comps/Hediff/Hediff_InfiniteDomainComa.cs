using RimWorld;
using System.Collections.Generic;
using Verse;

namespace JJK
{
    public class Hediff_InfiniteDomainComa : HediffWithComps
    {
        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
            pawn.health.Notify_HediffChanged(this);
            pawn.stances.stunner.StunFor(100, this.pawn);
        }

        public override void PostRemoved()
        {
            base.PostRemoved();
            pawn.health.Notify_HediffChanged(null);
        }

        public override bool ShouldRemove => Severity <= 0;

        public new List<PawnCapacityModifier> CapMods
        {
            get
            {
                float impact = Severity * 0.9f;
                return new List<PawnCapacityModifier>
                    {
                        new PawnCapacityModifier
                        {
                            capacity = PawnCapacityDefOf.Consciousness,
                            offset = -impact,
                            setMax = 1f - impact
                        }
                    };
            }
        }
    }
    //public class Hediff_InfiniteDomainComa : HediffWithComps
    //{ 
    //    public override void PostAdd(DamageInfo? dinfo)
    //    {
    //        base.PostAdd(dinfo);
    //        RecalculateConsciousness();
    //    }

    //    public override void PostRemoved()
    //    {
    //        base.PostRemoved();
    //        pawn.health.Notify_HediffChanged(null);
    //    }

    //    public override void Tick()
    //    {
    //        base.Tick();
    //        if (pawn.IsHashIntervalTick(250)) // Check every 250 ticks (about 4 seconds)
    //        {
    //            RecalculateConsciousness();
    //        }
    //    }
    //    public new List<PawnCapacityModifier> CapMods
    //    {
    //        get
    //        {
    //            float consciousnessImpact = 0.9f * Severity;
    //            return new List<PawnCapacityModifier>
    //            {
    //                new PawnCapacityModifier
    //                {
    //                    capacity = PawnCapacityDefOf.Consciousness,
    //                    offset = -consciousnessImpact,
    //                    setMax = 1f - consciousnessImpact
    //                }
    //            };
    //        }
    //    }
    //    private void RecalculateConsciousness()
    //    {
    //        float maxConsciousness = 1f - (0.9f * Severity);
    //        maxConsciousness = Mathf.Clamp(maxConsciousness, 0.1f, 1f);

    //        float ConciousnessLevel = pawn.health.capacities.GetLevel(PawnCapacityDefOf.Consciousness);
    //        PawnCapacityUtility.(pawn.health.capacities, .Consciousness, maxConsciousness);
    //        pawn.health.Notify_HediffChanged(this);
    //    }
    //}
}