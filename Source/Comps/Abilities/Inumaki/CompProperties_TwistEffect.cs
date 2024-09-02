using RimWorld;
using System.Collections.Generic;
using System.Linq;
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

                TwistTargetLimb(target.Pawn, parent.pawn, Props.baseDamage, CEScale);
            }
        }

        public static void TwistTargetLimb(Pawn Target, Pawn Caster, float BaseDamage, float Scale)
        {
            BodyPartRecord targetLimb = JJKUtility.GetRandomLimb(Target);

            if (targetLimb != null)
            {
                // Calculate damage
                float damage = BaseDamage * Scale;

                // Apply damage to the selected limb
                DamageInfo dinfo = new DamageInfo(JJKDefOf.JJK_TwistDamage, damage, 1f, -1f, Caster, targetLimb);
                Target.TakeDamage(dinfo);

                // Optional: Add a visual or sound effect
                MoteMaker.ThrowText(Caster.DrawPos, Caster.Map, "TWIST!", Color.red);
            }
        }
    }

}