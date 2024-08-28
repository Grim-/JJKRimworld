using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace JJK
{
    public class CompProperties_ZanpaktoWeapon : CompProperties
    {
        public CompProperties_ZanpaktoWeapon()
        {
            compClass = typeof(CompZanpaktoWeapon);
        }
    }
    public class CompZanpaktoWeapon : ThingCompExt
    {
        public new CompProperties_ZanpaktoWeapon Props => (CompProperties_ZanpaktoWeapon)props;
        private ZanpaktoWeapon Zanpakto => parent as ZanpaktoWeapon;

        //public override DamageWorker.DamageResult Notify_ApplyMeleeDamageToTarget(LocalTargetInfo target, DamageWorker.DamageResult DamageWorkerResult)
        //{
        //    DamageWorker.DamageResult  damageResult = base.Notify_ApplyMeleeDamageToTarget(target, DamageWorkerResult);
        //    Log.Message($"CompZanpaktoWeapon Notify_ApplyMeleeDamageToTarget");
        //    if (Zanpakto != null)
        //    {

        //        Log.Message($"CompZanpaktoWeapon");
        //        Log.Message(Zanpakto.GetSwordFormForState(Zanpakto.CurrentState));
        //        damageResult.totalDamageDealt *= Zanpakto.GetSwordFormForState(Zanpakto.CurrentState).MeleeDamage;
        //    }

        //    return damageResult;
        //}
    }
}