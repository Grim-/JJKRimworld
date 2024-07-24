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
    internal class Projectile_HollowPurple : Projectile
    {
        public override void Tick()
        {
            base.Tick();

            // Check if the projectile has reached its intended target
            if (DestinationCell == Position)
            {
                // Destroy the projectile
                Destroy();
                return;
            }

            // If the projectile hasn't reached its target, explode at its current position
            ExplodeAtPosition(Position, Map, 3f);
        }

        private void ExplodeAtPosition(IntVec3 pos, Map map, float radius)
        {
            List<Thing> ignoredThings = new List<Thing>();

            // Add pawns of the same faction as the launcher to the ignored list
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