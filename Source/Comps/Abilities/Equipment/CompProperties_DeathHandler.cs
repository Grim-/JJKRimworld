using System;
using Verse;

namespace JJK
{
    public class CompProperties_DeathHandler : CompProperties
    {
        public CompProperties_DeathHandler()
        {
            compClass = typeof(CompOnDeathHandler);
        }
    }

    public class CompOnDeathHandler : ThingCompExt
    {
        public event Action<Thing> OnDeath;
        public override void Notify_Killed(Map prevMap, DamageInfo? dinfo = null)
        {
            base.Notify_Killed(prevMap, dinfo);

            OnDeath?.Invoke(this.parent);
        }
    }
}