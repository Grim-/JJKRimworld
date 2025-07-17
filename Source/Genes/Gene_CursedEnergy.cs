using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse.Noise;
using Verse;
using UnityEngine;
using LudeonTK;

namespace JJK
{
    public class Gene_CursedEnergy : Gene_Resource, IGeneResourceDrain
    {
        public bool CursedEnergy = true;

        public Gene_Resource Resource => this;

        public Pawn Pawn => pawn;

        public bool CanOffset
        {
            get
            {
                if (Active)
                {
                    return !pawn.Deathresting;
                }
                return false;
            }
        }


        private int CurrentTick = 0;

        public override float Value
        {
            get => base.Value;
            set => base.Value = Mathf.Clamp(value, 0f, Max);
        }

        public float ValueCostMultiplied => Value * CostMult;

        public string DisplayLabel => Label + " (" + "Gene".Translate() + ")";
        public float ResourceLossPerDay => def.resourceLossPerDay;
        public override float InitialResourceMax => Pawn.GetStatValue(JJKDefOf.JJK_CursedEnergy);
        public override float MinLevelForAlert => 0.15f;
        public override float MaxLevelOffset => 0.1f;

        private float lastMax;
        public override float Max
        {
            get
            {
                float currentMax = Pawn.GetStatValue(JJKDefOf.JJK_CursedEnergy, true);
                if (currentMax != lastMax)
                {
                    lastMax = currentMax;
                    ForceBaseMaxUpdate(currentMax);
                }
                return currentMax;
            }
        }
        protected override Color BarColor => new ColorInt(3, 3, 138).ToColor;
        protected override Color BarHighlightColor => new ColorInt(42, 42, 145).ToColor;


        public override int ValueForDisplay => Mathf.RoundToInt(Value);
        public override int MaxForDisplay => Mathf.RoundToInt(Max);

        public float RegenMod => Pawn.GetStatValue(JJKDefOf.JJK_CursedEnergyRegen, true, 100);
        public int RegenTicks => Mathf.RoundToInt(Pawn.GetStatValue(JJKDefOf.JJK_CursedEnergyRegenSpeed, true, 100));
        public float CostMult => Pawn.GetStatValue(JJKDefOf.JJK_CursedEnergyCost, true, 100);




        public float GrowthMax = 15000f;


        public float TotalUsedCursedEnergy = 0;


        public override void PostAdd()
        {
            if (ModLister.CheckBiotech("Hemogen"))
            {
                base.PostAdd();
                Reset();
            }

        }

        public override void Notify_IngestedThing(Thing thing, int numTaken)
        {
            if (thing.def.IsMeat)
            {
                IngestibleProperties ingestible = thing.def.ingestible;
                if (ingestible != null && ingestible.sourceDef?.race?.Humanlike == true)
                {
                    RestoreCursedEnergy(JJKConstants.HumanConsumeRestoreBaseAmount * thing.GetStatValue(StatDefOf.Nutrition) * (float)numTaken);
                }
            }
        }
        private void ForceBaseMaxUpdate(float newMax)
        {
            // Force the base class to update its max value
            this.SetMax(newMax);
        }


        public void ConsumeCursedEnergy(float Amount)
        {
            if (!ModsConfig.BiotechActive)
            {
                return;
            }

            TotalUsedCursedEnergy += Amount;
            Value -= Amount * CostMult;
        }

        public void RestoreCursedEnergy(float Amount)
        {
            if (!ModsConfig.BiotechActive)
            {
                return;
            }

            Value += Amount;
        }

        public bool HasCursedEnergy(float Amount)
        {
            if (!ModsConfig.BiotechActive)
            {
                return false;
            }


            return Value >= Amount * CostMult;
        }


        public override void Tick()
        {
            base.Tick();

            CurrentTick++;
            if (CurrentTick >= RegenTicks)
            {
                //Log.Message($"Regenerating {RegenMod} after {RegenTicks} ticks.");
                RestoreCursedEnergy(RegenMod);
                ResetRegenTicks();
            }
        }


        public void ResetRegenTicks()
        {
            CurrentTick = 0;
        }

        public override void SetTargetValuePct(float val)
        {
            targetValue = Mathf.Clamp(val * Max, 0f, Max - MaxLevelOffset);
        }

        public bool ShouldConsumeHemogenNow()
        {
            return Value < targetValue;
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo gizmo in base.GetGizmos())
            {
                yield return gizmo;
            }
            foreach (Gizmo resourceDrainGizmo in GeneResourceDrainUtility.GetResourceDrainGizmos(this))
            {
                yield return resourceDrainGizmo;
            }

            if (Prefs.DevMode)
            {
                yield return new Command_Action()
                {
                    defaultLabel = "DEV: Restore all CE",
                    action = () =>
                    {
                        RestoreCursedEnergy(Max);
                    }
                };

            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref CursedEnergy, "cursedEnergy", defaultValue: true);
            Scribe_Values.Look(ref CurrentTick, "currentRegenTick", defaultValue: 0);
            Scribe_Values.Look(ref TotalUsedCursedEnergy, "TotalUsedCursedEnergy", defaultValue: 0);      
        }
    }
}
