using RimWorld;
using Verse;
using Verse.AI;

namespace JJK
{
    public class CompProperties_BoundBeast : CompProperties_CursedAbilityProps
    {
        public PawnKindDef summonedCreatureKind;
        public int maxDistance = 5;

        public CompProperties_BoundBeast()
        {
            compClass = typeof(CompAbilityEffect_BoundBeast);
        }
    }

    public class CompAbilityEffect_BoundBeast : BaseCursedEnergyAbility
    {
        private Pawn summonedCreature;

        public new CompProperties_BoundBeast Props => (CompProperties_BoundBeast)props;

        public override void ApplyAbility(LocalTargetInfo target, LocalTargetInfo dest)
        {
            if (summonedCreature == null || !summonedCreature.Spawned)
            {
                SummonCreature(target);
            }
            else
            {
                UnsummonCreature();
            }
        }

        private void SummonCreature(LocalTargetInfo target)
        {
            IntVec3 position = target.Cell;
            Map map = parent.pawn.Map;

            if (!TryFindSpawnCell(position, map, out IntVec3 spawnCell))
            {
                Messages.Message("Cannot find a valid spawn location.", MessageTypeDefOf.RejectInput);
                return;
            }

            PawnKindDef pawnKindDef = Props.summonedCreatureKind;
            Pawn pawn = PawnGenerator.GeneratePawn(pawnKindDef, parent.pawn.Faction);
            GenSpawn.Spawn(pawn, parent.pawn.Position, map);
            pawn.health.AddHediff(JJKDefOf.JJ_SummonedCreatureTag);


            SummonedCreatureManager summonManager = JJKUtility.SummonedCreatureManager;
            summonManager.RegisterSummon(pawn, parent.pawn);


            summonedCreature = pawn;
            Messages.Message($"{parent.pawn.LabelShort} has summoned a {pawn.KindLabel}.", MessageTypeDefOf.PositiveEvent);
        }

        private void UnsummonCreature()
        {
            if (summonedCreature != null && summonedCreature.Spawned)
            {
                SummonedCreatureManager summonManager = JJKUtility.SummonedCreatureManager;
                summonManager.UnregisterSummon(summonedCreature);
                Messages.Message($"{parent.pawn.LabelShort} has unsummoned the {summonedCreature.KindLabel}.", MessageTypeDefOf.PositiveEvent);
                summonedCreature.Destroy(DestroyMode.Vanish);
                summonedCreature = null;
            }
        }

        private bool TryFindSpawnCell(IntVec3 center, Map map, out IntVec3 result)
        {
            return CellFinder.TryFindRandomCellNear(center, map, Props.maxDistance,
                (IntVec3 c) => c.Standable(map) && !c.Fogged(map), out result);
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_References.Look(ref summonedCreature, "summonedCreature");
        }
    }
}

    