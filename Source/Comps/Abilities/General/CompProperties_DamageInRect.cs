using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace JJK
{
    public class CompProperties_DamageInRect : CompProperties_AbilityEffect
    {
        public float rectWidth = 3f;
        public float rectLength = 15f;
        public DamageDef damageType;
        public int damageAmount = 10;
        public bool useMouseAsOrigin = false;

        public CompProperties_DamageInRect()
        {
            compClass = typeof(CompAbilityEffect_DamageInRect);
        }
    }

    public class CompAbilityEffect_DamageInRect : CompAbilityEffect
    {
        public new CompProperties_DamageInRect Props => (CompProperties_DamageInRect)props;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            Pawn pawn = parent.pawn;
            Vector3 origin = GetOrigin(pawn, target);
            float angle = GetAngleToTarget(origin, target.Cell.ToVector3());
            List<Thing> targets = GetTargetsInRect(pawn.Map, origin, angle);
            ApplyDamageToTargets(pawn, targets);
        }

        protected virtual Vector3 GetOrigin(Pawn pawn, LocalTargetInfo target)
        {
            return Props.useMouseAsOrigin ? target.CenterVector3 : pawn.DrawPos;
        }

        protected virtual float GetAngleToTarget(Vector3 origin, Vector3 target)
        {
            Vector3 direction = target - origin;
            return Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        }

        protected virtual List<Thing> GetTargetsInRect(Map map, Vector3 origin, float angle)
        {
            return RotatedRectTargetFinder.GetTargetsInRotatedRect(map, origin, Props.rectWidth, Props.rectLength, angle);
        }

        protected virtual void ApplyDamageToTargets(Pawn caster, List<Thing> targets)
        {
            foreach (Thing thing in targets)
            {
                if (thing != caster && !thing.Destroyed)
                {
                    ApplyDamageToTarget(caster, thing);
                }
            }
        }

        protected virtual void ApplyDamageToTarget(Pawn caster, Thing target)
        {
            DamageInfo dinfo = new DamageInfo(Props.damageType, Props.damageAmount, 0, -1, caster);
            target.TakeDamage(dinfo);
        }

        public override void DrawEffectPreview(LocalTargetInfo target)
        {
            Pawn pawn = parent.pawn;
            Vector3 origin = GetOrigin(pawn, target);
            float angle = GetAngleToTarget(origin, target.Cell.ToVector3());
            List<IntVec3> cells = RotatedRectTargetFinder.GetCellsInRotatedRect(pawn.Map, origin, Props.rectWidth, Props.rectLength, angle);
            GenDraw.DrawFieldEdges(cells, Color.red);
        }
    }


}