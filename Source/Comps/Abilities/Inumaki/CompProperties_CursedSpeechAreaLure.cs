using RimWorld;
using System.Collections.Generic;
using Verse.AI;
using Verse;
using System.Linq;
using UnityEngine;

namespace JJK
{
    public class CompProperties_CursedSpeechAreaLure : CompProperties_CursedAbilityProps
    {
        public float radius = 10f;
        public int maxTargets = 10;
        public int lureDurationTicks = 1250;

        public CompProperties_CursedSpeechAreaLure()
        {
            compClass = typeof(CompAbilityEffect_CursedSpeechAreaLure);
        }
    }



    public class CompAbilityEffect_CursedSpeechAreaLure : BaseCursedEnergyAbility
    {
        public new CompProperties_CursedSpeechAreaLure Props => (CompProperties_CursedSpeechAreaLure)props;


        public override void ApplyAbility(LocalTargetInfo target, LocalTargetInfo dest)
        {
            if (target.IsValid && target.Cell.IsValid)
            {
                MoteMaker.ThrowText(parent.pawn.DrawPos, parent.pawn.Map, $"APPROACH!", Color.green);
                LurePawnsInArea(target.Cell);
            }
        }

        public override void DrawEffectPreview(LocalTargetInfo target)
        {
            base.DrawEffectPreview(target);

            if (target.IsValid)
            {
                GenDraw.DrawRadiusRing(target.Cell, Props.radius);
            }
        }

        private void LurePawnsInArea(IntVec3 center)
        {
            Map map = parent.pawn.Map;
            IEnumerable<Pawn> pawnsToLure = GetEnemyPawnsInRange(center, map, Props.radius)
                .Take(Props.maxTargets);

            Log.Message($"Attempting to lure pawns. Found {pawnsToLure.Count()} potential targets.");

            foreach (Pawn enemyPawn in pawnsToLure)
            {
                Job luredJob = JobMaker.MakeJob(JJKDefOf.JJK_CursedSpeechLure, parent.pawn);
                luredJob.expiryInterval = Props.lureDurationTicks;
                enemyPawn.jobs.StartJob(luredJob, JobCondition.InterruptForced);

                Log.Message($"Assigned lure job to {enemyPawn.LabelShort}. Current job: {enemyPawn.CurJob?.def.defName}");
            }
        }

        private IEnumerable<Pawn> GetEnemyPawnsInRange(IntVec3 center, Map map, float radius)
        {
            return GenRadial.RadialCellsAround(center, radius, true)
                .SelectMany(c => c.GetThingList(map))
                .OfType<Pawn>()
                .Where(p => p.Faction != null && p.Faction != Faction.OfPlayer);
        }

    }
}