using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace JJK
{
    public class CompProperties_Die : CompProperties_CursedAbilityProps
    {

        public float killThreshold = 1.2f;

        public CompProperties_Die()
        {
            compClass = typeof(CursedSpeechEffectDie);
        }
    }



    public class CursedSpeechEffectDie : BaseCursedEnergyAbility
    {
        public new CompProperties_Die Props => (CompProperties_Die)props;

        public override void ApplyAbility(LocalTargetInfo target, LocalTargetInfo dest)
        {
            if (target.Pawn != null)
            {
                float CEScale = JJKUtility.CalcCursedEnergyScalingFactor(parent.pawn, target.Pawn);

                if (CEScale >= 1f)
                {
                    //enemy is stronger, deal damage to caster
                    target.Pawn.Kill(new DamageInfo(DamageDefOf.Psychic, 999));
                    MoteMaker.ThrowText(parent.pawn.DrawPos, parent.pawn.Map, $"DIE!", Color.green);
                }
                else if (CEScale < 1f)
                {
                    //caster is stronger by atleast 20% kill the enemy target
                    parent.pawn.TakeDamage(new DamageInfo(DamageDefOf.Psychic, 20));
                    MoteMaker.ThrowText(parent.pawn.DrawPos, parent.pawn.Map, $"Ouch!", Color.red);
                }
            }
        }
    }

}