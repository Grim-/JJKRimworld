using RimWorld;
using Verse;

namespace JJK
{
    public class ZanpaktoCooldownStatPart : StatPart
    {
        private ZanpaktoWeapon Zanpakto = null;

        public override void TransformValue(StatRequest req, ref float val)
        {
            if (req.Thing is ZanpaktoWeapon zanpakto)
            {
                SwordFormDef currentForm = zanpakto.GetSwordFormForState(zanpakto.CurrentState);
                if (currentForm != null)
                {
                    val *= currentForm.CooldownMulti;
                }
            }
            else if(req.Thing is Pawn pawn)
            {
                ZanpaktoWeapon zanpaktoWeapon = pawn.GetZanpaktoWeapon();

                if (zanpaktoWeapon != null)
                {
                    SwordFormDef currentForm = zanpaktoWeapon.GetSwordFormForState(zanpaktoWeapon.CurrentState);
                    val *= currentForm.CooldownMulti;
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
                    return $"Zanpakto {zanpakto.CurrentState} form: x{currentForm.CooldownMulti.ToStringPercent()}";
                }
            }
            else if (req.Thing is Pawn pawn)
            {
                ZanpaktoWeapon zanpaktoWeapon = pawn.GetZanpaktoWeapon();
                if (zanpaktoWeapon != null)
                {
                    SwordFormDef currentForm = zanpaktoWeapon.GetSwordFormForState(zanpaktoWeapon.CurrentState);
                    return $"Zanpakto {zanpaktoWeapon.CurrentState} form: x{currentForm.CooldownMulti.ToStringPercent()}";
                }
            }
            return null;
        }
    }
}