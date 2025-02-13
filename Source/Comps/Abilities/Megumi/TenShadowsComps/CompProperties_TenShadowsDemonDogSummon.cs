using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace JJK
{
    public class CompProperties_TenShadowsDemonDogSummon : CompProperties
    {
        public CompProperties_TenShadowsDemonDogSummon()
        {
            compClass = typeof(Comp_TenShadowsDemonDog);
        }
    }


    public class Comp_TenShadowsDemonDog : Comp_TenShadowsSummon
    {
        public override void Notify_Killed(Map prevMap, DamageInfo? dinfo = null)
        {
            base.Notify_Killed(prevMap, dinfo);

            if (_TenShadowsUser != null)
            {
                _TenShadowsUser.ShouldSummonTotalityDivineDog = true;
            }
        }

        public override void OnTargetSummonAction(Pawn Master, Thing Target)
        {
            base.OnTargetSummonAction(Master, Target);

            if (Target != null)
            {
                Job Job = JobMaker.MakeJob(JJKDefOf.JJK_DemondogAttackAndVanish);
                Job.SetTarget(TargetIndex.A, ParentPawn);
                Job.SetTarget(TargetIndex.B, Target);
                ParentPawn.jobs.StartJob(Job, JobCondition.InterruptForced);
            }
        }
    }



}



