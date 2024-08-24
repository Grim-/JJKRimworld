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

                Job luredJob = JobMaker.MakeJob(JJKDefOf.JJK_CursedSpeechLure, parent.pawn);
                luredJob.expiryInterval = Props.lureDurationTicks;
                target.Pawn.jobs.StartJob(luredJob, JobCondition.InterruptForced);

                Log.Message($"Assigned lure job to {target.Pawn.LabelShort}. Current job: {target.Pawn.CurJob?.def.defName}");
            }
        }
    }
}