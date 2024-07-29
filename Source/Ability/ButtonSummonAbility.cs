using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace JJK
{
    public class ButtonSummonAbility : Ability
    {
        public ButtonSummonAbility(Pawn pawn) : base(pawn)
        {
        }

        public ButtonSummonAbility(Pawn pawn, AbilityDef def) : base(pawn, def)
        {
        }

        public override IEnumerable<Gizmo> GetGizmosExtra()
        {
            foreach (Gizmo g in base.GetGizmosExtra())
            {
                yield return g;
            }

            var manager = JJKUtility.AbsorbedCreatureManager;
            if (manager != null)
            {
                AbsorbedData Summoner = manager.GetAbsorbDataForPawn(pawn);

                if (Summoner != null)
                {
                    List<PawnKindDef> absorbedCreatures = Summoner.AbsorbedCreatures.ToList();
                    if (absorbedCreatures.Count > 0)
                    {
                        //create a button gizmo for each summon type
                        yield return new Gizmo_MultiImageButton(
                            absorbedCreatures.Select(creature => new Gizmo_MultiOption(
                                creature.defName,
                                creature.race.uiIcon,
                                () => HandleSummonInput(Summoner, creature),
                                () => HandleDeletingAbsorbedKind(Summoner, creature)
                            )).ToList()
                        );
                    }
                }
            }
        }

        //left click
        private void HandleSummonInput(AbsorbedData Summoner, PawnKindDef CreatureKind)
        {
            if (Summoner.SummonIsActiveOfKind(CreatureKind))
            {
                Pawn SummonedCreature = Summoner.GetActiveSummonOfKind(CreatureKind);

                if (SummonedCreature != null)
                {
                    Summoner.UnsummonCreature(SummonedCreature);
                }

            }
            else
            {
                Summoner.CreateCreatureOfKind(CreatureKind);
            }
        }

        //right
        private void HandleDeletingAbsorbedKind(AbsorbedData Summoner, PawnKindDef creature)
        {
            if (Summoner.HasAbsorbedCreatureKind(creature))
            {
                Summoner.DeleteAbsorbedCreature(creature);
            }
         }
    }
}