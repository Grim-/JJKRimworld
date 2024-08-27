using RimWorld;
using Verse;

namespace JJK
{
    public class CompProperties_ZanpaktoSeal : CompProperties_AbilityEffect
    {
        public CompProperties_ZanpaktoSeal()
        {
            compClass = typeof(Zanpakto_SealAbility);
        }
    }

    public class Zanpakto_SealAbility : CompAbilityEffect
    {
        private ZanpaktoWeapon Zanpakto => parent.pawn.equipment.Primary as ZanpaktoWeapon;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            if (Zanpakto == null)
            {
                Log.Error("CompAbilityEffect_ZanpaktoRelease: Primary weapon is not a ZanpaktoWeapon");
                return;
            }

            Log.Message($"CompAbilityEffect_ZanpaktoRelease applying to {Zanpakto}");
            Zanpakto.SetState(ZanpaktoState.Sealed);
        }
    }
}