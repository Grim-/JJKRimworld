﻿using RimWorld;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using static RimWorld.RitualStage_InteractWithRole;

namespace JJK
{
    public class CompProperties_Cleave : CompProperties_CursedAbilityProps
    {
        public int NumberOfCuts = 8;
        public int KnockbackDistance = 5;
        public float BaseDamage = 8f;
        public int TicksBetweenCuts = 10;
        public DamageDef DamageDef;

        public EffecterDef CleaveDamageEffecter;

        public CompProperties_Cleave()
        {
            compClass = typeof(CompAbilityEffect_Cleave);
        }
    }
    public class CompAbilityEffect_Cleave : BaseCursedEnergyAbility
    {
        public new CompProperties_Cleave Props => (CompProperties_Cleave)props;
        private Pawn TargetPawn;
        private float DamagePerCut;
        private Ticker DamageTicker;

        public override void ApplyAbility(LocalTargetInfo target, LocalTargetInfo dest)
        {
            Pawn pawn = target.Pawn;
            if (pawn == null)
            {
                return;
            }

            TargetPawn = pawn;
            DamagePerCut = Props.BaseDamage;

            // Initialize the Ticker
            DamageTicker = new Ticker(Props.TicksBetweenCuts, ApplyCut, null, true, Props.NumberOfCuts);

            // Launch the pawn
            IntVec3 launchDirection = pawn.Position - parent.pawn.Position;
            IntVec3 destination = pawn.Position + launchDirection * Props.KnockbackDistance;
            PawnFlyer pawnFlyer = PawnFlyer.MakeFlyer(JJKDefOf.JJK_Flyer, pawn, destination, null, null);
            GenSpawn.Spawn(pawnFlyer, destination, parent.pawn.Map);
        }

        public override void CompTick()
        {
            base.CompTick();

            if (DamageTicker != null && DamageTicker.IsRunning)
            {
                DamageTicker.Tick();
            }
        }

        private void ApplyCut()
        {
            if (TargetPawn == null || TargetPawn.Dead || TargetPawn.Destroyed)
            {
                DamageTicker.Stop();
                return;
            }

            if (this.Props.CleaveDamageEffecter != null)
            {
                this.Props.CleaveDamageEffecter.SpawnMaintained(TargetPawn, TargetPawn.MapHeld);
            }

            float actualDamage = DamagePerCut * TargetPawn.GetStatValue(JJKDefOf.JJK_CursedEnergyDamageBonus);
            DamageInfo damageInfo = new DamageInfo(Props.DamageDef, actualDamage);
            TargetPawn.TakeDamage(damageInfo);
        }
        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_References.Look(ref TargetPawn, "targetPawn");
            Scribe_Values.Look(ref DamagePerCut, "damagePerCut");
            Scribe_Deep.Look(ref DamageTicker, "damageTicker");
        }
    }

}

    