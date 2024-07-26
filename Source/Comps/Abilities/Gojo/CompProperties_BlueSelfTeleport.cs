using RimWorld;
using Verse;

namespace JJK
{
    public class CompProperties_BlueSelfTeleport : CompProperties_CursedAbilityProps
    {
        public float MaxRange = 20f;

        public CompProperties_BlueSelfTeleport()
        {
            compClass = typeof(CompAbilityEffect_BlueSelfTeleport);
        }
    }

    public class CompAbilityEffect_BlueSelfTeleport : BaseCursedEnergyAbility
    {
        public new CompProperties_BlueSelfTeleport Props
        {
            get
            {
                return (CompProperties_BlueSelfTeleport)this.props;
            }
        }

        public override bool AICanTargetNow(LocalTargetInfo target)
        {
            return target.Cell.InBounds(parent.pawn.Map) && target.Cell.Walkable(parent.pawn.Map);
        }

        public override void ApplyAbility(LocalTargetInfo target, LocalTargetInfo dest)
        {
            if (!target.Cell.InBounds(parent.pawn.Map) || !target.Cell.Walkable(parent.pawn.Map))
            {
                return;
            }

            Pawn caster = parent.pawn;
            IntVec3 originalPosition = caster.Position;
            Map map = caster.Map;

            // Perform the teleportation
            caster.Position = target.Cell;
            caster.Notify_Teleported();

            // Visual effects (you can customize these)
            FleckMaker.ThrowDustPuff(originalPosition, map, 1.0f);
            FleckMaker.ThrowDustPuff(target.Cell, map, 1.0f);

            // Consume Cursed Energy
            float distanceTraveled = (target.Cell - originalPosition).LengthHorizontal;
            float energyCost = distanceTraveled * 0.01f; // Adjust this multiplier as needed
            caster.GetCursedEnergy()?.ConsumeCursedEnergy(energyCost);
        }

        public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
        {
            return base.CanApplyOn(target, dest) &&
                   target.Cell.InBounds(parent.pawn.Map) &&
                   target.Cell.Walkable(parent.pawn.Map) &&
                   (target.Cell - parent.pawn.Position).LengthHorizontal <= Props.MaxRange;
        }
    }
}