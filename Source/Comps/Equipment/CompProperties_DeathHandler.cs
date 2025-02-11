using System;
using System.Collections.Generic;
using Verse;

namespace JJK
{
    public class CompProperties_DeathHandler : CompProperties
    {
        public CompProperties_DeathHandler()
        {
            compClass = typeof(Comp_OnDeathHandler);
        }
    }

    public class Comp_OnDeathHandler : ThingCompExt
    {
        public event Action<Thing> OnDeath;
        public event Action<Thing> OnDespawned;

        public override void Notify_KilledPawn(Pawn pawn)
        {
            base.Notify_KilledPawn(pawn);
        }

        public override void Notify_Killed(Map prevMap, DamageInfo? dinfo = null)
        {
            OnDeath?.Invoke(this.parent);
            base.Notify_Killed(prevMap, dinfo);
        }

        public override void PostDeSpawn(Map map)
        {
            OnDespawned?.Invoke(parent);
            base.PostDeSpawn(map);
        }
    }
}