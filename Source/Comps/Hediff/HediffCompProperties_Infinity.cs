using RimWorld;
using System.Collections.Generic;
using Verse;

namespace JJK
{
    public class HediffCompProperties_Infinity : HediffCompProperties
    {
        public float CheckRadius = 4f;
        public int CheckIntervalTicks = 60; // Default to checking every 60 ticks (1 second)
        public EffecterDef Effect;

        public HediffCompProperties_Infinity()
        {
            compClass = typeof(HediffComp_Infinity);
        }
    }



    public class HediffComp_Infinity : HediffComp
    {
        public HediffCompProperties_Infinity Props => (HediffCompProperties_Infinity)props;
        private Effecter AuraEffect = null;


        public override void CompPostMake()
        {
            base.CompPostMake();

            if (Props.Effect != null)
            {
                AuraEffect = Props.Effect.Spawn(this.Pawn, this.Pawn.MapHeld);
            }
        }


        public override void CompPostPostRemoved()
        {
            base.CompPostPostRemoved();

            if (AuraEffect != null)
            {
                AuraEffect.Cleanup();
                AuraEffect = null;
            }
        }


        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);

            if (AuraEffect != null)
            {
                AuraEffect.EffectTick(this.Pawn, this.Pawn);
            }

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
            CheckAndPushEnemyPawns();
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

        private void CheckAndPushEnemyPawns()
        {
            if (parent.pawn.Map == null) return;

            foreach (IntVec3 cell in GenAdj.CellsAdjacent8Way(new TargetInfo(parent.pawn.Position, parent.pawn.MapHeld)))
            {
                if (cell.InBounds(parent.pawn.Map))
                {
                    Pawn enemyPawn = cell.GetFirstPawn(parent.pawn.Map);
                    if (enemyPawn != null && enemyPawn.Faction != Faction.OfPlayer && enemyPawn.Faction.HostileTo(Faction.OfPlayer))
                    {
                        PushEnemyPawn(enemyPawn);
                    }
                }
            }
        }

        private void PushEnemyPawn(Pawn enemyPawn)
        {
            IntVec3 pushDirection = parent.pawn.Rotation.FacingCell;
            //IntVec3 pushDirection = (enemyPawn.Position - parent.pawn.Position);
            IntVec3 targetCell = enemyPawn.Position + pushDirection;

            if (targetCell.InBounds(parent.pawn.Map))
            {
                bool isObstructed = IsWallOrBuildingPresent(targetCell);

                if (!isObstructed)
                {
                    enemyPawn.Position = targetCell;
                    enemyPawn.Notify_Teleported(true, false);
                }
                else
                {
                    if (Rand.Range(0, 100) <= 80)
                    {
                        DamageInfo damageInfo = new DamageInfo(DamageDefOf.Vaporize, 100f, 0f, -1f, parent.pawn);
                        enemyPawn.TakeDamage(damageInfo);
                        Messages.Message($"SPLAT {enemyPawn.Label} has been turned into a mess on the wall.", MessageTypeDefOf.NeutralEvent);
                        EffecterDefOf.MeatExplosionExtraLarge.Spawn(targetCell, parent.pawn.Map);
                    }
                }
            }
        }

        private bool IsWallOrBuildingPresent(IntVec3 cell)
        {
            if (cell.InBounds(parent.pawn.Map))
            {
                List<Thing> thingList = cell.GetThingList(parent.pawn.Map);
                foreach (Thing thing in thingList)
                {
                    if (thing is Building && (thing.def.fillPercent == 1f || thing.def.passability == Traversability.Impassable))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public override void CompExposeData()
        {
            base.CompExposeData();

            // Save whether the AuraEffect is active
            bool auraEffectActive = AuraEffect != null;
            Scribe_Values.Look(ref auraEffectActive, "AuraEffectActive", false);

            // When loading, recreate the AuraEffect if it was active
            if (Scribe.mode == LoadSaveMode.PostLoadInit && auraEffectActive && Props.Effect != null)
            {
                AuraEffect = Props.Effect.SpawnAttached(this.Pawn, this.Pawn.MapHeld);
            }
        }
    }
}


