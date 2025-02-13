using RimWorld;
using Verse;

namespace JJK
{
    public class CompProperties_Gama_Carry : CompProperties_AbilityEffect
    {
        public CompProperties_Gama_Carry()
        {
            compClass = typeof(CompAbilityEffect_Gama_Carry);
        }
    }

    public class CompAbilityEffect_Gama_Carry : CompAbilityEffect
    {
        public new CompProperties_Gama_Carry Props => (CompProperties_Gama_Carry)props;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            Pawn pawn = parent.pawn;
            Map map = pawn.MapHeld;

            if (pawn == null || target.Pawn == null)
            {
                return;
            }

            if (pawn.TryGetComp(out Comp_TenShadowsToad shadowsToad))
            {
                shadowsToad.TryPickupPawn(target.Pawn);
            }
        }
    }
}