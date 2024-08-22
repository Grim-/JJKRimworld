using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace JJK
{
    public class CompProperties_CursedSpeechEffect : CompProperties_CursedAbilityProps
    {
        public HediffDef hediffToApply;
        public int minDurationTicks = 60;
        public int maxDurationTicks = 3600;
        public float baseEffectStrength = 1f;
        public bool scaleSeverity = false;

        public CompProperties_CursedSpeechEffect()
        {
            compClass = typeof(CompCursedSpeechEffect);
        }
    }

    public class CompCursedSpeechEffect : BaseCursedEnergyAbility
    {
        public new CompProperties_CursedSpeechEffect Props => (CompProperties_CursedSpeechEffect)props;

        public override void ApplyAbility(LocalTargetInfo target, LocalTargetInfo dest)
        {
            if (target.Pawn != null)
            {
      
                ApplyCursedSpeechEffect(parent.pawn, target.Pawn);
            }
        }

        public virtual float CalcSeverityEffectiveness(Pawn casterPawn, Pawn targetPawn)
        {
            Gene_CursedEnergy targetCursedEnergy = targetPawn.GetCursedEnergy();
            if (targetCursedEnergy != null)
            {
                float targetCursedEnergyReserves = targetPawn.GetStatValue(JJKDefOf.JJK_CursedEnergy);
                float casterCursedEnergyReserves = casterPawn.GetStatValue(JJKDefOf.JJK_CursedEnergy);
                float casterCursedEnergyDamageBonus = casterPawn.GetStatValue(JJKDefOf.JJK_CursedEnergyDamageBonus);

                float effectiveness = (casterCursedEnergyReserves * casterCursedEnergyDamageBonus) /
                                      (targetCursedEnergyReserves + 1f);

                return Mathf.Lerp(0, 1, Mathf.Clamp01(effectiveness * Props.baseEffectStrength));
            }
            return 1f;
        }

        public virtual int CalcDurationEffectiveness(Pawn casterPawn, Pawn targetPawn)
        {
            Gene_CursedEnergy targetCursedEnergy = targetPawn.GetCursedEnergy();
            if (targetCursedEnergy != null)
            {
                float targetCursedEnergyReserves = targetPawn.GetStatValue(JJKDefOf.JJK_CursedEnergy);
                float casterCursedEnergyReserves = casterPawn.GetStatValue(JJKDefOf.JJK_CursedEnergy);
                float casterCursedEnergyDamageBonus = casterPawn.GetStatValue(JJKDefOf.JJK_CursedEnergyDamageBonus);

                float effectiveness = (casterCursedEnergyReserves * casterCursedEnergyDamageBonus) /
                                      (targetCursedEnergyReserves + 1f);

                return Mathf.RoundToInt(Mathf.Lerp(Props.minDurationTicks, Props.maxDurationTicks, Mathf.Clamp01(effectiveness)));
            }
            return Props.maxDurationTicks;
        }

        public virtual void ApplyCursedSpeechEffect(Pawn casterPawn, Pawn targetPawn)
        {
            if (Props.hediffToApply != null)
            {
                Hediff hediff = HediffMaker.MakeHediff(Props.hediffToApply, targetPawn);

                if (hediff.TryGetComp<HediffComp_Disappears>(out HediffComp_Disappears disappears))
                {
                    int duration = CalcDurationEffectiveness(casterPawn, targetPawn);
                    disappears.ticksToDisappear = duration;
                    Log.Message($"Setting TickDuration is {duration}");
                }

                if (Props.scaleSeverity)
                {
                    float Severity = CalcSeverityEffectiveness(casterPawn, targetPawn);
                    hediff.Severity = Severity;
                    Log.Message($"Setting Severity is {Severity}");
                }

                targetPawn.health.AddHediff(hediff);
            }
        }

        protected int CalculateEffectDuration(float effectiveness)
        {
            return Mathf.RoundToInt(Mathf.Lerp(Props.minDurationTicks, Props.maxDurationTicks, effectiveness));
        }
    }
}