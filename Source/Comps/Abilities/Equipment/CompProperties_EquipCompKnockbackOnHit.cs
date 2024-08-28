using RimWorld;
using UnityEngine;
using Verse;

namespace JJK
{
    public class CompProperties_KnockbackOnHit : CompProperties
    {
        public float knockbackChance = 0.5f;
        public IntRange knockbackRange = new IntRange(2, 3);
        public float collisionDamage = 5f;

        public CompProperties_KnockbackOnHit()
        {
            compClass = typeof(CompKnockbackOnHit);
        }
    }





    public class CompKnockbackOnHit : ThingCompExt
    {
        public CompProperties_KnockbackOnHit Props => (CompProperties_KnockbackOnHit)props;

        public override DamageWorker.DamageResult Notify_ApplyMeleeDamageToTarget(LocalTargetInfo target, DamageWorker.DamageResult DamageWorkerResult)
        {
            if (target.Pawn == null)
            {
               // Log.Error("Notify_ApplyMeleeDamageToTarget: target.Pawn is null");
                return base.Notify_ApplyMeleeDamageToTarget(target, DamageWorkerResult);
            }

            Map map = target.Pawn.Map;
            if (map == null)
            {
                //Log.Error("Notify_ApplyMeleeDamageToTarget: target.Pawn.Map is null");
                return base.Notify_ApplyMeleeDamageToTarget(target, DamageWorkerResult);
            }

            if (!target.Pawn.DeadOrDowned && Rand.Range(0, 1) <= Props.knockbackChance)
            {
                IntVec3 launchDirection = target.Pawn.Position - parent.Position;
                IntVec3 destination = target.Pawn.Position + launchDirection * Rand.Range(1, 2);
                destination = destination.ClampInsideMap(map);

                if (destination.IsValid && destination.InBounds(map))
                {
                    PawnFlyer pawnFlyer = PawnFlyer.MakeFlyer(JJKDefOf.JJK_PlayfulCloudKnockbackFlyer, target.Pawn, destination, null, null);
                    if (pawnFlyer != null)
                    {
                        GenSpawn.Spawn(pawnFlyer, destination, map);
                    }
                    else
                    {
                        Log.Error("Failed to create PawnFlyer");
                    }
                }
            }
            return base.Notify_ApplyMeleeDamageToTarget(target, DamageWorkerResult);
        }

        public override string CompInspectStringExtra()
        {
            return base.CompInspectStringExtra() + 
                $"\r\n This equipment has a chance ({Mathf.RoundToInt(Props.knockbackChance * 100)} to knock back upto {Props.knockbackRange.max} tiles away, the target of a melee attack made with it.";
        }

        public override string GetDescriptionPart()
        {
            return base.GetDescriptionPart() +
                $"\r\n This equipment has a chance ({Mathf.RoundToInt(Props.knockbackChance * 100)} to knock back upto {Props.knockbackRange.max} tiles away, the target of a melee attack made with it.";
        }
    }

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