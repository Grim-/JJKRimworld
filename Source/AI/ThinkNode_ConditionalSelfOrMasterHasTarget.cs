using RimWorld;
using Verse;
using Verse.AI;

namespace JJK
{
    public class ThinkNode_ConditionalSelfOrMasterHasTarget : ThinkNode_Conditional
    {

        //SummonedCreatureManager _SummonedCreatureManager;
        //SummonedCreatureManager SummonedCreatureManager
        //{
        //    get
        //    {
        //        if (_SummonedCreatureManager == null)
        //        {
        //            _SummonedCreatureManager = Find.World.GetComponent<SummonedCreatureManager>();
        //        }

        //        return _SummonedCreatureManager;
        //    }
        //}

        //AbsorbedCreatureManager _AbsorbedCreatureManager;
        //AbsorbedCreatureManager AbsorbedCreatureManager
        //{
        //    get
        //    {
        //        if (_AbsorbedCreatureManager == null)
        //        {
        //            _AbsorbedCreatureManager = Find.World.GetComponent<AbsorbedCreatureManager>();
        //        }

        //        return _AbsorbedCreatureManager;
        //    }
        //}


        //Pawn Master = null;


        protected override bool Satisfied(Pawn pawn)
        {
            if (GenHostility.AnyHostileActiveThreatToPlayer(pawn.Map))
            {
                return true;
            }
            return false;
        }
    }
}

    