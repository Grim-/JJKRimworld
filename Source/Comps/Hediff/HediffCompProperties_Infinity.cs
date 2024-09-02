using RimWorld;
using System.Collections.Generic;
using Verse;

namespace JJK
{
    public class HediffCompProperties_Infinity : HediffCompProperties
    {
        public float CheckRadius = 4f;
        public int CheckIntervalTicks = 60; // Default to checking every 60 ticks (1 second)

        public HediffCompProperties_Infinity()
        {
            compClass = typeof(HediffComp_Infinity);
        }
    }



    public class HediffComp_Infinity : HediffComp
    {
        public HediffCompProperties_Infinity Props => (HediffCompProperties_Infinity)props;
        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            if (parent.pawn.IsHashIntervalTick(Props.CheckIntervalTicks))
            {
                PerformInfinityEffects();
            }
        }

        public override void Notify_PawnPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            base.Notify_PawnPostApplyDamage(dinfo, totalDamageDealt);
        }

        private void PerformInfinityEffects()
        {
            CheckAndReflectProjectiles();
            // TODO: Add other Infinity effects here
        }

        private void CheckAndReflectProjectiles()
        {
            if (Pawn.Map == null) return;

            IEnumerable<Thing> projectilesInRange = GenRadial.RadialDistinctThingsAround(parent.pawn.Position, parent.pawn.MapHeld, Props.CheckRadius, true);
            foreach (var item in projectilesInRange)
            {
                if (item is Projectile projectile && projectile.Launcher.Faction != Faction.OfPlayer)
                {
                    ProjectileUtility.DeflectProjectile(projectile, parent.pawn);
                }
            }
        }
    }
}


