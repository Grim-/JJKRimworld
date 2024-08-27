using RimWorld;
using UnityEngine;
using Verse;

namespace JJK
{
    public class CompProperties_AbilityLightningStrike : CompProperties_CursedAbilityProps
    {
        public bool lightning = true;
        public float explosionRadius = 3f;
        public int explosionDamage = 50;
        public SoundDef soundOnImpact;

        public CompProperties_AbilityLightningStrike()
        {
            compClass = typeof(CompAbilityEffect_LightningStrike);
        }
    }

    public class CompAbilityEffect_LightningStrike : BaseCursedEnergyAbility
    {
        public new CompProperties_AbilityLightningStrike Props => (CompProperties_AbilityLightningStrike)props;

        public override void ApplyAbility(LocalTargetInfo target, LocalTargetInfo dest)
        {

            Map map = parent.pawn.Map;
            IntVec3 strikeLocation = target.Cell;

            // Create and fire the lightning strike event
            WeatherEvent_LightningStrike lightningStrike = new WeatherEvent_LightningStrike(map, strikeLocation);
            lightningStrike.FireEvent();

            // Apply additional effects
            if (Props.explosionRadius > 0f)
            {
                GenExplosion.DoExplosion(
                    strikeLocation,
                    map,
                    Props.explosionRadius,
                    DamageDefOf.Bomb,
                    parent.pawn,
                    Props.explosionDamage,
                    -1f
                );
            }
        }

        public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
        {
            return base.Valid(target, throwMessages) && target.Cell.Standable(parent.pawn.Map);
        }
    }
}