using RimWorld;

namespace JJK
{
    public class Verb_CastDomainExpansion : Verb_CastAbility
    {
        protected override bool TryCastShot()
        {
            Ability_ExpandDomain ability = this.ability as Ability_ExpandDomain;
            if (ability == null) return false;

            if (ability.IsDomainActive)
            {
                ability.DestroyActiveDomain();
                return true;
            }
            else
            {
                return ability.ActivateDomain(currentTarget.Cell);
            }
        }
    }
}