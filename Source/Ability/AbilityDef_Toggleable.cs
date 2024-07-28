using RimWorld;

namespace JJK
{
    public class AbilityDef_Toggleable : AbilityDef
    {
        public AbilityDef_Toggleable()
        {
            this.abilityClass = typeof(Ability_Toggleable);
        }
    }

    public class AbilityDef_MultiButtonSummon : AbilityDef
    {
        public AbilityDef_MultiButtonSummon()
        {
            this.abilityClass = typeof(ButtonSummonAbility);
        }
    }
}