using RimWorld;
using System;
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


        public override void CompTick()
        {
            base.CompTick();
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


    public class Ticker: IExposable
    {
        public int Ticks = 100;

        protected Action _OnTick;
        protected int CurrentTick = 0;
        public bool IsRunning { private set; get; }

        public Ticker(int ticks, Action onTick, bool startAutomatically = true)
        {
            Ticks = ticks;
            _OnTick = onTick;
            CurrentTick = 0;
           if(startAutomatically) Start();
        }

        public void Tick()
        {
            CurrentTick++;

            if (CurrentTick >= Ticks)
            {
                _OnTick?.Invoke();
                Reset();
            }
        }

        public void Reset()
        {
            CurrentTick = 0;
        }

        public void Start()
        {
            IsRunning = true;
        }

        public void Stop(bool reset = false)
        {
            IsRunning = false;

            if (reset) Reset();
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref CurrentTick, "timerCurrentTick", 0);
        }
    }
}