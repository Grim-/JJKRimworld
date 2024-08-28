using RimWorld;
using System.Collections.Generic;
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

        public override string CompInspectStringExtra()
        {
            return base.CompInspectStringExtra() +
                $"\r\n This equipment has a chance ({Props.ApplyChance * 100}) on hit to apply {Props.hediffToApply.defName}";
        }

        public override string GetDescriptionPart()
        {
            return base.GetDescriptionPart() +
                $"\r\n This equipment has a chance ({Props.ApplyChance * 100}) on hit to apply {Props.hediffToApply.defName}";
        }
    }

    public class CompProperties_EquipCompRemoveHediffOnHit : CompProperties
    {
        public bool ApplyToSelf = false;
        public bool ApplyOnTarget = true;
        public float Chance = 0.5f;

        public float Severity = 1f;



        public List<HediffDef> hediffsToRemove;

        public CompProperties_EquipCompRemoveHediffOnHit()
        {
            compClass = typeof(EquipComp_ApplyHediffOnHit);
        }
    }

    public class EquipComp_RemoveHediffOnHit : ThingCompExt
    {
        public new CompProperties_EquipCompRemoveHediffOnHit Props => (CompProperties_EquipCompRemoveHediffOnHit)props;

        public override DamageWorker.DamageResult Notify_ApplyMeleeDamageToTarget(LocalTargetInfo target, DamageWorker.DamageResult DamageWorkerResult)
        {
            if (Props.ApplyOnTarget && target.Pawn != null)
            {
                if (Rand.Range(0, 1) <= Props.Chance)
                {
                    foreach (var item in Props.hediffsToRemove)
                    {
                        if (item == null)
                        {
                            continue;
                        }

                        Hediff hediff = target.Pawn.health.hediffSet.GetFirstHediffOfDef(item);
                        if (hediff != null)
                        {
                            target.Pawn.health.RemoveHediff(hediff);
                        }
                    }
                    return base.Notify_ApplyMeleeDamageToTarget(target, DamageWorkerResult);
                }
            }
            else if (Props.ApplyToSelf && _EquipOwner != null)
            {
                if (Rand.Range(0, 1) <= Props.Chance)
                {
                    foreach (var item in Props.hediffsToRemove)
                    {
                        if (item == null)
                        {
                            continue;
                        }

                        Hediff hediff = _EquipOwner.health.hediffSet.GetFirstHediffOfDef(item);
                        if (hediff != null)
                        {
                            _EquipOwner.health.RemoveHediff(hediff);
                        }
                    }

                    return base.Notify_ApplyMeleeDamageToTarget(target, DamageWorkerResult);
                }
            }
            return base.Notify_ApplyMeleeDamageToTarget(target, DamageWorkerResult);
        }
    }
}