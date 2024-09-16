using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Noise;
using static UnityEngine.GraphicsBuffer;

namespace JJK
{
    public class Projectile_ImpactAOE : Projectile
    {
        public new ProjectileProperties_ImpactAOE Props => (ProjectileProperties_ImpactAOE)this.def.projectile;


        private float CasterDamageBonus
        {
            get
            {
                if (this.launcher is Pawn launcherPawn)
                {
                    return launcherPawn.GetStatValueForPawn(JJKDefOf.JJK_CursedEnergyDamageBonus, launcherPawn);
                }
                return 1f;
            }
        }

        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            List<Thing> ThingsToHit = JJKUtility.GetThingsInRange(this.Position, this.MapHeld, this.Props.ExplosionRadius, (Thing) =>
            {
                return Thing != this;
            }).ToList();



            JJKUtility.DealDamageToThingsInRange(ThingsToHit, Props.damageDef, Props.BaseDamage * CasterDamageBonus, Props.GetArmorPenetration(this.launcher));

            foreach (var thingToHit in ThingsToHit)
            {
                if (!thingToHit.Destroyed && thingToHit is Pawn pawnToHit)
                {
                    PushBack(pawnToHit);
                }
            }


            if (this.Props.ExplosionEffect != null)
            {
                Log.Message("wdwddwdw");
                this.Props.ExplosionEffect.Spawn(this.Position, this.MapHeld);
            }

            base.Impact(hitThing, blockedByShield);
        }

        private void PushBack(Pawn HitPawn)
        {
            if (launcher == null || launcher.Map == null)
            {
                return;
            }

            Map map = launcher.Map;

            IntVec3 launchDirection = HitPawn.Position - launcher.Position;

            IntVec3 destination = HitPawn.Position + launchDirection * Rand.Range(1, 4);
            destination = destination.ClampInsideMap(launcher.Map);

            IntVec3 finalDestinattion = IntVec3.Zero;

            if (GenAdj.TryFindRandomAdjacentCell8WayWithRoom(HitPawn, out finalDestinattion))
            {
                if (finalDestinattion.IsValid && finalDestinattion.InBounds(map) && finalDestinattion.Walkable(map))
                {
                    PawnFlyer pawnFlyer = PawnFlyer.MakeFlyer(JJKDefOf.JJK_Flyer, HitPawn, finalDestinattion, null, null);
                    GenSpawn.Spawn(pawnFlyer, HitPawn.Position, launcher.Map);
                }
            }
        }
    }


    public class ProjectileProperties_ImpactAOE : ProjectileProperties
    {
        public float BaseDamage = 1f;
        public float ExplosionRadius = 10f;
        public EffecterDef ExplosionEffect;
    }
}