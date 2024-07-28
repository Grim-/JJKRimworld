using JJK;
using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Noise;
using static HarmonyLib.Code;

namespace JJK
{
    public class CompProperties_RCT : CompProperties_RCTBase
    {
        public CompProperties_RCT()
        {
            compClass = typeof(CompAbilityEffect_RCT);
        }
    }
    public class CompAbilityEffect_RCT : CompAbilityEffect_RCTBase
    {
        new CompProperties_RCT Props => (CompProperties_RCT)props;

        public override void ApplyAbility(LocalTargetInfo target, LocalTargetInfo dest)
        {
            ToggleRCT();
        }

        private void ToggleRCT()
        {
            IsCurrentlyCasting = !IsCurrentlyCasting;

            Ability_Toggleable toggleable = (Ability_Toggleable)parent;
            toggleable.Toggle();

            accumulatedTicks = 0;
            if (IsCurrentlyCasting)
            {  
                if (!JJKUtility.HasRCTActive(parent.pawn))
                {
                    JJKUtility.ApplyRCTHediffTo(parent.pawn);
                }
            }
            else
            {
                StopCasting();
                JJKUtility.RemoveRCTHediff(parent.pawn);
            }
        }

        private float accumulatedTicks = 0f;

        public override void CompTick()
        {
            base.CompTick();

            Gene_CursedEnergy cursedEnergyGene = parent.pawn.genes?.GetFirstGeneOfType<Gene_CursedEnergy>();
            if (!IsCurrentlyCasting || cursedEnergyGene == null)
                return;

            Hediff currentRCTHediff = parent.pawn.health.hediffSet.GetFirstHediffOfDef(JJKDefOf.RCTRegenHediff);

            if (currentRCTHediff == null)
            {
                IsCurrentlyCasting = false;
                accumulatedTicks = 0f;
                return;
            }

            float rctSpeedBonus = parent.pawn.GetStatValue(JJKDefOf.JJK_RCTSpeedBonus);
            accumulatedTicks += 1f * rctSpeedBonus;

            while (accumulatedTicks >= Props.PartRegenTickCount)
            {
                OnTick(cursedEnergyGene);
                EffecterDefOf.ShamblerRaise.SpawnAttached(parent.pawn, parent.pawn.Map);
                accumulatedTicks -= Props.PartRegenTickCount;
            }

            // Clamp accumulatedTicks to ensure it doesn't go below 0
            accumulatedTicks = Mathf.Max(accumulatedTicks, 0f);
        }

        protected override void StopCasting()
        {
            base.StopCasting();
            accumulatedTicks = 0;
        }

        private void OnTick(Gene_CursedEnergy CursedEnergy)
        {
            if (CursedEnergy.Value >= GetTickCost())
            {
                if (CursedEnergy.Value >= GetPartCost())
                {
                    HealTargetPawn(CursedEnergy, parent.pawn);
                }
            }
            else
            {
                JJKUtility.RemoveRCTHediff(parent.pawn);
            }

            parent.pawn.GetCursedEnergy()?.ConsumeCursedEnergy(GetTickCost());
        }

        public override float GetCost()
        {
            return Props.cursedEnergyCost;
        }


        public override string CompInspectStringExtra()
        {
            string String = base.CompInspectStringExtra();

            if (IsCurrentlyCasting) String = String + "RCT Active";
            return String;
        }

    }
}