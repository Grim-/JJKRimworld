using RimWorld;
using Verse;

namespace JJK
{
    public class ZanpaktoARPStatPart : StatPart
    {
        public override void TransformValue(StatRequest req, ref float val)
        {
            if (req.Thing is ZanpaktoWeapon zanpakto)
            {
                SwordFormDef currentForm = zanpakto.GetSwordFormForState(zanpakto.CurrentState);
                if (currentForm != null)
                {
                    val *= currentForm.ArmourPenMulti;
                }
            }
        }

        public override string ExplanationPart(StatRequest req)
        {
            if (req.Thing is ZanpaktoWeapon zanpakto)
            {
                SwordFormDef currentForm = zanpakto.GetSwordFormForState(zanpakto.CurrentState);
                if (currentForm != null)
                {
                    return $"Zanpakto {zanpakto.CurrentState} form: x{currentForm.ArmourPenMulti.ToStringPercent()}";
                }
            }
            return null;
        }
    }
}