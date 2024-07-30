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
            yield return new Toil
            {
                initAction = () =>
                {
                    Pawn master = Find.World.GetComponent<SummonedCreatureManager>().GetMasterFor(pawn);
                    if (master == null || !master.Spawned)
                    {
                        //master = Current.Game.GetComponent<AbsorbedCreatureManager>().GetMasterForAbsorbedCreature(pawn);
                        if (master == null || !master.Spawned)
                        {
                            Log.Message($"Ending defend master job for {pawn.LabelShort}: master null or not spawned");
                            this.EndJobWith(JobCondition.Incompletable);
                            return;
                        }
                    }
                    job.SetTarget(TargetIndex.A, master);
                }
            };

            Toil defendToil = new Toil();
            defendToil.tickAction = () =>
            {
                Pawn master = job.GetTarget(TargetIndex.A).Pawn;
                if (master == null || !master.Spawned)
                {
                    Log.Message($"Ending defend master job for {pawn.LabelShort}: master null or not spawned");
                    this.EndJobWith(JobCondition.Incompletable);
                    return;
                }

                Pawn threat = FindNearestThreat();
                if (threat != null)
                {
                    // If there's a threat, prioritize attacking it
                    if (pawn.CanReach(threat, PathEndMode.Touch, Danger.Deadly))
                    {
                        // If in melee range, attack
                        pawn.meleeVerbs.TryMeleeAttack(threat);
                    }
                    else
                    {
                        // Move towards the threat
                        pawn.pather.StartPath(threat.Position, PathEndMode.Touch);
                    }
                }
                else if (pawn.Position.DistanceTo(master.Position) > DefendRadius)
                {
                    // If no immediate threat and too far from master, move closer
                    pawn.pather.StartPath(master.Position, PathEndMode.OnCell);
                }
                else if (!pawn.pather.Moving)
                {
                    // If close to master and no threat, patrol around
                    IntVec3 wanderCell = CellFinder.RandomClosewalkCellNear(master.Position, master.Map, 5);
                    pawn.pather.StartPath(wanderCell, PathEndMode.OnCell);
                }
            };
            defendToil.defaultCompleteMode = ToilCompleteMode.Never;
            yield return defendToil;
        }

        private Pawn FindNearestThreat()
        {
            return GenClosest.ClosestThingReachable(
                pawn.Position,
                pawn.Map,
                ThingRequest.ForGroup(ThingRequestGroup.Pawn),
                PathEndMode.Touch,
                TraverseParms.For(pawn),
                9999f,
                (Thing p) => p is Pawn enemy && enemy.Faction != null && enemy.Faction.HostileTo(pawn.Faction) && !enemy.Downed
            ) as Pawn;
        }
    }
}

    