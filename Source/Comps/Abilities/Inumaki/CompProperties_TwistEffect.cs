using RimWorld;
using UnityEngine;
using Verse;

namespace JJK
{
    public class CompProperties_TwistEffect : CompProperties_CursedAbilityProps
    {
        public float baseDamage;

        public CompProperties_TwistEffect()
        {
            compClass = typeof(CursedSpeechTwistEffect);
        }
    }


    public class CursedSpeechTwistEffect : BaseCursedEnergyAbility
    {
        public new CompProperties_TwistEffect Props => (CompProperties_TwistEffect)props;

        public override void ApplyAbility(LocalTargetInfo target, LocalTargetInfo dest)
        {
            if (target.Pawn != null)
            {
                float CEScale = JJKUtility.CalcCursedEnergyScalingFactor(parent.pawn, target.Pawn);

                BodyPartRecord targetLimb = JJKUtility.GetRandomLimb(target.Pawn);

                if (targetLimb != null)
                {
                    // Calculate damage
                    float damage = Props.baseDamage * CEScale;

                    // Apply damage to the selected limb
                    DamageInfo dinfo = new DamageInfo(JJKDefOf.JJK_TwistDamage, damage, 1f, -1f, this.parent.pawn, targetLimb);
                    target.Pawn.TakeDamage(dinfo);

                    // Optional: Add a visual or sound effect
                    MoteMaker.ThrowText(target.Pawn.DrawPos, target.Pawn.Map, "TWIST!", Color.red);
                }


            }
        }
    }
}