using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace JJK
{
    public class CompProperties_PawnPuller : CompProperties
    {
        public float PullRadius = 10f;
        public int PullTicks = 600;
        public DamageDef Damage = DamageDefOf.Crush;
        public float DamageAmount = 15f;
        public float ArmourPen = 1f;

        public CompProperties_PawnPuller()
        {
            compClass = typeof(CompEffect_PawnPuller);
        }
    }
    public class CompEffect_PawnPuller : ThingComp
    {
        private CompProperties_PawnPuller Props => (CompProperties_PawnPuller)props;

        // Timer for periodic pulling
        private int ticksUntilPull = 0;

        public override void CompTick()
        {
            base.CompTick();
            if (ticksUntilPull <= 0)
            {
                PullPawnsTowardsCenter();
                ticksUntilPull = Props.PullTicks;
            }
            else
            {
                ticksUntilPull--;
            }
        }

        private void PullPawnsTowardsCenter()
        {
            List<Pawn> pawnsToPull = JJKUtility.GetEnemyPawnsInRange(parent.Position, parent.MapHeld, Props.PullRadius).ToList();
            foreach (Pawn pawn in pawnsToPull)
            {
                bool isBlueUser = pawn.IsLimitlessUser() || pawn.Faction == Faction.OfPlayer;
                if (isBlueUser)
                {
                    continue;
                }

                if (Props.Damage != null && !pawn.Dead)
                {
                    pawn.TakeDamage(new DamageInfo(Props.Damage, Props.DamageAmount, Props.ArmourPen, -1, parent));
                }
                
                // Calculate direction towards the center (position of the thing)
                IntVec3 direction = parent.Position - pawn.Position;

                // Apply pulling effect (move pawn towards the center)
                IntVec3 destination = pawn.Position + direction;

                // Spawn the flyer
                PawnFlyer pawnFlyer = PawnFlyer.MakeFlyer(JJKDefOf.JJK_Flyer, pawn, destination, null, null);
                GenSpawn.Spawn(pawnFlyer, destination, parent.MapHeld);
            }

        }
    }



}