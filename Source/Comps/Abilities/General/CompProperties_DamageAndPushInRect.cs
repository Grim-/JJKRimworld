using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace JJK
{
    public class CompProperties_DamageAndPushInRect : CompProperties_DamageInRect
    {
        public float pushDistance = 2f;

        public CompProperties_DamageAndPushInRect()
        {
            compClass = typeof(CompAbilityEffect_DamageAndPushInRect);
        }
    }

    public class CompAbilityEffect_DamageAndPushInRect : CompAbilityEffect_DamageInRect
    {
        public new CompProperties_DamageAndPushInRect Props => (CompProperties_DamageAndPushInRect)props;
        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);

            Map map = this.parent.pawn.Map;
            map.effecterMaintainer.AddEffecterToMaintain(
                JJKDefOf.JJK_WaterSpray.Spawn(this.parent.pawn.Position, map, 1f),
                this.parent.pawn,
                new TargetInfo(target.Cell, map),
                169
            );
        }

        protected override void ApplyDamageToTarget(Pawn caster, Thing target)
        {
            base.ApplyDamageToTarget(caster, target);
            if (target is Pawn targetPawn && !targetPawn.Dead)
            {
                TryPushPawn(targetPawn);
            }

        }

        private void TryPushPawn(Pawn targetPawn)
        {
            Rot4 currentRotation = targetPawn.Rotation;

            IntVec3 pushDirection = -currentRotation.FacingCell;

            IntVec3 destinationCell = targetPawn.Position;
            for (int i = 0; i < Props.pushDistance; i++)
            {
                IntVec3 nextCell = destinationCell + pushDirection;
                if (CanPawnMoveToCell(targetPawn, nextCell))
                {
                    destinationCell = nextCell;
                }
                else
                {
                    break;
                }
            }


            if (destinationCell != targetPawn.Position)
            {
                if (targetPawn.Map != null)
                {
                    EffecterDefOf.Fire_SpewShort.Spawn(targetPawn, targetPawn.Map, 3f);
                }
                targetPawn.Position = destinationCell;
                targetPawn.Notify_Teleported();
            }
        }

        private bool CanPawnMoveToCell(Pawn pawn, IntVec3 cell)
        {
            if (!cell.InBounds(pawn.Map))
            {
                return false;
            }


            if (!cell.Walkable(pawn.Map))
            {
                return false;
            }

            List<Thing> thingList = cell.GetThingList(pawn.Map);
            foreach (Thing thing in thingList)
            {
                if (thing.def.fillPercent > 0f && thing != pawn)
                {
                    return false;
                }
            }

            return true;
        }
    }
}