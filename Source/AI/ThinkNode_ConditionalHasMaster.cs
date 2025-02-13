using Verse;
using Verse.AI;

namespace JJK
{
    public class ThinkNode_ConditionalHasMaster : ThinkNode_Conditional
    {
        protected override bool Satisfied(Pawn pawn)
        {
            return pawn != null && pawn.IsShikigami() && pawn.GetMaster() != null;
        }
    }

    public class ThinkNode_ConditionalHasNoMaster : ThinkNode_Conditional
    {
        protected override bool Satisfied(Pawn pawn)
        {
            return pawn != null && pawn.IsShikigami() && pawn.GetMaster() == null;
        }
    }
}

    