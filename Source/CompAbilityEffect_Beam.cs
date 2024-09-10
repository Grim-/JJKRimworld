using RimWorld;
using Verse;

namespace JJK
{
    public class CompProperties_ComplexBeam : CompProperties_AbilityEffect
    {
        public EffecterDef BeamStart;
        public ThingDef BeamMiddle;
        public EffecterDef BeamEnd;

        public int BeamLifeInTicks = 12540;

        public CompProperties_ComplexBeam()
        {
            compClass = typeof(CompAbilityEffect_Beam);
        }
    }

    public class CompAbilityEffect_Beam : CompAbilityEffect
    {
        public new CompProperties_ComplexBeam Props => (CompProperties_ComplexBeam)props;

        private ComplexBeamEffect activeBeamEffect;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);

            Pawn caster = parent.pawn;

            ComplexBeamEffect beamEffect = new ComplexBeamEffect();
            beamEffect.Initialize(caster.Map,
                new LocalTargetInfo(caster),
                target,
                Props.BeamLifeInTicks,
                Props.BeamStart,
                Props.BeamMiddle,
                Props.BeamEnd
            );


            // Store the reference to the active beam effect
            activeBeamEffect = beamEffect;
        }

        public override void CompTick()
        {
            base.CompTick();

            // Check if the beam effect is still active and should be maintained
            if (activeBeamEffect != null)
            {

                //activeBeamEffect.UpdateTarget(newTargetPosition);
                activeBeamEffect.Tick();
                if (activeBeamEffect.ShouldEnd())
                {
                    activeBeamEffect.Destroy();
                    activeBeamEffect = null;
                }
            }
        }
    }
}
    

