using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace JJK
{
    public class CapacityScalingHediff : Hediff
    {
        public CapacityScalingHediffDef Def => (CapacityScalingHediffDef)def;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref def, "def");
        }

        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
            pawn.health.capacities.Notify_CapacityLevelsDirty();
        }

        public override void PostRemoved()
        {
            base.PostRemoved();
            pawn.health.capacities.Notify_CapacityLevelsDirty();
        }


        public override void PostTick()
        {
            base.PostTick();

            if (pawn.IsHashIntervalTick(60))
            {
                pawn.health.capacities.Notify_CapacityLevelsDirty();
            }
        }

        public float ApplyCapacityScaling(PawnCapacityDef capacity, float baseValue)
        {
            foreach (var scaling in Def.capacityScalings)
            {
                if (scaling.capacity == capacity.defName)
                {
                    float scaledOffset = Mathf.Lerp(scaling.minValue, scaling.maxValue, Severity);
                    baseValue += scaledOffset;
                }
            }
            return Mathf.Clamp(baseValue, 0f, 1f);
        }
    }
    //public class CapacityScalingHediff : Hediff
    //{
    //    public new CapacityScalingHediffDef def;
    //    public new List<PawnCapacityModifier> CapMods => CurrentScalingValues;

    //    private List<PawnCapacityModifier> CurrentScalingValues = null;


    //    public override void PostMake()
    //    {
    //        base.PostMake();

    //        CurrentScalingValues = new List<PawnCapacityModifier>();
    //    }

    //    public override void PostTick()
    //    {
    //        base.PostTick();

    //        CurrentScalingValues.Clear();

    //        if (def.capacityScalings == null)
    //        {
    //            Log.Error($"CapacityScalingHediff def capacityScalings is null.");
    //            return;
    //        }

    //        foreach (var item in def.capacityScalings)
    //        {
    //            PawnCapacityDef capacityDef = DefDatabase<PawnCapacityDef>.GetNamedSilentFail(item.capacity);
    //            if (capacityDef == null)
    //            {
    //                Log.Error($"CapacityScalingHediff on {pawn?.Name} has invalid capacity {item.capacity}. Skipping.");
    //                continue;
    //            }

    //            CurrentScalingValues.Add(new PawnCapacityModifier()
    //            {
    //                capacity = capacityDef,
    //                offset = Mathf.Lerp(item.minValue, item.maxValue, Severity)
    //            });
    //        }
    //    }

    //}

    public class CapacityScalingHediffDef : HediffDef
    {
        public List<CapacityScaling> capacityScalings = new List<CapacityScaling>();
    }


    public class CapacityScaling
    {
        public string capacity;
        public float minValue = 0f;
        public float maxValue = 1f;
    }


    //public class HediffCompProperties_CursedSpeechEffect : HediffCompProperties
    //{
    //    public List<CapacityScaling> capacityScalings = new List<CapacityScaling>();

    //    public HediffCompProperties_CursedSpeechEffect()
    //    {
    //        compClass = typeof(HediffComp_CursedSpeechEffect);
    //    }
    //}

    //public class HediffComp_CursedSpeechEffect : HediffComp
    //{
    //    public HediffCompProperties_CursedSpeechEffect Props => (HediffCompProperties_CursedSpeechEffect)props;

    //    public override string CompTipStringExtra
    //    {
    //        get
    //        {
    //            string result = "";
    //            foreach (var scaling in Props.capacityScalings)
    //            {
    //                float currentValue = Mathf.Lerp(scaling.minValue, scaling.maxValue, 1f - parent.Severity);
    //                result += $"{scaling.capacity.label}: {currentValue.ToStringPercent()}\n";
    //            }
    //            return result.TrimEndNewlines();
    //        }
    //    }

    //    public override void CompPostTick(ref float severityAdjustment)
    //    {
    //        base.CompPostTick(ref severityAdjustment);

    //        Pawn.health.capacities.Notify_CapacityLevelsDirty();
    //    }
    //}
}