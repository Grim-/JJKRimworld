using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace JJK
{
    public class Ability_Toggleable : Ability
    {
        public bool IsActive = false;

        public event Action OnToggle;

        public Ability_Toggleable() : base ()
        {

        }

        public Ability_Toggleable(Pawn pawn) : base(pawn)
        {
        }

        public Ability_Toggleable(Pawn pawn, AbilityDef def) : base(pawn, def)
        {

        }
        public Ability_Toggleable(Pawn pawn, Precept sourcePrecept, AbilityDef def) : base(pawn, sourcePrecept, def)
        {

        }

       

        public override IEnumerable<Command> GetGizmos()
        {
            if (!this.pawn.Drafted || this.def.showWhenDrafted)
            {
                Command_ToggleAbility gizmo = new Command_ToggleAbility(pawn, this, IsActive, Toggle);
                gizmo.Order = this.def.uiOrder;
                yield return gizmo;
            }
        }

        public virtual void Toggle()
        {
            IsActive = !IsActive;
            OnToggle?.Invoke();
        }
    }

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

            var manager = Find.World.GetComponent<AbsorbedCreatureManager>();
            if (manager != null)
            {
                PawnAbsorbption Summoner = manager.GetOrCreateSummoner(pawn);

                if (Summoner != null)
                {
                    List<PawnKindDef> absorbedCreatures = Summoner.AbsorbedCreatures.ToList();
                    if (absorbedCreatures.Count > 0)
                    {
                        yield return new Gizmo_MultiImageButton(
                            absorbedCreatures.Select(creature => new Gizmo_MultiOption(
                                creature.defName,
                                creature.race.uiIcon,
                                () =>
                                {
                                    if (Summoner.SummonIsActiveOfKind(creature))
                                    {
                                        UnSummonCreature(pawn, creature);
                                    }
                                    else
                                    {
                                        SummonCreature(pawn, creature);
                                    }
                                },
                                () =>
                                {
                                    if (Summoner.HasSummonType(creature))
                                    {
                                        Summoner.DeleteAbsorbedCreature(creature);
                                    }
                                }
                            )).ToList()
                        );
                    }
                }
            }
        }

        private void SummonCreature(Pawn caster, PawnKindDef creature)
        {
            var manager = Find.World.GetComponent<AbsorbedCreatureManager>();

            if (manager != null)
            {
                PawnAbsorbption Summoner = manager.GetOrCreateSummoner(caster);

                if (Summoner != null)
                {
                    if (Summoner.SummonCreature(creature))
                    {
                        Messages.Message($"{caster.LabelShort} has summoned {creature.label}.", MessageTypeDefOf.PositiveEvent);
                    }
                }
            }
        }

        private void UnSummonCreature(Pawn caster, PawnKindDef creature)
        {
            var manager = Find.World.GetComponent<AbsorbedCreatureManager>();

            if (manager != null)
            {
                PawnAbsorbption Summoner = manager.GetOrCreateSummoner(caster);

                if (Summoner != null)
                {
                    Summoner.UnsummonCreature(creature);
                    Messages.Message($"{caster.LabelShort} has unsummoned {creature.label}.", MessageTypeDefOf.PositiveEvent);
                }
            }
        }
    }
}