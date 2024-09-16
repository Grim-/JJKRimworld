using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace JJK
{
    public class CompProperties_BlueAOE : CompProperties
    {
        public float PullRadius = 10f;
        public int PullTicks = 80;
        public int DamageTicks = 180;
        public DamageDef Damage = DamageDefOf.Crush;
        public float DamageAmount = 15f;
        public float ArmourPen = 1f;
        public EffecterDef AOEEffecter;
        public CompProperties_BlueAOE()
        {
            compClass = typeof(CompEffect_BlueAOE);
        }
    }
    public class CompEffect_BlueAOE : ThingComp
    {
        private CompProperties_BlueAOE Props => (CompProperties_BlueAOE)props;

        private Ticker PullTickTimer = null;
        private Ticker DamageTickTimer = null;

        private Effecter Effect;

        public override void PostPostMake()
        {
            base.PostPostMake();

            if (PullTickTimer == null)
            {
                PullTickTimer = new Ticker(Props.PullTicks, PullPawnsTowardsCenter);
            }


            if (DamageTickTimer == null)
            {
                DamageTickTimer = new Ticker(Props.DamageTicks, DamagePawnsInRadius);
            }


        }

        public override void PostDeSpawn(Map map)
        {
            if (Effect != null)
            {
                Effect.Cleanup();
                Effect = null;
            }
            base.PostDeSpawn(map);
        }


        public override void CompTick()
        {
            base.CompTick();

            if (Effect == null && Props.AOEEffecter != null)
            {
                Effect = Props.AOEEffecter.SpawnMaintained(this.parent.Position, this.parent.MapHeld);
            }

            if (Effect != null)
            {
                Effect.EffectTick(this.parent, this.parent);
            }

            if (PullTickTimer != null)
            {
                PullTickTimer.Tick();
            }

            if (DamageTickTimer != null)
            {
                DamageTickTimer.Tick();
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

                IntVec3 direction = parent.Position - pawn.Position;
                IntVec3 destination = pawn.Position + direction;
                PawnFlyer pawnFlyer = PawnFlyer.MakeFlyer(JJKDefOf.JJK_Flyer, pawn, destination, null, null);
                GenSpawn.Spawn(pawnFlyer, destination, parent.MapHeld);
            }
        }


        private void DamagePawnsInRadius()
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
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Deep.Look(ref PullTickTimer, "pullTickTimer");
            Scribe_Deep.Look(ref DamageTickTimer, "damageTickTimer");
        }
    }
}