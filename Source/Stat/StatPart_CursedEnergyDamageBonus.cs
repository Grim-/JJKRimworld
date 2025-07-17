using RimWorld;
using UnityEngine;

namespace JJK
{
    public class StatPart_CursedEnergyDamageBonus : StatPart
    {
        public override void TransformValue(StatRequest req, ref float val)
        {
            if (req.Pawn != null && req.Pawn.TrYGetCursedEnergy(out Gene_CursedEnergy _CursedEnergy))
            {
                float maxBonus = GetMaxBonus(_CursedEnergy);
                val += maxBonus;
            }
        }
        private float GetMaxBonus(Gene_CursedEnergy _CursedEnergy)
        {
            return Mathf.Lerp(0, 50, _CursedEnergy.TotalUsedCursedEnergy / _CursedEnergy.Max);
        }

        public override string ExplanationPart(StatRequest req)
        {
            string baseString = "";

            if (req.Pawn != null && req.Pawn.TrYGetCursedEnergy(out Gene_CursedEnergy _CursedEnergy))
            {
                float maxBonus = GetMaxBonus(_CursedEnergy);
                baseString = $"+{maxBonus} from cursed energy refinement";
            }

            return baseString;
        }
    }
}
