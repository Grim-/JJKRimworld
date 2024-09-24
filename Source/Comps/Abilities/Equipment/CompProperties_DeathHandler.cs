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
            Log.Message("Notify_KilledPawn called");
            base.Notify_KilledPawn(pawn);
        }

        public override void Notify_Killed(Map prevMap, DamageInfo? dinfo = null)
        {
            OnDeath?.Invoke(this.parent);
            Log.Message("Notify_Killed called");
            base.Notify_Killed(prevMap, dinfo);
        }

        public override void PostDeSpawn(Map map)
        {
            Log.Message("PostDeSpawn called");
            OnDespawned?.Invoke(parent);
            base.PostDeSpawn(map);
        }
    }
}