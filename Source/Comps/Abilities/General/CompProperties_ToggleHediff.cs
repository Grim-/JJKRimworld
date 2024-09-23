using RimWorld;
using System.Collections.Generic;
using Verse;

namespace JJK
{
    public class CompProperties_ToggleHediff : CompProperties_ToggleableEffect
    {
        public List<HediffDef> hediffDefs;
        public float cursedEnergyCostPerTick = 1f;
        
        public CompProperties_ToggleHediff()
        {
            compClass = typeof(CompAbilityEffect_ToggleHediff);
        }
    }

    public class CompAbilityEffect_ToggleHediff : ToggleableCompAbilityEffect
    {
        public new CompProperties_ToggleHediff Props => (CompProperties_ToggleHediff)props;


        private Gene_CursedEnergy _CursedEnergy;
        private Gene_CursedEnergy CursedEnergy
        {
            get
            {
                if (_CursedEnergy == null)
                {
                    _CursedEnergy = parent.pawn.GetCursedEnergy();
                }

                return _CursedEnergy;
            }
        }

        public override void ApplyAbility(LocalTargetInfo target, LocalTargetInfo dest)
        {
            //if (target.Pawn != null)
            //{
            //    Toggle();
            //}
        }

        public override void CompTick()
        {
            base.CompTick();

            if (parent.pawn != null)
            {
                if (IsActive && parent.pawn.IsHashIntervalTick(Props.Ticks))
                {                
                    if (!CursedEnergy.HasCursedEnergy(Props.cursedEnergyCostPerTick))
                    {
                        RemoveHediff(parent.pawn);
                        DeactiveOnParentAbility();
                    }
                    else
                    {
                        CursedEnergy.ConsumeCursedEnergy(Props.cursedEnergyCostPerTick);
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
            foreach (var item in Props.hediffDefs)
            {
                Hediff hediff = HediffMaker.MakeHediff(item, pawn);
                pawn.health.AddHediff(hediff);
                //Messages.Message($"Added {item.label} to {pawn.LabelShort}", MessageTypeDefOf.NeutralEvent);
            }
        }

        public override bool ShouldDisableBecauseNoCE(float Cost)
        {
            if (IsActive)
            {
                return false;
            }
            else return base.ShouldDisableBecauseNoCE(Cost);
        }

        private void RemoveHediff(Pawn pawn)
        {
            foreach (var item in Props.hediffDefs)
            {
                Hediff existingHediff = pawn.health.hediffSet.GetFirstHediffOfDef(item);
                if (existingHediff != null)
                {
                    pawn.health.RemoveHediff(existingHediff);
                    //Messages.Message($"Removed {item.label} from {pawn.LabelShort}", MessageTypeDefOf.NeutralEvent);
                }
            }

        }


        public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
        {
            return target.Pawn != null && base.Valid(target, throwMessages);
        }
    }
}