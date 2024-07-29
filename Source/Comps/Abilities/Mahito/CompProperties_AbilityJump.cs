using RimWorld;
using UnityEngine;
using Verse;

namespace JJK
{
    public class CompProperties_AbilityJumper : CompProperties_CursedAbilityProps
    {
        public float flightDuration = 0.5f;
        public HediffDef exhaustionHediffDef;
        public float exhaustionSeverity = 0.25f;

        public CompProperties_AbilityJumper()
        {
            compClass = typeof(CompAbilityEffect_Jump);
        }
    }

    public class CompAbilityEffect_Jump : BaseCursedEnergyAbility
    {
        public new CompProperties_AbilityJumper Props => (CompProperties_AbilityJumper)props;
        public override void ApplyAbility(LocalTargetInfo target, LocalTargetInfo dest)
        {
            Pawn pawn = parent.pawn;
            Map map = pawn.MapHeld;
            IntVec3 position = pawn.Position;
            IntVec3 targetCell = target.Cell;

            if (!CanHitTargetSafely(pawn, map, targetCell))
            {
                Messages.Message("JJK_JumpFailed".Translate(), MessageTypeDefOf.RejectInput, false);
                return;
            }

            PawnFlyer pawnFlyer = PawnFlyer.MakeFlyer(JJKDefOf.JJK_Flyer, pawn, targetCell, null, null);
            GenSpawn.Spawn(pawnFlyer, targetCell, map);

            // Apply any post-jump effects
            if (Props.exhaustionHediffDef != null)
            {
                float severity = pawn.health.hediffSet.HasHediff(Props.exhaustionHediffDef)
                    ? pawn.health.hediffSet.GetFirstHediffOfDef(Props.exhaustionHediffDef).Severity + Props.exhaustionSeverity
                    : Props.exhaustionSeverity;

                HealthUtility.AdjustSeverity(pawn, Props.exhaustionHediffDef, severity);
            }
        }

        private bool CanHitTargetSafely(Pawn Caster, Map map, IntVec3 targetCell)
        {
            if (!targetCell.InBounds(map) || targetCell == Caster.Position)
                return false;

            float distanceToTarget = Caster.Position.DistanceTo(targetCell);
            if (distanceToTarget > parent.def.verbProperties.range)
                return false;

            return targetCell.Walkable(map);
        }
    }
}