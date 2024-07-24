using RimWorld;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Noise;
using static UnityEngine.GraphicsBuffer;

namespace JJK
{
    internal class Projectile_Red : Projectile
    {
        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            base.Impact(hitThing, blockedByShield);

            if (hitThing != null)
            {
                if (hitThing is Pawn pawn)
                {
                    PushBack(pawn);
                }

                if (launcher is Pawn launcherPawn)
                {
                    Damage(hitThing, launcherPawn);
                }
             
            }

        }

        private void PushBack(Pawn HitPawn)
        {
            IntVec3 launchDirection = HitPawn.Position - launcher.Position;

            // Calculate destination based on knockback distance
            IntVec3 destination = HitPawn.Position + launchDirection * Rand.Range(1, 4);

            // Ensure the destination is within the map bounds
            destination = destination.ClampInsideMap(launcher.Map);

            // Spawn the flyer if destination is valid
            if (destination.IsValid && destination.InBounds(launcher.Map))
            {
                PawnFlyer pawnFlyer = PawnFlyer.MakeFlyer(JJKDefOf.JJK_Flyer, HitPawn, destination, null, null);
                GenSpawn.Spawn(pawnFlyer, destination, launcher.Map);
            }
        }

        private void Damage(Thing HitThing, Pawn Caster)
        {
            if (HitThing == null)
            {
                return;
            }

            float DamageBonus = 1f;

            if (Caster != null)
            {
                DamageBonus = Caster.GetStatValue(JJKDefOf.JJK_CursedEnergyDamageBonus);
            }

            // Deal damage to the hitThing
            DamageInfo damageInfo = new DamageInfo(def.projectile.damageDef, DamageAmount * DamageBonus, 0f, -1f, HitThing, null, null, DamageInfo.SourceCategory.ThingOrUnknown);
            HitThing.TakeDamage(damageInfo);
        }
    }
}