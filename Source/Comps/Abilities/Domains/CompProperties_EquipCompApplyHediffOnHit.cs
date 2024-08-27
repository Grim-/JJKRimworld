using RimWorld;
using UnityEngine;
using Verse;

namespace JJK
{
    public class CompProperties_EquipCompApplyHediffOnHit : CompProperties
    {
        public bool ApplyToSelf = false;
        public bool ApplyOnTarget = true;
        public float ApplyChance = 0.5f;

        public float Severity = 1f;

        public HediffDef hediffToApply;

        public CompProperties_EquipCompApplyHediffOnHit()
        {
            compClass = typeof(EquipComp_ApplyHediffOnHit);
        }
    }

    public class EquipComp_ApplyHediffOnHit : ThingCompExt
    {
        public new CompProperties_EquipCompApplyHediffOnHit Props => (CompProperties_EquipCompApplyHediffOnHit)props;

        public override DamageWorker.DamageResult Notify_ApplyMeleeDamageToTarget(LocalTargetInfo target, DamageWorker.DamageResult DamageWorkerResult)
        {
            if (Props.ApplyOnTarget && target.Pawn != null)
            {
                if (Rand.Range(0, 1) <= Props.ApplyChance)
                {
                    Hediff hediff = target.Pawn.health.GetOrAddHediff(Props.hediffToApply);
                    hediff.Severity = Props.Severity;
                    return base.Notify_ApplyMeleeDamageToTarget(target, DamageWorkerResult);
                }
            }
            else if (Props.ApplyToSelf && _EquipOwner != null)
            {
                if (Rand.Range(0, 1) <= Props.ApplyChance)
                {
                    Hediff hediff = _EquipOwner.health.GetOrAddHediff(Props.hediffToApply);
                    hediff.Severity = Props.Severity;
                    return base.Notify_ApplyMeleeDamageToTarget(target, DamageWorkerResult);
                }
            }
            return base.Notify_ApplyMeleeDamageToTarget(target, DamageWorkerResult);
        }
    }
}