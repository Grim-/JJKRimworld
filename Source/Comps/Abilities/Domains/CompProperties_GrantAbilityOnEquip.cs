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

        public override void Notify_Equipped(Pawn pawn)
        {
            base.Notify_Equipped(pawn);

            if (pawn.Faction == Faction.OfPlayer && !pawn.HasAbility(Props.AbilityToGrant))
            {
                pawn.abilities.GainAbility(Props.AbilityToGrant);
                DidGrant = true;
            }


        }

        public override void Notify_Unequipped(Pawn pawn)
        {
            base.Notify_Unequipped(pawn);

            if (DidGrant)
            {
                pawn.abilities.RemoveAbility(Props.AbilityToGrant);
            }
        }
    }
}