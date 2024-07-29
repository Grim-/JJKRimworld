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
                AbsorbedData Summoner = manager.GetAbsorbDataForPawn(parent.pawn);
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
                                    Log.Message($"JJK: CLICKED SUMMON BUTTON");



                                    if (Summoner.SummonIsActiveOfKind(creature))
                                    {
                                        Log.Message($"JJK: ALREADY HAVE A CREATURE SUMMONED OF KIND {creature.label}");

                                        Pawn ActiveSummon = Summoner.GetActiveSummonOfKind(creature);

                                      
                                        if (ActiveSummon != null)
                                        {
                                            Log.Message($"JJK: {ActiveSummon.Label} {ActiveSummon.ThingID} IS THAT ACTIVE CREATURE.");

                                            if (Summoner.UnsummonCreature(ActiveSummon))
                                            {
                                                Messages.Message($"{parent.pawn.LabelShort} has unsummoned {ActiveSummon.LabelShort}.", MessageTypeDefOf.PositiveEvent);
                                            }
                                        }                                  
                                    }
                                    else
                                    {
                                        if (Summoner.CreateCreatureOfKind(creature))
                                        {
                                            parent.pawn.GetCursedEnergy()?.ConsumeCursedEnergy(Props.cursedEnergyCost);
                                        }
                                    }
                                },
                                () =>
                                {
                                    if (Summoner.HasAbsorbedCreatureKind(creature))
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
    }
}