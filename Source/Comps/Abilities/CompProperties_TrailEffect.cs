using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace JJK
{
    public class CompProperties_TrailEffect : CompProperties
    {
        public ThingDef TrailMote;
        public float MoteInterval = 0.1f;
        public float MoteSize = 1f;
        public float MoteRotationRate = 0f;
        public float MoteDuration = 60f;
        public bool AlignWithMovement = true;

        public CompProperties_TrailEffect()
        {
            compClass = typeof(Comp_TrailEffect);
        }
    }

    public class Comp_TrailEffect : ThingComp
    {
        private CompProperties_TrailEffect Props => (CompProperties_TrailEffect)props;
        private int ticksSinceLastMote = 0;

        public override void CompTick()
        {
            base.CompTick();

            if (parent.Map == null || Props.TrailMote == null) return;

            ticksSinceLastMote++;
            if (ticksSinceLastMote >= Props.MoteInterval * 60)
            {
                SpawnStaticMote();
                ticksSinceLastMote = 0;
            }
        }
        private void SpawnStaticMote()
        {
            Mote mote = (Mote)ThingMaker.MakeThing(Props.TrailMote, null);
            mote.Scale = Props.MoteSize;
            mote.exactPosition = parent.DrawPos;

            if (Props.AlignWithMovement && parent is Projectile projectile)
            {
                IntVec3 targetCell = projectile.intendedTarget.Cell;
                IntVec3 projectileCell = projectile.Position;
                Vector3 direction = (targetCell.ToVector3() - projectileCell.ToVector3()).normalized;
                mote.exactRotation = direction.ToAngleFlat();
                GenSpawn.Spawn(mote, parent.Position, parent.Map);
            }
            else
            {
                GenSpawn.Spawn(mote, parent.Position, parent.Map);
            }
        }
    }
}