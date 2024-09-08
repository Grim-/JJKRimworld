using RimWorld;
using Verse;
using Verse.AI;

namespace JJK
{
    public class JobGiver_DemondogAttackTarget : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            if (pawn.jobs.curJob == null ||  pawn.jobs.curJob?.def != JJKDefOf.JJK_DemondogAttackAndVanish)
            {
                DespawnDemondog(pawn, "Invalid job");
                return null;
            }

            LocalTargetInfo target = pawn.CurJob.GetTarget(TargetIndex.B);
            if (!target.IsValid || !(target.Thing is Pawn) || !target.Thing.HostileTo(pawn) || !pawn.CanReach(target, PathEndMode.Touch, Danger.Deadly))
            {
                DespawnDemondog(pawn, "Target invalid or unreachable");
                return null;
            }

            return pawn.jobs.curJob;
        }

        private void DespawnDemondog(Pawn pawn, string reason)
        {
            if (!pawn.Destroyed)
            {
                Log.Message($"JobGiver_DemondogAttackTarget Demondog despawning: {reason}");
                pawn.Destroy();
            }
     
        }
    }
}

    