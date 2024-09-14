using UnityEngine;
using Verse;

namespace JJK
{
    public class CompProperties_Orbiter : CompProperties
    {
        public CompProperties_Orbiter()
        {
            this.compClass = typeof(Comp_Orbiter);
        }
    }

    public class Comp_Orbiter : ThingComp
    {
        private IntVec3 spawnPoint;
        private float angle = 0f;
        private const float orbitRadius = 4f;
        private const float orbitSpeed = 4f;
        private const float lerpSpeed = 0.3f;

        private Vector3 currentPosition;
        private Vector3 targetPosition;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            spawnPoint = parent.Position;
            currentPosition = parent.DrawPos;
            targetPosition = currentPosition;
        }

        public override void CompTick()
        {
            base.CompTick();
            UpdateOrbitPosition();
            UpdateDrawPosition();
        }

        private void UpdateOrbitPosition()
        {
            angle += orbitSpeed;
            if (angle >= 360f)
            {
                angle -= 360f;
            }

            float radians = Mathf.Deg2Rad * angle;
            Vector3 orbitOffset = new Vector3(
                orbitRadius * Mathf.Cos(radians),
                0f,
                orbitRadius * Mathf.Sin(radians)
            );

            targetPosition = spawnPoint.ToVector3() + orbitOffset;
        }

        private void UpdateDrawPosition()
        {
            currentPosition = Vector3.Lerp(currentPosition, targetPosition, lerpSpeed);
            parent.SetPositionDirect(currentPosition.ToIntVec3());
        }

        public override void PostDraw()
        {
            base.PostDraw();
            // Optional: Draw a line to visualize the orbit path
            GenDraw.DrawLineBetween(spawnPoint.ToVector3(), parent.DrawPos, SimpleColor.Red);
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref spawnPoint, "spawnPoint");
            Scribe_Values.Look(ref angle, "angle");
            Scribe_Values.Look(ref currentPosition, "currentPosition");
            Scribe_Values.Look(ref targetPosition, "targetPosition");
        }
    }
}