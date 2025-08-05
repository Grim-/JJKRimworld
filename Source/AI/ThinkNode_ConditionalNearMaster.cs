//using System.Collections.Generic;
//using Verse;
//using Verse.AI;

//namespace JJK
//{
//    public class ThinkNode_ConditionalNearMaster : ThinkNode_Conditional
//    {
//        public float MaxDistanceToMaster = 5;

//        protected override bool Satisfied(Pawn pawn)
//        {
//            if (pawn != null && pawn.IsShikigami() && pawn.GetMaster() != null)
//            {
//                Pawn master = pawn.GetMaster();
//                return pawn.Position.DistanceTo(master.Position) <= MaxDistanceToMaster;
//            }
//            return false;
//        }
//    }

//    public class ThinkNode_ConditionalNotInFormation : ThinkNode_Conditional
//    {
//        public float MaxDistanceToMaster = 2;

//        protected override bool Satisfied(Pawn pawn)
//        {
//            Pawn Master = pawn.GetMaster();

//            if (pawn != null && pawn.IsShikigami() && Master != null)
//            {
//                if (pawn.TryGetComp(out Comp_TenShadowsSummon shadowsSummon))
//                {
//                    IntVec3 formationPosition = FormationUtils.GetFormationPosition(shadowsSummon.TenShadowsUser.FormationType,
//                        Master.Position.ToVector3(), 
//                        Master.Rotation, 
//                        shadowsSummon.ShikigamiData.ActiveShadows.IndexOf(pawn), 
//                        shadowsSummon.ShikigamiData.ActiveShadows.Count);

//                    return pawn.Position.DistanceTo(formationPosition) > shadowsSummon.TenShadowsUser.FormationRadius;
//                }

//                return pawn.Position.DistanceTo(Master.Position) > MaxDistanceToMaster;
//            }
//            return false;
//        }
//    }
//}

    