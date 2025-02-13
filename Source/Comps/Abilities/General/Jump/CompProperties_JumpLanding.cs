using RimWorld;
using Verse;

namespace JJK
{
    public class CompProperties_JumpLanding : CompProperties_AbilityEffect
    {
        public float flightDuration = 0.5f;
        public CompProperties_JumpLanding()
        {
            compClass = typeof(CompAbilityEffect_JumpLanding);
        }
    }

    public abstract class CompAbilityEffect_JumpLanding : CompAbilityEffect
    {
        public abstract void OnLand(Pawn Pawn, PawnFlyer PawnFlyer, Map map);
    }
}