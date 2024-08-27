using RimWorld;
using Verse;

namespace JJK
{
    public class CompProperties_GrantAbilityOnEquip : CompProperties
    {
        public AbilityDef AbilityToGrant;

        public CompProperties_GrantAbilityOnEquip()
        {
            compClass = typeof(CompGrantAbilityOnEquip);
        }
    }


    public class CompGrantAbilityOnEquip : ThingComp
    {
        new CompProperties_GrantAbilityOnEquip Props => (CompProperties_GrantAbilityOnEquip)props;

        private bool DidGrant = false;
        private int? StoredCooldown = null;

        public override void Notify_Equipped(Pawn pawn)
        {
            base.Notify_Equipped(pawn);
            if (pawn.Faction == Faction.OfPlayer && !pawn.HasAbility(Props.AbilityToGrant))
            {
                pawn.abilities.GainAbility(Props.AbilityToGrant);

                Ability grantedAbility = pawn.abilities.GetAbility(Props.AbilityToGrant);
                DidGrant = true;

                if (StoredCooldown.HasValue)
                {
                    grantedAbility.StartCooldown(StoredCooldown.Value);
                }
            }
        }

        public override void Notify_Unequipped(Pawn pawn)
        {
            base.Notify_Unequipped(pawn);
            if (DidGrant)
            {
                Ability abilityToRemove = pawn.abilities.GetAbility(Props.AbilityToGrant);
                if (abilityToRemove != null)
                {
                    StoredCooldown = abilityToRemove.CooldownTicksRemaining;
                    pawn.abilities.RemoveAbility(Props.AbilityToGrant);
                }
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref DidGrant, "DidGrant");
            Scribe_Values.Look(ref StoredCooldown, "StoredCooldown");
        }
    }
}