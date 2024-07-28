using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace JJK
{
    public class CompProperties_SummonCreature : CompProperties_CursedAbilityProps
    {
        public CompProperties_SummonCreature()
        {
            compClass = typeof(CompAbilityEffect_SummonCreature);
        }
    }
    public class CompAbilityEffect_SummonCreature : BaseCursedEnergyAbility
    {
        public new CompProperties_SummonCreature Props => (CompProperties_SummonCreature)props;

        public override void ApplyAbility(LocalTargetInfo target, LocalTargetInfo dest)
        {
            //does not need to do anything handled by button gizmos
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            var manager = JJKUtility.AbsorbedCreatureManager;
            if (manager != null)
            {
                PawnAbsorbption Summoner = manager.GetOrCreateSummoner(parent.pawn);

                if (Summoner != null)
                {
                    List<PawnKindDef> absorbedCreatures = Summoner.AbsorbedCreatures.ToList();
                    if (absorbedCreatures.Count > 0)
                    {
                        var options = new List<Gizmo_MultiOption>();
                        foreach (var creature in absorbedCreatures)
                        {
                            options.Add(new Gizmo_MultiOption(
                                creature.defName,
                                creature.race.uiIcon,
                                () =>
                                {
                                    if (Summoner.SummonIsActiveOfKind(creature))
                                    {
                                        UnSummonCreature(parent.pawn, creature);
                                    }
                                    else
                                        SummonCreature(parent.pawn, creature);
                                },
                                () =>
                                {
                                    if (Summoner.HasSummonType(creature))
                                    {
                                        Summoner.DeleteAbsorbedCreature(creature);
                                    }
                                }
                            ));
                        }
                        yield return new Gizmo_MultiImageButton(options);
                    }
                    else
                    {
                        //no creatures
                    }
                }
                else
                {
                    //no summoner
                }
            }
            else
            {
                //cant find manager
            }
        }

        public void SummonCreature(Pawn caster, PawnKindDef creature)
        {
            var manager = Find.World.GetComponent<AbsorbedCreatureManager>();

            if (manager != null)
            {
                PawnAbsorbption Summoner = manager.GetOrCreateSummoner(caster);

                if (Summoner != null)
                {
                    if (Summoner.SummonCreature(creature))
                    {
                        caster.GetCursedEnergy()?.ConsumeCursedEnergy(Props.cursedEnergyCost);
                        Messages.Message($"{caster.LabelShort} has summoned {creature.label}.", MessageTypeDefOf.PositiveEvent);
                    }                   
                }                  
            }          
        }

        public void UnSummonCreature(Pawn caster, PawnKindDef creature)
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