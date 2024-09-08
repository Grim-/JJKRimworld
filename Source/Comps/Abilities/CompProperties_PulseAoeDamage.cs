using RimWorld;
using Verse;

namespace JJK
{
    public class CompProperties_PulseAoeDamage : CompProperties
    {
        public int TicksBetweenPulse = 80;
        public float ExplosionRadius = 3f;
        public DamageDef DamageType = DamageDefOf.Bomb;
        public int DamageAmount = 7;
        public float ArmorPenetration = 0.4f;
        public SoundDef ExplosionSound = null;

        public CompProperties_PulseAoeDamage()
        {
            compClass = typeof(PulseAOEDamageComp);
        }
    }

    public class PulseAOEDamageComp : ThingComp
    {
        private int TicksUntilPulse;

        public CompProperties_PulseAoeDamage Props => (CompProperties_PulseAoeDamage)props;

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            ResetTimer();
        }

        public override void CompTick()
        {
            base.CompTick();

            TicksUntilPulse--;

            if (TicksUntilPulse <= 0)
            {
                Explode();
                ResetTimer();
            }
        }

        private void Explode()
        {
            GenExplosion.DoExplosion(
                parent.Position,
                parent.Map,
                Props.ExplosionRadius,
                Props.DamageType,
                parent,
                Props.DamageAmount,
                Props.ArmorPenetration,
                Props.ExplosionSound
            );
        }

        private void ResetTimer()
        {
            TicksUntilPulse = Props.TicksBetweenPulse;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref TicksUntilPulse, "TicksUntilExplosion");
        }
    }
}