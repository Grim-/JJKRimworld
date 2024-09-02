using Verse;
using Verse.AI;

namespace JJK
{
    public class ThinkNode_ConditionalSummonedDefender : ThinkNode_Conditional
    {
        protected override bool Satisfied(Pawn pawn)
        {
            Hediff_Summon summon = (Hediff_Summon)pawn.health.GetOrAddHediff(JJKDefOf.JJK_Shikigami);
            if (summon != null)
            {
                return true;
            }
            return false;
        }
    }
}

    