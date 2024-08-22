using RimWorld;
using Verse;

namespace JJK
{
    public class CompProperties_ToggleHediff : CompProperties_ToggleableEffect
    {
        public HediffDef hediffDef;
        public float cursedEnergyCostPerTick = 1f;
        public int ticksBetweenCost = 2500;

        
        public CompProperties_ToggleHediff()
        {
            compClass = typeof(CompAbilityEffect_ToggleHediff);
        }
    }

    public class CompAbilityEffect_ToggleHediff : ToggleableCompAbilityEffect
    {
        public new CompProperties_ToggleHediff Props => (CompProperties_ToggleHediff)props;

        public override void ApplyAbility(LocalTargetInfo target, LocalTargetInfo dest)
        {
            if (target.Pawn != null)
            {
                Toggle();
            }
        }

        public override void CompTick()
        {
            base.CompTick();

            if (parent.pawn != null)
            {
                if (parent.pawn.IsHashIntervalTick(Props.ticksBetweenCost))
                {                
                    if (!HasCursedEnergy(parent.pawn, Props.cursedEnergyCostPerTick))
                    {
                        RemoveHediff(parent.pawn);
                    }
                    else
                    {
                        ConsumeCursedEnergy(Props.cursedEnergyCostPerTick);
                    }
                }
            }

        }

        public override void Activate()
        {
            base.Activate();
            AddHediff(parent.pawn);
        }

        public override void DeActivate()
        {
            base.DeActivate();
            RemoveHediff(parent.pawn);
        }

        private void AddHediff(Pawn pawn)
        {
            Hediff hediff = HediffMaker.MakeHediff(Props.hediffDef, pawn);
            pawn.health.AddHediff(hediff);
            Messages.Message($"Added {Props.hediffDef.label} to {pawn.LabelShort}", MessageTypeDefOf.NeutralEvent);
        }

        private void RemoveHediff(Pawn pawn)
        {
            Hediff existingHediff = pawn.health.hediffSet.GetFirstHediffOfDef(Props.hediffDef);
            if (existingHediff != null)
            {
                pawn.health.RemoveHediff(existingHediff);
                Messages.Message($"Removed {Props.hediffDef.label} from {pawn.LabelShort}", MessageTypeDefOf.NeutralEvent);
            }
        }


        public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
        {
            return target.Pawn != null && base.Valid(target, throwMessages);
        }
    }
}