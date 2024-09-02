using RimWorld;
using Verse;

namespace JJK
{
    public class CompProperties_TeleportMeleeAttack : CompProperties_AbilityEffect
    {
        public CompProperties_TeleportMeleeAttack()
        {
            compClass = typeof(CompAbilityEffect_TeleportMeleeAttack);
        }
    }
    public class CompAbilityEffect_TeleportMeleeAttack : CompAbilityEffect
    {
        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            if (target.Thing != null)
            {
                parent.pawn.SetPositionDirect(target.Thing.RandomAdjacentCell8Way());
                parent.pawn.Notify_Teleported();
                if (parent.pawn.meleeVerbs.TryMeleeAttack(target.Thing, null, false))
                {

                }
            }
        }
    }
}