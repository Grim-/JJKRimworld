using RimWorld;
using System.Collections.Generic;
using Verse;

namespace JJK
{
    public class CompProperties_AbilityAbsorbingWeapon : CompProperties
    {
        public CompProperties_AbilityAbsorbingWeapon()
        {
            compClass = typeof(CompAbilityAbsorbingWeapon);
        }
    }

    public class CompAbilityAbsorbingWeapon : ThingCompExt
    {
        public List<AbilityDef> absorbedAbilities = new List<AbilityDef>();

        public List<AbilityDef> skillsTaughtToOwner = new List<AbilityDef>();

        CompProperties_AbilityAbsorbingWeapon Props => (CompProperties_AbilityAbsorbingWeapon)props;

        public Pawn EquipOwner { get; protected set; }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Collections.Look(ref absorbedAbilities, "absorbedAbilities", LookMode.Def);
        }

        public override void Notify_Equipped(Pawn pawn)
        {
            base.Notify_Equipped(pawn);

            EquipOwner = pawn;
            TeachOwnerSkills();
            Messages.Message("You feel the weapon granting you the abilities it has reaped.", MessageTypeDefOf.PositiveEvent);
        }

        public override void Notify_Unequipped(Pawn pawn)
        {
            base.Notify_Unequipped(pawn);
            RemoveSkillsFromOwner();
            EquipOwner = null;
            Messages.Message("The weapon has taken what it granted.", MessageTypeDefOf.PositiveEvent);
        }

        public override void Notify_EquipOwnerUsedVerb(Pawn pawn, Verb verb)
        {
            base.Notify_EquipOwnerUsedVerb(pawn, verb);

            Log.Message($"Notify_EquipOwnerUsedVerb {pawn} {verb}");
        }


        private void TeachOwnerSkills()
        {
            if (EquipOwner != null)
            {
                foreach (var item in absorbedAbilities)
                {
                    Teach(item);
                }
            }
        }

        private void Teach(AbilityDef Ability)
        {         
            if (EquipOwner != null)
            {
                EquipOwner.abilities.GainAbility(Ability);
                skillsTaughtToOwner.Add(Ability);
            }
         }

        private void RemoveSkillsFromOwner()
        {
            if (EquipOwner != null)
            {
                foreach (var item in skillsTaughtToOwner)
                {
                    EquipOwner.abilities.RemoveAbility(item);
                }

                skillsTaughtToOwner.Clear();
            }
        }


        public override DamageWorker.DamageResult Notify_ApplyMeleeDamageToTarget(LocalTargetInfo target, DamageWorker.DamageResult DamageWorkerResult)
        {
            base.Notify_ApplyMeleeDamageToTarget(target, DamageWorkerResult);


            if (target.Thing != null && target.Thing is Pawn TargetPawn)
            {
                AbsorbAbilities(TargetPawn);
            }

            return DamageWorkerResult;
        }

 
        public void AbsorbAbilities(Pawn targetPawn)
        {
            Log.Message($"AbsorbAbilities called for target: {targetPawn.Label}");
            if (targetPawn.abilities != null)
            {
                Log.Message($"Target has {targetPawn.abilities.abilities.Count} abilities");
                foreach (Ability ability in targetPawn.abilities.abilities)
                {
                    Log.Message($"Checking ability: {ability.def.label}");
                    if (!absorbedAbilities.Contains(ability.def))
                    {
                        absorbedAbilities.Add(ability.def);
                        Messages.Message($"{parent.Label} absorbed {ability.def.label} from {targetPawn.Label}!", MessageTypeDefOf.PositiveEvent);
                        Log.Message($"Absorbed ability: {ability.def.label}");

                        if (EquipOwner != null)
                        {
                            Teach(ability.def);
                        }
                    }
                }
            }
            else
            {
                Log.Message("Target has no abilities");
            }
        }

    }
}