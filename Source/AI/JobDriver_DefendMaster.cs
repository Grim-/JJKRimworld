using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace JJK
{

    public class JobGiver_DefendMaster : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            return JobMaker.MakeJob(JJKDefOf.JJK_DefendMaster);
        }
    }

    public class JobDriver_DefendMaster : JobDriver_SummonBase
    {
        private const float DefendRadius = 10f;
        private const float AttackRadius = 9f;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            Pawn threat = FindNearestThreat();


            Toil defendToil = new Toil();
            defendToil.tickAction = () =>
            {
                if (Summoner == null || !Summoner.Spawned)
                {
                    Log.Message($"Ending defend master job for {pawn.LabelShort}: master null or not spawned");
                    this.EndJobWith(JobCondition.Incompletable);
                    return;
                }

   
                if (threat != null)
                {
                    if (Self.CanReach(threat, PathEndMode.Touch, Danger.Deadly))
                    {
                        Self.Map.attackTargetReservationManager.Reserve(this.pawn, this.job, threat);

                        // If can reach the threat (adjacent), attack
                        if (Self.meleeVerbs.TryGetMeleeVerb(threat) != null)
                        {
                            Self.meleeVerbs.TryMeleeAttack(threat);
                        }
                    }
                    else
                    {
                        // Move towards the threat
                        IntVec3 targetCell = CellFinder.RandomClosewalkCellNear(threat.Position, threat.Map, 1);
                        if (targetCell.InBounds(threat.Map) && Self.Map.pawnDestinationReservationManager.CanReserve(targetCell, Self))
                        {
                                Self.Map.pawnDestinationReservationManager.Reserve(Self, this.job, targetCell);
                              Self.pather.StartPath(targetCell, PathEndMode.Touch);
                        }
                    }
                }
                else
                {
                    // No threat found, behave as before
                    if (!Self.Position.InHorDistOf(Summoner.Position, DefendRadius))
                    {
                        Self.pather.StartPath(Summoner.Position, PathEndMode.OnCell);
                    }
                    else if (!Self.pather.Moving)
                    {
                        IntVec3 wanderCell = CellFinder.RandomClosewalkCellNear(Summoner.Position, Self.Map, 5);
                        Self.pather.StartPath(wanderCell, PathEndMode.OnCell);
                    }
                }
            };
            defendToil.defaultCompleteMode = ToilCompleteMode.Never;
            yield return defendToil;
        }

        private Pawn FindNearestThreat()
        {
            //PawnComponentsUtility

            IAttackTarget attackTarget = AttackTargetFinder.BestAttackTarget(Self, TargetScanFlags.NeedReachable, null, 0, 60);

            if (attackTarget != null && attackTarget.Thing is Pawn pawn)
            {
                return pawn;
            }

            if (Summoner.mindState.meleeThreat != null)
            {
                return Summoner.mindState.meleeThreat;
            }

            if (Summoner.mindState.enemyTarget != null && Summoner.mindState.enemyTarget is Pawn enemyPawn)
            {
                return enemyPawn;
            }

            if (Self.mindState.meleeThreat != null)
            {
                return Self.mindState.meleeThreat;
            }

            if (Self.mindState.enemyTarget != null && Self.mindState.enemyTarget is Pawn SelfenemyPawn)
            {
                return SelfenemyPawn;
            }

            return null;
        }
    }
}

    