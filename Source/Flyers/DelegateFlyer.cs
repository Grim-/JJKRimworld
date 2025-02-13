using RimWorld;
using System;
using Verse;

namespace JJK
{
    public class DelegateFlyer : PawnFlyer
    {
        public event Action<PawnFlyer> OnSpawn;

        public event Action<Pawn, PawnFlyer, Map> OnBeforeRespawnPawn;
        public event Action<Pawn, PawnFlyer, Map> OnRespawnPawn;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);

            if (!respawningAfterLoad)
            {
                OnSpawn?.Invoke(this);
            }
        }

        protected override void RespawnPawn()
        {
            Pawn pawn = this.FlyingPawn;
            OnBeforeRespawnPawn?.Invoke(this.FlyingPawn, this, this.Map);
            base.RespawnPawn();
            OnRespawnPawn?.Invoke(pawn, this, pawn.Map);
        }
    }
}