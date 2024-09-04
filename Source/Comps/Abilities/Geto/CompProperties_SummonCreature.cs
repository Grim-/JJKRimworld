﻿using JJK;
using RimWorld;
using System.Collections.Generic;
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
            // This method is empty as the functionality is handled by button gizmos
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            Hediff_CursedSpiritManipulator hediff = parent.pawn.health.GetOrAddHediff(JJKDefOf.JJK_CursedSpiritManipulator) as Hediff_CursedSpiritManipulator;
            if (hediff == null)
            {
                Log.Error($"JJK: No Hediff_CursedSpiritManipulator found for {parent.pawn.LabelShort} {parent.pawn.ThingID}");
                yield break;
            }

            List<PawnKindDef> absorbedCreatures = hediff.GetAbsorbedCreatures();
            if (absorbedCreatures.Count == 0)
            {
                yield break;
            }

            var options = new List<Gizmo_MultiOption>();
            foreach (var creature in absorbedCreatures)
            {
                options.Add(new Gizmo_MultiOption(
                    creature.defName,
                    creature.race.uiIcon,
                    () => ToggleSummon(hediff, creature),
                    () => DeleteAbsorbedCreature(hediff, creature)
                ));
            }

            yield return new Gizmo_MultiImageButton(options);
        }

        private void ToggleSummon(Hediff_CursedSpiritManipulator hediff, PawnKindDef creature)
        {
            if (hediff.IsCreatureActive(creature))
            {
                UnsummonCreature(hediff, creature);
            }
            else
            {
                SummonCreature(hediff, creature);
            }
        }

        private void UnsummonCreature(Hediff_CursedSpiritManipulator hediff, PawnKindDef creature)
        {
            Pawn activeSummon = hediff.GetActiveSummonOfKind(creature);
            if (activeSummon != null)
            {
                if (hediff.UnsummonCreature(activeSummon))
                {
                    Messages.Message($"{parent.pawn.LabelShort} has unsummoned {activeSummon.LabelShort}.", MessageTypeDefOf.PositiveEvent);
                }
            }
        }

        private void SummonCreature(Hediff_CursedSpiritManipulator hediff, PawnKindDef creature)
        {
            if (hediff.SummonCreature(creature))
            {
                parent.pawn.GetCursedEnergy()?.ConsumeCursedEnergy(Props.cursedEnergyCost);
                Messages.Message($"{parent.pawn.LabelShort} has summoned a {creature.label}.", MessageTypeDefOf.PositiveEvent);
            }
        }

        private void DeleteAbsorbedCreature(Hediff_CursedSpiritManipulator hediff, PawnKindDef creature)
        {
            if (hediff.HasAbsorbedCreatureKind(creature))
            {
                hediff.DeleteAbsorbedCreature(creature);
                Messages.Message($"{parent.pawn.LabelShort} has forgotten how to summon {creature.label}.", MessageTypeDefOf.NeutralEvent);
            }
        }
    }
}

//using RimWorld;
//using System.Collections.Generic;
//using System.Linq;
//using Verse;

//namespace JJK
//{
//    public class CompProperties_SummonCreature : CompProperties_CursedAbilityProps
//    {
//        public CompProperties_SummonCreature()
//        {
//            compClass = typeof(CompAbilityEffect_SummonCreature);
//        }
//    }
//    public class CompAbilityEffect_SummonCreature : BaseCursedEnergyAbility
//    {
//        public new CompProperties_SummonCreature Props => (CompProperties_SummonCreature)props;

//        public override void ApplyAbility(LocalTargetInfo target, LocalTargetInfo dest)
//        {
//            //does not need to do anything handled by button gizmos
//        }

//        public override IEnumerable<Gizmo> CompGetGizmosExtra()
//        {
//            var manager = JJKUtility.AbsorbedCreatureManager;
//            if (manager != null)
//            {
//                AbsorbedData Summoner = manager.GetAbsorbDataForPawn(parent.pawn);
//                if (Summoner != null)
//                {
//                    List<PawnKindDef> absorbedCreatures = Summoner.AbsorbedCreatures;
//                    if (absorbedCreatures.Count > 0)
//                    {
//                        var options = new List<Gizmo_MultiOption>();
//                        foreach (var creature in absorbedCreatures)
//                        {
//                            options.Add(new Gizmo_MultiOption(
//                                creature.defName,
//                                creature.race.uiIcon,
//                                () =>
//                                {
//                                    Log.Message($"JJK: CLICKED SUMMON BUTTON");



//                                    if (Summoner.SummonIsActiveOfKind(creature))
//                                    {
//                                        Log.Message($"JJK: ALREADY HAVE A CREATURE SUMMONED OF KIND {creature.label}");

//                                        Pawn ActiveSummon = Summoner.GetActiveSummonOfKind(creature);


//                                        if (ActiveSummon != null)
//                                        {
//                                            Log.Message($"JJK: {ActiveSummon.Label} {ActiveSummon.ThingID} IS THAT ACTIVE CREATURE.");

//                                            if (Summoner.UnsummonCreature(ActiveSummon))
//                                            {
//                                                Messages.Message($"{parent.pawn.LabelShort} has unsummoned {ActiveSummon.LabelShort}.", MessageTypeDefOf.PositiveEvent);
//                                            }
//                                        }
//                                    }
//                                    else
//                                    {
//                                        if (Summoner.CreateCreatureOfKind(creature))
//                                        {
//                                            parent.pawn.GetCursedEnergy()?.ConsumeCursedEnergy(Props.cursedEnergyCost);
//                                        }
//                                    }
//                                },
//                                () =>
//                                {
//                                    if (Summoner.HasAbsorbedCreatureKind(creature))
//                                    {
//                                        Summoner.DeleteAbsorbedCreature(creature);
//                                    }
//                                }
//                            ));
//                        }
//                        yield return new Gizmo_MultiImageButton(options);
//                    }
//                    else
//                    {
//                        //no creatures
//                    }
//                }
//                else
//                {
//                    //no summoner
//                    Log.Message($"JJK: CSUMMONCREATE NO AbsorbedData FOR {parent.pawn} {parent.pawn.ThingID}");
//                }
//            }
//            else
//            {
//                //cant find manager
//            }
//        }
//    }
//}