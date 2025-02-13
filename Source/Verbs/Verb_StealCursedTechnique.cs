using RimWorld;
using Verse;

namespace JJK
{
    public class Verb_StealCursedTechnique : Verb_MeleeAttackDamage
    {
        protected override DamageWorker.DamageResult ApplyMeleeDamageToTarget(LocalTargetInfo target)
        {
            DamageWorker.DamageResult damageResult = base.ApplyMeleeDamageToTarget(target);
            if (target.Thing is Pawn targetPawn)
            {
                targetPawn.health.GetOrAddHediff(JJKDefOf.JJK_SplitSoul);
            }

            return damageResult;
        }
    }
}