using RimWorld;

namespace JJK
{
    public class DomainExpansionDef : AbilityDef
    {
        public string DomainThingDefName = "JJK_ForestDomain";

        public DomainExpansionDef()
        {
            abilityClass = typeof(Ability_ExpandDomain);
        }
    }
}