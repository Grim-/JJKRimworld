using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Verse.DamageWorker;
using Verse;

namespace JJK
{
    public class DamageWorker_Dismantle : DamageWorker_AddInjury
    {
        protected override BodyPartRecord ChooseHitPart(DamageInfo dinfo, Pawn pawn)
        {
            return pawn.health.hediffSet.GetRandomNotMissingPart(dinfo.Def, dinfo.Height, BodyPartDepth.Outside);
        }

        protected override void ApplySpecialEffectsToPart(Pawn pawn, float totalDamage, DamageInfo dinfo, DamageResult result)
        {
            // Select a random body part for dismemberment
            BodyPartRecord partToDismember = pawn.health.hediffSet.GetRandomNotMissingPart(dinfo.Def, dinfo.Height, BodyPartDepth.Outside);

            // Apply injury to the selected body part
            DamageInfo dinfo2 = dinfo;
            dinfo2.SetHitPart(partToDismember);
            FinalizeAndAddInjury(pawn, totalDamage + 999, dinfo2, result);
        }
    }
}