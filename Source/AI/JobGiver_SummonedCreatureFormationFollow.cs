using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace JJK
{
    public class JobGiver_SummonedCreatureFormationFollow : JobGiver_AIFollowMaster
    {
        protected override Pawn GetFollowee(Pawn pawn)
        {
            if (pawn.IsShikigami())
            {
                return pawn.GetMaster();
            }
            return null;
        }

        protected override float GetRadius(Pawn pawn)
        {
            return 3f;
        }

        protected override Job TryGiveJob(Pawn pawn)
        {
            Pawn followee = GetFollowee(pawn);
            if (followee == null)
            {
                Log.Error($"Followee is null for {pawn.LabelShort}");
                return null;
            }

            if (!followee.Spawned)
            {
                Log.Message($"Followee {followee.LabelShort} is not spawned");
                return null;
            }

            var TenShadowGene = followee.genes.GetFirstGeneOfType<TenShadowGene>();
            var shadows = TenShadowGene.GetAllActiveShadows();
            if (shadows == null || !shadows.Contains(pawn))
            {
                return null;
            }
            if (!JobDriver_FormationFollow.FarEnoughAndPossibleToStartJob(pawn, followee, TenShadowGene, GetRadius(pawn)))
            {
                return null;
            }

            Job job = JobMaker.MakeJob(JJKDefOf.JJK_FormationFollow, followee);
            job.expiryInterval = 200;
            job.followRadius = 4;
            job.SetTarget(TargetIndex.A, followee);
            job.reportStringOverride = "Following in formation";
            return job;
        }
    }
    public class JobDriver_FormationFollow : JobDriver_FollowClose
    {
        protected TenShadowGene TenShadows => this.TargetA.Pawn.genes.GetFirstGeneOfType<TenShadowGene>();

        private List<Pawn> GetAllActiveShadows()
        {
            return TenShadows.GetAllActiveShadows();
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedOrNull(TargetIndex.A);
            Toil formationToil = ToilMaker.MakeToil("FormationFollow");

            formationToil.tickAction = () =>
            {
                Pawn followee = this.TargetA.Pawn;
                float followRadius = this.job.followRadius;

                if (!this.pawn.pather.Moving || this.pawn.IsHashIntervalTick(30))
                {
                    List<Pawn> activeShadows = GetAllActiveShadows();
                    int formationIndex = activeShadows.IndexOf(this.pawn);

                    if (formationIndex == -1)
                    {
                        base.EndJobWith(JobCondition.Errored);
                        return;
                    }

                    IntVec3 targetCell = FormationUtils.GetFormationPosition(
                        FormationUtils.FormationType.Column,
                        followee.Position.ToVector3(),
                        followee.Rotation,
                        formationIndex,
                        activeShadows.Count);

                    if (this.pawn.Position != targetCell)
                    {
                        if (!this.pawn.CanReach(targetCell, PathEndMode.OnCell, Danger.Deadly))
                        {
                            for (int i = 0; i < 6; i++)
                            {
                                targetCell = CellFinder.StandableCellNear(targetCell, this.Map, 5);
                                if (!this.pawn.CanReach(targetCell, PathEndMode.OnCell, Danger.Deadly))
                                {                                
                                    continue;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }

                        if (!this.pawn.CanReach(targetCell, PathEndMode.OnCell, Danger.Deadly))
                        {
                            base.EndJobWith(JobCondition.Incompletable);
                            return;
                        }

                        this.pawn.pather.StartPath(targetCell, PathEndMode.OnCell);
                        this.locomotionUrgencySameAs = followee;
                    }
                    else if (!followee.pather.Moving)
                    {
                        base.EndJobWith(JobCondition.Succeeded);
                        return;
                    }
                }
            };

            formationToil.defaultCompleteMode = ToilCompleteMode.Never;
            yield return formationToil;
        }

        public override bool IsContinuation(Job j)
        {
            return this.job.GetTarget(TargetIndex.A) == j.GetTarget(TargetIndex.A);
        }

        public static new bool FarEnoughAndPossibleToStartJob(Pawn follower, Pawn followee, TenShadowGene Gene, float radius)
        {
            if (radius <= 0f)
            {
                Log.ErrorOnce($"Checking formation follow job with radius <= 0. pawn={follower.ToStringSafe<Pawn>()}",
                    follower.thingIDNumber ^ 843254009);
                return false;
            }

            // Check if the formation position is reachable
            var shadows = Gene.GetAllActiveShadows();
            int index = shadows.IndexOf(follower);

            if (index == -1)
                return false;

            Vector3 targetPos = followee.Position.ToVector3() + new Vector3(index - 2, 0, -index / 5);
            IntVec3 targetCell = targetPos.ToIntVec3();

            return follower.CanReach(targetCell, PathEndMode.OnCell, Danger.Deadly);
        }
    }
}

    