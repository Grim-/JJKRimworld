using System;
using System.Collections.Generic;
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


        public override void PostPostMake()
        {
            base.PostPostMake();
            Log.Message("CompOnDeathHandler PostPostMake called");

        }

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

        public override void Notify_KilledLeavingsLeft(List<Thing> leavings)
        {
            Log.Message("Notify_KilledLeavingsLeft called");
            base.Notify_KilledLeavingsLeft(leavings);

        }
    }
}