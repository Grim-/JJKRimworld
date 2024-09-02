using RimWorld;
using UnityEngine;
using Verse.AI;
using Verse;

namespace JJK
{
    public class CompProperties_CursedSpeechLure : CompProperties_CursedAbilityProps
    {
        public int lureDurationTicks = 1250;

        public CompProperties_CursedSpeechLure()
        {
            compClass = typeof(CompAbilityEffect_CursedSpeechLure);
        }
    }

    public class CompAbilityEffect_CursedSpeechLure : BaseCursedEnergyAbility
    {
        public new CompProperties_CursedSpeechLure Props => (CompProperties_CursedSpeechLure)props;

        public override void ApplyAbility(LocalTargetInfo target, LocalTargetInfo dest)
        {
            if (target.IsValid && target.Pawn != null)
            {
                MoteMaker.ThrowText(parent.pawn.DrawPos, parent.pawn.Map, $"APPROACH!", Color.green);

                LureTarget(target.Pawn, parent.pawn, Props.lureDurationTicks);
            }
        }


        public static void LureTarget(Pawn Target, Pawn Lure, int DurationTicks)
        {
            Job luredJob = JobMaker.MakeJob(JJKDefOf.JJK_CursedSpeechLure, Lure);
            luredJob.expiryInterval = DurationTicks;
            Target.jobs.StartJob(luredJob, JobCondition.InterruptForced);
            //Log.Message($"Assigned lure job to {Target.LabelShort}. Current job: {Target.CurJob?.def.defName}");
        }
    }
}