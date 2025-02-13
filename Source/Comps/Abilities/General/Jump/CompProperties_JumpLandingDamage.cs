using RimWorld;
using System.Linq;
using Verse;

namespace JJK
{
    public class CompProperties_JumpLandingDamage : CompProperties_JumpLanding
    {
        public float damage;
        public DamageDef damageType;
        public float radius = 1f;
        public bool canTargetFriendly = false;

        public CompProperties_JumpLandingDamage()
        {
            compClass = typeof(CompAbilityEffect_JumpLandingDamage);
        }
    }

    public class CompAbilityEffect_JumpLandingDamage : CompAbilityEffect_JumpLanding
    {
        new CompProperties_JumpLandingDamage Props => (CompProperties_JumpLandingDamage)props;

        public override void OnLand(Pawn Pawn, PawnFlyer PawnFlyer, Map map)
        {
            Log.Message($"OnLand - Pawn: {Pawn}, Pos: {Pawn.Position}, PawnMap: {Pawn.Map}, FlyerMap: {PawnFlyer.Map}");


            foreach (var item in JJKUtility.GetThingsInRange(PawnFlyer.Position, PawnFlyer.Map, Props.radius, (Thing thing) =>
            {
                Log.Message($"Validating thing: {thing}");
                return ValidateTarget(thing, Pawn, PawnFlyer);
            }).ToList())
            {
                item.TakeDamage(new DamageInfo(Props.damageType != null ? Props.damageType : DamageDefOf.Blunt, Props.damage));
            }
        }

        private bool ValidateTarget(Thing Target, Pawn Pawn, PawnFlyer PawnFlyer)
        {
            return Target.def.useHitPoints && Target != Pawn && Target != PawnFlyer;
        }
    }
}