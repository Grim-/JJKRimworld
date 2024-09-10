using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace JJK
{    
    public class ComplexBeamEffect
    {
        protected LocalTargetInfo Source;
        protected LocalTargetInfo Target;

        private Vector3 Origin => GetPositionFromTarget(Source) + BeamOffset;
        private Vector3 TargetPosition => GetPositionFromTarget(Target);

        private int DurationTicks;
        private int CurrentLifeTicks;
        private Map Map;
        private Effecter StartEffecter;
        private Mote BeamEffecter;
        private Effecter EndEffecter;

        private Mote MoteBeam;
        private static readonly Vector3 BeamOffset = new Vector3(0, 0.2f, 0);

        public void Initialize(Map map, LocalTargetInfo source, LocalTargetInfo target, int duration, EffecterDef startEffecterDef, ThingDef beamEffecterDef, EffecterDef endEffecterDef)
        {
            if (map == null)
            {
                Log.Error("ComplexBeamEffect: Map is null during initialization.");
                return;
            }

            Source = source;
            Target = target;
            Map = map;
            DurationTicks = duration;
            CurrentLifeTicks = 0;

            Log.Message($"ComplexBeamEffect: Initializing beam from {Origin} to {Target}");

            StartEffecter = CreateEffecter(startEffecterDef, Origin);
            GenerateMiddle(beamEffecterDef);
            EndEffecter = CreateEffecter(endEffecterDef, TargetPosition);

            UpdateBeamEffecter();
        }

        private Vector3 GetPositionFromTarget(LocalTargetInfo targetInfo)
        {
            if (targetInfo.HasThing)
            {
                return targetInfo.Thing.DrawPos;
            }
            else
            {
                return targetInfo.Cell.ToVector3Shifted();
            }
        }

        private void GenerateMiddle(ThingDef beamEffecterDef)
        {
            MoteBeam = MoteMaker.MakeConnectingLine(Origin, TargetPosition, beamEffecterDef, Map);
           // MoteBeam = (MoteBeam)GenSpawn.Spawn(beamEffecterDef, Origin.ToIntVec3(), Map);
        }

        public void Tick()
        {
            CurrentLifeTicks++;

            TickEffecters();
            UpdateBeamEffecter();
        }

        private void TickEffecters()
        {
            StartEffecter?.EffectTick(null, null);
            MoteBeam?.Maintain();
            EndEffecter?.EffectTick(null, null);
        }

        private Effecter CreateEffecter(EffecterDef effecterDef, Vector3 position)
        {
            if (effecterDef == null) return null;
            return effecterDef.SpawnMaintained(position.ToIntVec3(), Map);
        }

        private void UpdateBeamEffecter()
        {
            if (BeamEffecter == null || !Target.IsValid) return;
            UpdateTarget(GetPositionFromTarget(Target));
        }

        public virtual bool ShouldEnd()
        {
            return CurrentLifeTicks >= DurationTicks;
        }

        public void UpdateTarget(Vector3 targetPositionWorld)
        {
            if (MoteBeam == null || Map == null) return;
            MoteBeam.Tick();
            MoteBeam.Maintain();
            //MoteBeam.UpdateTargets(GetPositionFromTarget(Source), GetPositionFromTarget(Target));
        }

        public void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            StartEffecter?.Cleanup();
            BeamEffecter?.Destroy();
            EndEffecter?.Cleanup();
        }
    }
}
    

