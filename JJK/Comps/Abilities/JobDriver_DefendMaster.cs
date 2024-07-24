using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace JJK
{
    public class ThinkNode_ConditionalSummonedDefender : ThinkNode_Conditional
    {
        protected override bool Satisfied(Pawn pawn)
        {
            return pawn.GetMaster() != null;
        }
    }

    public class JobGiver_DefendMaster : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            return JobMaker.MakeJob(JJKDefOf.JJK_DefendMaster);
        }
    }

    public class JobDriver_DefendMaster : JobDriver
    {
        private const float DefendRadius = 10f;
        private const float AttackRadius = 9f;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            Toil defendToil = new Toil();
            defendToil.tickAction = () =>
            {
                Pawn master = Find.World.GetComponent<SummonedCreatureManager>().GetMaster(pawn);
                if (master == null || !master.Spawned)
                {
                    Log.Message($"Ending defend master job for {pawn.LabelShort}: master null or not spawned");
                    this.EndJobWith(JobCondition.Incompletable);
                    return;
                }

                // Move towards master if too far away
                if (this.pawn.Position.DistanceTo(master.Position) > DefendRadius)
                {
                    this.pawn.pather.StartPath(master.Position, PathEndMode.OnCell);
                }
                else
                {
                    // Attack nearby threats
                    Pawn threat = FindNearestThreat();
                    if (threat != null)
                    {
                        this.pawn.meleeVerbs.TryMeleeAttack(threat);
                    }
                    else if (!this.pawn.pather.Moving)
                    {
                        // Randomly move around the master
                        IntVec3 wanderCell = CellFinder.RandomClosewalkCellNear(master.Position, master.Map, 5);
                        this.pawn.pather.StartPath(wanderCell, PathEndMode.OnCell);
                    }
                }
            };
            defendToil.defaultCompleteMode = ToilCompleteMode.Never;
            yield return defendToil;
        }

        private Pawn FindNearestThreat()
        {
            return GenClosest.ClosestThingReachable(
                this.pawn.Position,
                this.pawn.Map,
                ThingRequest.ForGroup(ThingRequestGroup.Pawn),
                PathEndMode.OnCell,
                TraverseParms.For(this.pawn),
                AttackRadius,
                (Thing t) => t is Pawn p && p.HostileTo(this.pawn) && !p.Downed
            ) as Pawn;
        }
    }
}

    