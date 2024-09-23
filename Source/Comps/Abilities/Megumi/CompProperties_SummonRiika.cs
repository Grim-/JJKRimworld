using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace JJK
{
    public class CompProperties_SummonRiika : CompProperties_ToggleableEffect
    {
        public PawnKindDef riikaKindDef;

        public CompProperties_SummonRiika()
        {
            compClass = typeof(CompAbilityEffect_SummonRiika);
        }
    }

    public class CompAbilityEffect_SummonRiika : ToggleableCompAbilityEffect
    {
        public new CompProperties_SummonRiika Props => (CompProperties_SummonRiika)props;
        private HediffComp_YutaAbilities Storage => this.parent.pawn.GetYutaAbilityStorage();

        private Pawn summonedRiika;
        private Ability_Toggleable removedSummonAbility;

        public override void ApplyAbility(LocalTargetInfo target, LocalTargetInfo dest)
        {
            //handled by Ability_Toggleable leave empty
        }

        public override void Activate()
        {
            base.Activate();

            if (summonedRiika == null || !summonedRiika.Spawned)
            {
                SummonRiika();
                RemoveSummonAbility();

                //if (Storage != null)
                //{
                //    Storage.GrantRiikaAbilities(this.parent.pawn);
                //}

            }
        }




        public override void DeActivate()
        {
            base.DeActivate();
            UnsummonRiika();
            //if (Storage != null)
            //{
            //    Storage.RemoveRiikaAbilities(this.parent.pawn);
            //}
            ReaddSummonAbility();
        }



        private void SummonRiika()
        {
            summonedRiika = JJKUtility.SpawnShikigami(Props.riikaKindDef, parent.pawn, parent.pawn.Map, parent.pawn.Position);
        }

        private void UnsummonRiika()
        {
            if (summonedRiika != null && summonedRiika.Spawned)
            {
                summonedRiika.DeSpawn(DestroyMode.Vanish);
                summonedRiika = null;
            }
        }

        private void RemoveSummonAbility()
        {
            Pawn caster = parent.pawn;
            removedSummonAbility = (Ability_Toggleable)caster.abilities.GetAbility(JJKDefOf.JJK_RiikaSummon);
            if (removedSummonAbility != null)
            {
                if (removedSummonAbility.IsActive)
                {
                    removedSummonAbility.ForceDeactivate();
                }

                caster.abilities.RemoveAbility(removedSummonAbility.def);
            }
        }

        private void ReaddSummonAbility()
        {
            if (removedSummonAbility != null)
            {
                Pawn caster = parent.pawn;
                caster.abilities.GainAbility(removedSummonAbility.def);
                removedSummonAbility = null;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_References.Look(ref summonedRiika, "summonedRiika");
            Scribe_References.Look(ref removedSummonAbility, "removedSummonAbility");
        }
    }
}