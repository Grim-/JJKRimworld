using RimWorld;
using Verse;
using Verse.AI;

namespace JJK
{
    public class JobGiver_AICastOffensiveAbility : JobGiver_AICastAbility
    {
        public AbilityDef abilityDef;

        protected override LocalTargetInfo GetTarget(Pawn caster, Ability ability)
        {
            // Find the nearest hostile pawn within the ability's range
            float maxRange = ability.def.verbProperties.range;
            return (LocalTargetInfo)GenClosest.ClosestThingReachable(
                caster.Position,
                caster.Map,
                ThingRequest.ForGroup(ThingRequestGroup.Pawn),
                PathEndMode.Touch,
                TraverseParms.For(caster),
                maxRange,
                (Thing t) => t is Pawn p &&
                             p.HostileTo(caster) &&
                             !p.Downed &&
                             ability.Activate(new LocalTargetInfo(caster), new LocalTargetInfo(p))
            );
        }

        public override ThinkNode DeepCopy(bool resolve = true)
        {
            JobGiver_AICastOffensiveAbility copy = (JobGiver_AICastOffensiveAbility)base.DeepCopy(resolve);
            copy.ability = this.ability;
            return copy;
        }
    }
}

    