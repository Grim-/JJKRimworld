using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace JJK
{
    public class CompProperties_DamageInRect : CompProperties_AbilityEffect
    {
        public float rectWidth = 10f;
        public float rectLength = 2f;
        public DamageDef damageType = DamageDefOf.Cut;
        public int damageAmount = 10;
        public bool useMouseAsOrigin = true;

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
            Vector3 origin = Props.useMouseAsOrigin ? target.CenterVector3 : pawn.DrawPos;
            float angle = GetAngleToTarget(origin, target.Cell.ToVector3());
            List<Thing> targets = TargetFinderUtil.GetTargetsInCone(pawn.Map, origin, 15, 90f, angle, 0, 5);
            foreach (Thing thing in targets)
            {
                Pawn targetPawn = thing as Pawn;
                if (targetPawn != null && targetPawn != pawn)
                {
                    DamageInfo dinfo = new DamageInfo(Props.damageType, Props.damageAmount, 0, -1, pawn);
                    targetPawn.TakeDamage(dinfo);
                    MoteMaker.ThrowText(targetPawn.DrawPos, targetPawn.Map, "Damaged!", Color.red);
                }
            }
        }

        public override void DrawEffectPreview(LocalTargetInfo target)
        {
            Pawn pawn = parent.pawn;
            Vector3 origin = Props.useMouseAsOrigin ? target.CenterVector3 : pawn.DrawPos;
            float angle = GetAngleToTarget(origin, target.Cell.ToVector3());
            List<IntVec3> cells = TargetFinderUtil.GetCellsInCone(pawn.Map, origin, 15, 90f, angle, 0, 5);

            GenDraw.DrawFieldEdges(cells, Color.red);
        }

        private float GetAngleToTarget(Vector3 origin, Vector3 target)
        {
            Vector3 direction = target - origin;
            float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            return angle;
        }
    }
}