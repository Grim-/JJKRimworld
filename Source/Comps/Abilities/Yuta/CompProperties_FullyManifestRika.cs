using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace JJK
{
    public class CompProperties_FullyManifestRika : CompProperties_ToggleableEffect
    {
        public PawnKindDef riikaKindDef;

        public CompProperties_FullyManifestRika()
        {
            compClass = typeof(CompAbilityEffect_FullyManifestRika);
        }
    }

    public class CompAbilityEffect_FullyManifestRika : ToggleableCompAbilityEffect
    {
        public new CompProperties_FullyManifestRika Props => (CompProperties_FullyManifestRika)props;
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
            }
        }

        public override void DeActivate()
        {
            base.DeActivate();
            UnsummonRiika();
            ReaddSummonAbility();
        }

        private void SummonRiika()
        {
            summonedRiika = JJKUtility.SpawnShikigami(Props.riikaKindDef, parent.pawn, parent.pawn.Map, parent.pawn.Position);

            Comp_OnDeathHandler onDeathHandler = summonedRiika.TryGetComp<Comp_OnDeathHandler>();

            if (onDeathHandler != null)
            {
                onDeathHandler.OnDeath += OnDeathHandler_OnDeath;
            }
        }

        private void OnDeathHandler_OnDeath(Thing obj)
        {
            if (this.parent is Ability_Toggleable toggleable)
            {
                toggleable.ForceDeactivate();
            }

            Ability partiallyManifest = this.parent.pawn.abilities.GetAbility(JJKDefOf.JJK_RiikaPartialManifest);

            if (partiallyManifest != null)
            {
                partiallyManifest.StartCooldown(1250);
            }

            this.parent.StartCooldown(1250);
        }

        private void UnsummonRiika()
        {
            if (summonedRiika != null && summonedRiika.Spawned)
            {
                Comp_OnDeathHandler onDeathHandler = summonedRiika.TryGetComp<Comp_OnDeathHandler>();

                if (onDeathHandler != null)
                {
                    onDeathHandler.OnDeath -= OnDeathHandler_OnDeath;
                }
                summonedRiika.DeSpawn(DestroyMode.Vanish);
                summonedRiika = null;
            }
        }

        private void RemoveSummonAbility()
        {
            Pawn caster = parent.pawn;
            removedSummonAbility = (Ability_Toggleable)caster.abilities.GetAbility(JJKDefOf.JJK_RiikaPartialManifest);
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

                if (!caster.HasAbility(removedSummonAbility.def))
                {
                    caster.abilities.GainAbility(removedSummonAbility.def);
                }   
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