using RimWorld;
using UnityEngine;
using Verse;

namespace JJK
{
    public class Hediff_CursedReinforcement : HediffWithComps
    {
        public override void PostTick()
        {
            base.PostTick();

            if (pawn.IsHashIntervalTick(2500))
            {
                ScaleSeverity();
            }
        }

        public void ScaleSeverity()
        {
            if (pawn == null) 
                return;

            float sixEyesBonus = CalcSixEyesBonus();
            float maxCECap = JJKMod.CursedEnergyScalingCap;
            float cursedEnergy = pawn.GetStatValue(JJKDefOf.JJK_CursedEnergy);
            cursedEnergy += sixEyesBonus;
            cursedEnergy = Mathf.Clamp(cursedEnergy, 0f, maxCECap);
            float t = cursedEnergy / maxCECap;
            float newSeverity = Mathf.Lerp(0f, 1f, t);
            Severity = newSeverity;
        }

        private float CalcSixEyesBonus()
        {
            return pawn.HasSixEyes() ? JJKMod.SixEyesCursedReinforcementBonus : 0;
        }

        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
            ScaleSeverity();
        }
    }
}


