using RimWorld;
using UnityEngine;
using Verse;

namespace JJK
{
    public class PlayfulCloudKnockbackFlyer : PawnFlyer
    {
        public Thing instigator;
        public float collisionDamage = 1f;

        protected override void RespawnPawn()
        {
            DamageInfo damageInfo = new DamageInfo(DamageDefOf.Blunt, collisionDamage);

            if (!FlyingPawn.Destroyed && !FlyingPawn.Dead)
            {
                FlyingPawn.TakeDamage(damageInfo);
            }
         
            base.RespawnPawn();
            //this.Destroy();
        }
    }
}