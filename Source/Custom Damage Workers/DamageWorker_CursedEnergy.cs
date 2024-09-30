using RimWorld;
using System.Collections.Generic;
using Verse;

namespace JJK
{
    public class DamageWorker_CursedEnergy : DamageWorker
    {
        public override DamageResult Apply(DamageInfo dinfo, Thing thing)
        {
            DamageResult damage = base.Apply(dinfo, thing);
            return damage;
        }
    }


}