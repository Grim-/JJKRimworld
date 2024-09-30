using Verse;

namespace JJK
{
    public class ProjectileCapsule : Projectile
    {
        private CompCapsule capsuleComp;
        private ThingDef capsuleDef;

        public void SetCapsuleComp(Thing Launcher, LocalTargetInfo used, LocalTargetInfo Target, CompCapsule comp, ProjectileHitFlags flags)
        {
            if (comp == null)
            {
                Log.Error("Cannot create capsule projectile, capsule comp passed in is null.");
                return;
            }

            capsuleComp = comp;
            capsuleDef = comp.parent.def;
            this.Launch(Launcher, used, Target, flags);
        }

        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            Map map = Map;
            IntVec3 position = Position;

            // Spawn the capsule at the impact location
            Thing spawnedCapsule = GenSpawn.Spawn(capsuleDef, position, map);
            CompCapsule spawnedComp = spawnedCapsule.TryGetComp<CompCapsule>();

            if (spawnedComp != null)
            {
                spawnedComp.TransferData(capsuleComp);

                if (spawnedComp.State == CompCapsule.CapsuleState.Packed)
                {
                    spawnedComp.Unpack();
                }
            }

            // Destroy the projectile
            this.Destroy();
        }
    }
}