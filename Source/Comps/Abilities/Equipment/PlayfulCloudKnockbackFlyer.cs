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
            FlyingPawn.TakeDamage(damageInfo);
            MoteMaker.ThrowText(FlyingPawn.DrawPos, this.Map, "Collision!", Color.red);
            base.RespawnPawn();
            //this.Destroy();
        }
    }
}