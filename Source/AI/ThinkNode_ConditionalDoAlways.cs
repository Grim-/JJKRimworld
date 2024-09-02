using Verse;
using Verse.AI;

namespace JJK
{
    public class ThinkNode_ConditionalDoAlways : ThinkNode_Conditional
    {
        protected override bool Satisfied(Pawn pawn)
        {
            return true;
        }
    }
}

    