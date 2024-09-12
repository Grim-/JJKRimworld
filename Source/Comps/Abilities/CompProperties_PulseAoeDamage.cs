using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace JJK
{
    public class CompProperties_PulseAoeDamage : CompProperties
    {
        public int TicksBetweenPulse = 80;
        public float ExplosionRadius = 3f;
        public DamageDef DamageType = DamageDefOf.Bomb;
        public int DamageAmount = 7;
        public float ArmorPenetration = 0.4f;
        public bool CanDamageSelf = false;
        public bool CanDamageFriendly = true;

        public CompProperties_PulseAoeDamage()
        {
            compClass = typeof(PulseAOEDamageComp);
        }
    }

    public class PulseAOEDamageComp : ThingComp
    {
        private int TicksUntilPulse;

        public CompProperties_PulseAoeDamage Props => (CompProperties_PulseAoeDamage)props;



        private Thing Launcher => Projectile.Launcher;
        private Projectile Projectile => (Projectile)parent;

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            ResetTimer();
        }

        public override void CompTick()
        {
            base.CompTick();

            TicksUntilPulse--;

            if (TicksUntilPulse <= 0)
            {
                Explode();
                ResetTimer();
            }
        }

        private void Explode()
        {
            List<Thing> thingsInRadius = JJKUtility.GetThingsInRange(parent.Position, parent.MapHeld, Props.ExplosionRadius, TargetFilter).ToList();

            foreach (Thing thing in thingsInRadius)
            {
                if (thing.Position.DistanceTo(parent.Position) <= Props.ExplosionRadius)
                {
                    IntVec3 position = thing.Position;
                    Props.DamageType.soundExplosion.PlayOneShot(new TargetInfo(position, parent.Map, false));
                    FleckMaker.WaterSplash(position.ToVector3Shifted(), parent.Map, 1f * 6f, 20f);

                    if (!thing.Destroyed)
                    {
                        DamageInfo dinfo = new DamageInfo(Props.DamageType, Props.DamageAmount, Props.ArmorPenetration, -1f, parent);
                        thing.TakeDamage(dinfo);
                    }
                }
            }
        }

        private bool TargetFilter(Thing Thing)
        {
            if (Launcher is Pawn launcherPawn && !Props.CanDamageSelf)
            {
                return Thing != launcherPawn;
            }
            else if (!Thing.Faction.HostileTo(parent.Faction) && Props.CanDamageFriendly)
            {
                return true;
            }
            return true;
        }

        private void ResetTimer()
        {
            TicksUntilPulse = Props.TicksBetweenPulse;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref TicksUntilPulse, "TicksUntilExplosion");
        }
    }
}