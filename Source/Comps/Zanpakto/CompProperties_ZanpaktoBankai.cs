using RimWorld;
using System;
using System.Collections;
using UnityEngine;
using Verse;

namespace JJK
{
    public class CompProperties_ZanpaktoBankai : CompProperties_AbilityEffect
    {
        public CompProperties_ZanpaktoBankai()
        {
            compClass = typeof(CompAbilityEffect_ZanpaktoBankai);
        }
    }

    public class CompAbilityEffect_ZanpaktoBankai : CompAbilityEffect
    {

        private AnimatedTextDisplay AnimatedTextDisplay;
        private ZanpaktoWeapon Zanpakto => parent.pawn.equipment.Primary as ZanpaktoWeapon;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);

            if (Zanpakto == null)
            {
                Log.Error("CompAbilityEffect_ZanpaktoRelease: Primary weapon is not a ZanpaktoWeapon");
                return;
            }


            if (Zanpakto.CurrentState == ZanpaktoState.Bankai)
            {
                return;
            }

            Log.Message($"CompAbilityEffect_ZanpaktoRelease applying to {Zanpakto}");



            MoteMaker.ThrowText(Zanpakto.DrawPos, Zanpakto.MapHeld, "BAN-", 2f);
            AnimatedTextDisplay = new AnimatedTextDisplay(() =>
            {

                //WeatherEvent_LightningFlash lightningStrike = new WeatherEvent_LightningStrike(parent.pawn.MapHeld, parent.pawn.Position);
                //lightningStrike.FireEvent();
                MoteMaker.ThrowText(parent.pawn.DrawPos, parent.pawn.MapHeld, "KAI!", 2f);
                EffecterDefOf.ExtinguisherExplosion.SpawnAttached(parent.pawn, parent.pawn.MapHeld, 1f);
                Zanpakto.SetState(ZanpaktoState.Bankai);
            }, 80);
        }


        public override void CompTick()
        {
            base.CompTick();

            if (AnimatedTextDisplay != null)
            {
                AnimatedTextDisplay.Tick();
            }
        }
    }

    public class AnimatedTextDisplay
    {
        private int _Timer;
        private int _TicksToWait = 500;
        private Action _Action;
        private bool _IsRunning = false;
        private bool _Repeats = false;

        public AnimatedTextDisplay(Action ActionToDo, int TicksToWait, bool ShouldRepeat = false)
        {
            _Action = ActionToDo;
            _Timer = 0;
            _TicksToWait = TicksToWait;
            _IsRunning = true;
            _Repeats = ShouldRepeat;
        }

        public void Tick()
        {
            if (!_IsRunning) return;

            _Timer++;

            if (_Timer >= _TicksToWait)
            {
                _Action?.Invoke();
                if(!_Repeats) _IsRunning = false;
                _Timer = 0;
            }
        }
    }
}