using Verse;

namespace JJK
{
    public class Verb_ThrowCapsule : Verb
    {
        CompCapsule capsuleComp => EquipmentSource?.GetComp<CompCapsule>();

        public override bool Available()
        {
            bool baseAvailable = base.Available();
            return baseAvailable && EquipmentSource != null && EquipmentSource.GetComp<CompCapsule>() != null;
        }

        protected override bool TryCastShot()
        {
            if (currentTarget.HasThing && currentTarget.Thing.Map != caster.Map)
            {
                return false;
            }

            CompCapsule capsuleComp = EquipmentSource?.GetComp<CompCapsule>();
            if (capsuleComp == null)
            {
                return false;
            }

            ThrowCapsule(capsuleComp, currentTarget);
            return true;
        }
        public override void OrderForceTarget(LocalTargetInfo target)
        {
            base.OrderForceTarget(target);
            currentTarget = target;
            TryCastShot();
        }

        private void ThrowCapsule(CompCapsule capsuleComp, LocalTargetInfo target)
        {
            ProjectileCapsule projectile = (ProjectileCapsule)GenSpawn.Spawn(ThingDef.Named("JJK_ProjectileCapsule"), caster.Position, caster.Map);

            projectile.SetCapsuleComp(caster, target, target, capsuleComp, ProjectileHitFlags.IntendedTarget);

            (caster as Pawn)?.equipment.Remove(capsuleComp.parent);
        }
    }
}