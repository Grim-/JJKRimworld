using RimWorld;
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
        public int numCuts = 8;
        public int knockback = 5;
        public float cutDamage = 8f;
        public int ticksBetweenCuts = 10;

        public CompProperties_Cleave()
        {
            compClass = typeof(CompAbilityEffect_Cleave);
        }
    }
    public class CompAbilityEffect_Cleave : BaseCursedEnergyAbility
    {
        public new CompProperties_Cleave Props => (CompProperties_Cleave)props;

        public override void ApplyAbility(LocalTargetInfo target, LocalTargetInfo dest)
        {
            Pawn pawn = target.Pawn;
            if (pawn == null)
            {
                return;
            }

            int totalCuts = Props.numCuts;
            float damagePerCut = Props.cutDamage;
            int ticksBetweenCuts = Props.ticksBetweenCuts;

            // Calculate the total duration for applying damage
            int totalTicks = totalCuts * ticksBetweenCuts;

            // Calculate the damage per tick
            float damagePerTick = damagePerCut / ticksBetweenCuts;

            // Start applying damage over time
            ApplyDamageOverTime(pawn, totalTicks, damagePerTick);

            // Launch the pawn
            IntVec3 launchDirection = pawn.Position - parent.pawn.Position;
            IntVec3 destination = pawn.Position + launchDirection * Props.knockback;
            PawnFlyer pawnFlyer = PawnFlyer.MakeFlyer(JJKDefOf.JJK_Flyer, pawn, destination, null, null);
            GenSpawn.Spawn(pawnFlyer, destination, parent.pawn.Map);
        }


        private async void ApplyDamageOverTime(Pawn pawn, int totalTicks, float damagePerTick)
        {
            for (int i = 0; i < totalTicks; i++)
            {
                pawn.TakeDamage(new DamageInfo(DamageDefOf.Cut, damagePerTick));
                await Task.Delay(1); // Adjust delay according to your desired time frame
            }
        }
    }

}

    