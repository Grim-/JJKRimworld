using RimWorld;
using Verse;

namespace JJK
{
    public class CompProperties_ChangeFertilityInArea : CompProperties_AbilityEffect
    {
        public float radius = 3f;
        public float statIncrease = 0.5f;

        public CompProperties_ChangeFertilityInArea()
        {
            compClass = typeof(CompAbilityEffect_ChangeFertilityInArea);
        }
    }

    public class CompAbilityEffect_ChangeFertilityInArea : CompAbilityEffect
    {
        public new CompProperties_ChangeFertilityInArea Props => (CompProperties_ChangeFertilityInArea)props;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            Map map = parent.pawn.Map;
            if (map == null) return;

            foreach (IntVec3 cell in GenRadial.RadialCellsAround(target.Cell, Props.radius, true))
            {
                if (!cell.InBounds(map)) continue;

                TerrainDef terrain = cell.GetTerrain(map);
                terrain.fertility += Props.statIncrease;
            }
        }

        public override void DrawEffectPreview(LocalTargetInfo target)
        {
            GenDraw.DrawRadiusRing(target.Cell, Props.radius);
        }
    }
}