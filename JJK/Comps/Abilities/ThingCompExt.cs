using RimWorld;
using Verse;

namespace JJK
{
    public abstract class ThingCompExt : ThingComp
    {
        public virtual DamageWorker.DamageResult Notify_ApplyMeleeDamageToTarget(LocalTargetInfo target, DamageWorker.DamageResult DamageWorkerResult)
        {
            return DamageWorkerResult;
        }


        public virtual void Notify_EquipOwnerUsedVerb(Pawn pawn, Verb verb)
        {
  
        }

    }


    public class CompProperties_GrantAbilityOnEquip : CompProperties
    {
        public AbilityDef AbilityToGrant;

        public CompProperties_GrantAbilityOnEquip()
        {
            compClass = typeof(CompGrantAbilityOnEquip);
        }
    }

    public class CompGrantAbilityOnEquip : ThingCompExt
    {
        new CompProperties_GrantAbilityOnEquip Props => (CompProperties_GrantAbilityOnEquip)props;

        private bool DidGrant = false;

        public override void Notify_Equipped(Pawn pawn)
        {
            base.Notify_Equipped(pawn);

            if (!pawn.HasAbility(Props.AbilityToGrant))
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