using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Verse.DamageWorker;
using Verse;

namespace JJK
{
    public class DamageWorker_Dismantle : DamageWorker_Cut
    {
        protected override BodyPartRecord ChooseHitPart(DamageInfo dinfo, Pawn pawn)
        {
            return pawn.health.hediffSet.GetRandomNotMissingPart(dinfo.Def, dinfo.Height, BodyPartDepth.Outside);
        }

        public override DamageResult Apply(DamageInfo dinfo, Thing thing)
        {
            DamageResult damage = base.Apply(dinfo, thing);

            if (thing is Building building)
            {
                damage.deflectedByMetalArmor = false;
                damage.totalDamageDealt *= 10f;
            }

            return damage;
        }

        protected override void ApplySpecialEffectsToPart(Pawn pawn, float totalDamage, DamageInfo dinfo, DamageResult result)
        {
            BodyPartRecord partToDismember = pawn.health.hediffSet.GetRandomNotMissingPart(dinfo.Def, dinfo.Height, BodyPartDepth.Outside);
            DamageInfo dinfo2 = dinfo;
            dinfo2.SetHitPart(partToDismember);
            FinalizeAndAddInjury(pawn, totalDamage + 999, dinfo2, result);
        }
    }
}