using RimWorld;
using Verse;

namespace JJK
{
    public class CompProperties_ZanpaktoBankai : CompProperties_AbilityEffect
    {
        public CompProperties_ZanpaktoBankai()
        {
            compClass = typeof(CompAbilityEffect_ZanpaktoBankai);
        }
    }

    public class CompAbilityEffect_ZanpaktoBankai : CompAbilityEffect
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
            Zanpakto.SetState(ZanpaktoState.Bankai);
        }
    }
}