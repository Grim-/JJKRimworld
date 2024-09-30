using RimWorld;
using System.Linq;
using Verse;

namespace JJK
{
    public class CompProperties_MalevolentShrineDomain : CompProperties_DomainComp
    {
        public CompProperties_MalevolentShrineDomain()
        {
            compClass = typeof(CompMalevolentShrineDomain);
        }
    }

    public class CompMalevolentShrineDomain : CompDomainEffect
    {
        private const int TicksBetweenEffects = 90; 
        public override void ApplyDomainEffects()
        {
            FireDismantleProjectiles();
        }

        public override void CompTick()
        {
            base.CompTick();
            if (Find.TickManager.TicksGame % TicksBetweenEffects == 0)
            {
                FireDismantleProjectiles();
            }
        }

        private void FireDismantleProjectiles()
        {
            var map = parent.Map;
            var validTargets = GetThingsInDomain();

            foreach (var target in validTargets)
            {
                if (ValidateTarget(target))
                {
                    var originCell = GetRandomCellInDomain();

                    FireProjectileAt(originCell, target);
                }
            }
        }

        private bool ValidateTarget(Thing Thing)
        {
            if (Thing.Destroyed)
            {
                return false;
            }

            if (Thing.Faction == Faction.OfPlayerSilentFail)
            {
                return false;
            }

            if (constructedWalls.Contains(Thing))
            {
                return false;
            }

            if (Thing is Pawn Pawn && !Pawn.Destroyed && !Pawn.Dead && !Pawn.IsImmuneToDomainSureHit() && Pawn != DomainCaster)
            {
                return true;
            }

            return true;
        }

        private IntVec3 GetRandomCellInDomain()
        {
            return CellRect.CenteredOn(parent.Position, (int)Props.AreaRadius).Cells
                .Where(c => c.InBounds(parent.Map))
                .RandomElement();
        }

        private void FireProjectileAt(IntVec3 origin, Thing target)
        {
            var proj = (Projectile)GenSpawn.Spawn(JJKDefOf.JJK_DismantleProjectile, origin, parent.Map);
            proj.Launch(parent, origin.ToVector3(), target, target, ProjectileHitFlags.IntendedTarget);
        }

        public override void RemoveDomainEffects()
        {
        
        }
    }
}