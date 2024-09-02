﻿using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace JJK
{
    public class CompProperties_TwistAreaEffect : CompProperties_CursedAbilityProps
    {
        public float baseDamage;
        public int maxTargets = 10;
        public float radius = 10f;

        public CompProperties_TwistAreaEffect()
        {
            compClass = typeof(CursedSpeechAreaTwistEffect);
        }
    }


    public class CursedSpeechAreaTwistEffect : BaseCursedEnergyAbility
    {
        public new CompProperties_TwistAreaEffect Props => (CompProperties_TwistAreaEffect)props;

        public override void ApplyAbility(LocalTargetInfo target, LocalTargetInfo dest)
        {
            if (target.IsValid && target.Cell.IsValid)
            {
                MoteMaker.ThrowText(parent.pawn.DrawPos, parent.pawn.Map, $"APPROACH!", Color.green);
                TwistPawnsInArea(target.Cell);
            }
        }
        private void TwistPawnsInArea(IntVec3 center)
        {
            Map map = parent.pawn.Map;
            IEnumerable<Pawn> pawnsToLure = GetEnemyPawnsInRange(center, map, Props.radius)
                .Take(Props.maxTargets);

            foreach (Pawn enemyPawn in pawnsToLure)
            {
                float CEScale = JJKUtility.CalcCursedEnergyScalingFactor(parent.pawn, enemyPawn);
                CursedSpeechAreaTwistEffect.TwistTargetLimb(enemyPawn, parent.pawn, Props.baseDamage, CEScale);
            }
        }

        private IEnumerable<Pawn> GetEnemyPawnsInRange(IntVec3 center, Map map, float radius)
        {
            return GenRadial.RadialCellsAround(center, radius, true)
                .SelectMany(c => c.GetThingList(map))
                .OfType<Pawn>()
                .Where(p => p.Faction != null && p.Faction != Faction.OfPlayer);
        }

        public static void TwistTargetLimb(Pawn Target, Pawn Caster, float BaseDamage, float Scale)
        {
            BodyPartRecord targetLimb = JJKUtility.GetRandomLimb(Target);

            if (targetLimb != null)
            {
                // Calculate damage
                float damage = BaseDamage * Scale;

                // Apply damage to the selected limb
                DamageInfo dinfo = new DamageInfo(JJKDefOf.JJK_TwistDamage, damage, 1f, -1f, Caster, targetLimb);
                Target.TakeDamage(dinfo);

                // Optional: Add a visual or sound effect
                MoteMaker.ThrowText(Caster.DrawPos, Caster.Map, "TWIST!", Color.red);
            }
        }
    }
}