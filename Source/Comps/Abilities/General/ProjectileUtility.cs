using RimWorld;
using UnityEngine;
using Verse;

namespace JJK
{
    public static class ProjectileUtility
    {
        public static void ReflectProjectile(Projectile projectile, Pawn reflector)
        {
            if (projectile.Launcher.Faction != Faction.OfPlayer)
            {
                Thing originalLauncher = projectile.Launcher;
                IntVec3 position = projectile.Position;
                ThingDef projectileDef = projectile.def;
                projectile.Destroy();
                Projectile newProjectile = (Projectile)GenSpawn.Spawn(projectileDef, position, reflector.MapHeld);
                newProjectile.Launch(reflector, reflector.DrawPos, originalLauncher, originalLauncher, ProjectileHitFlags.IntendedTarget);
                EffecterDefOf.Deflect_General_Bullet.Spawn(position, reflector.MapHeld);
            }
        }

        public static void DeflectProjectile(Projectile projectile, Pawn reflector)
        {
            if (projectile.Launcher.Faction != Faction.OfPlayer)
            {
                Thing originalLauncher = projectile.Launcher;
                IntVec3 position = projectile.Position;
                ThingDef projectileDef = projectile.def;
                projectile.Destroy();
                Projectile newProjectile = (Projectile)GenSpawn.Spawn(projectileDef, position, reflector.MapHeld);
                newProjectile.Launch(reflector, reflector.DrawPos, position + new IntVec3(Random.Range(-1, 1), 0, Random.Range(-1, 1)), originalLauncher, ProjectileHitFlags.None);
                EffecterDefOf.Deflect_General_Bullet.Spawn(position, reflector.MapHeld);
            }
        }
    }
}