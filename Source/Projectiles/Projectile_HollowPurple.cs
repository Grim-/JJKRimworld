using RimWorld;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace JJK
{
    internal class Projectile_HollowPurple : ScalingStatDamageProjectile
    {
        public override void Tick()
        {
            base.Tick();

            if (DestinationCell == Position)
            {
                Destroy();
                return;
            }

            ExplodeAtPosition(Position, Map, 3f);
        }

        private void ExplodeAtPosition(IntVec3 pos, Map map, float radius)
        {
            List<Thing> ignoredThings = new List<Thing>();

            if (launcher != null && launcher.Faction != null)
            {
                Faction launcherFaction = launcher.Faction;
                foreach (Pawn pawn in map.mapPawns.AllPawns.Where(p => p.Faction == launcherFaction))
                {
                    ignoredThings.Add(pawn);
                }
            }

            // Perform explosion, excluding ignored things
            if (launcher != null && intendedTarget != null)
            {
                GenExplosion.DoExplosion(
                    pos, map, radius, DamageDefOf.Blunt, launcher, 999, ArmorPenetration,
                    SoundDefOf.PlanetkillerImpact, equipmentDef, def, intendedTarget.Thing, null, 0, 1,
                    null, false, null, 0, 1, 0, false, null, ignoredThings, null, false);
            }
        }
    }
}