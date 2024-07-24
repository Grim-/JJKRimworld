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


        public string DisplayLabel => Label + " (" + "Gene".Translate() + ")";

        public float ResourceLossPerDay => def.resourceLossPerDay;

        public override float InitialResourceMax => Pawn.GetStatValue(JJKDefOf.JJK_CursedEnergy);

        public override float MinLevelForAlert => 0.15f;

        public override float MaxLevelOffset => 0.1f;

       // public override int ValueForDisplay => base.ValueForDisplay;
       // public override int MaxForDisplay => (int)Pawn.GetStatValue(JJKDefOf.JJK_CursedEnergy);

        public override float Max => Pawn.GetStatValue(JJKDefOf.JJK_CursedEnergy);

        protected override Color BarColor => new ColorInt(3, 3, 138).ToColor;

        protected override Color BarHighlightColor => new ColorInt(42, 42, 145).ToColor;




        public float RegenMod => Pawn.GetStatValue(JJKDefOf.JJK_CursedEnergyRegen);
        public float CostMult => Pawn.GetStatValue(JJKDefOf.JJK_CursedEnergyCost);

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
                    RestoreCursedEnergy(pawn, JJKConstants.HumanConsumeRestoreBaseAmount * thing.GetStatValue(StatDefOf.Nutrition) * (float)numTaken);
                }
            }
        }

        public void ConsumeCursedEnergy(Pawn Pawn, float Amount)
        {
            if (!ModsConfig.BiotechActive)
            {
                return;
            }

            Gene_CursedEnergy cursedEnerrgy = Pawn.GetCursedEnergy();
            if (cursedEnerrgy != null)
            {
                cursedEnerrgy.Value -= Amount;
            }
            else
            {
                //log error
            }
        }

        public void RestoreCursedEnergy(Pawn Pawn, float Amount)
        {
            if (!ModsConfig.BiotechActive)
            {
                return;
            }

            Gene_CursedEnergy cursedEnerrgy = Pawn.GetCursedEnergy();
            if (cursedEnerrgy != null)
            {
                cursedEnerrgy.Value += Amount;
            }
            else
            {
                //log error
            }
        }


        public override void Tick()
        {
            base.Tick();

            CurrentTick++;

            if (CurrentTick % JJKConstants.CursedEnergyRegenTicks == 0)
            {
                RestoreCursedEnergy(pawn, pawn.GetStatValue(JJKDefOf.JJK_CursedEnergyRegen));
                CurrentTick = 0;
            }
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
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref CursedEnergy, "hemogenPacksAllowed", defaultValue: true);
        }
    }
}
