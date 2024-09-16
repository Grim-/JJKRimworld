using RimWorld;
using System;
using Verse;

namespace JJK
{
    public class DelegateFlyer : PawnFlyer
    {
        public Action<PawnFlyer> OnSpawn;
        public Action<Pawn, PawnFlyer> OnRespawnPawn;


        public override void PostMake()
        {
            base.PostMake();
            OnSpawn?.Invoke(this);
        }

        protected override void RespawnPawn()
        {
            OnRespawnPawn?.Invoke(this.FlyingPawn, this);
            base.RespawnPawn();
        }
    }
}