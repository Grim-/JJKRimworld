using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace JJK
{
    public class CompProperties_GameOfLife : CompProperties
    {
        public int gridSize = 40; // Default to 11x11 grid (5 cells in each direction + center)

        public CompProperties_GameOfLife()
        {
            compClass = typeof(CompGameOfLife);
        }
    }

    public class CompGameOfLife : ThingComp
    {
        private int tickCounter = 0;
        private const int updateInterval = 60; // Update every 60 ticks (1 second)
        private Map map;
        private CompProperties_GameOfLife Props => (CompProperties_GameOfLife)props;

        private Dictionary<IntVec3, bool> State = new Dictionary<IntVec3, bool>();

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            map = parent.Map;
        }

        public override void CompTick()
        {
            base.CompTick();
            tickCounter++;

            if (tickCounter >= updateInterval)
            {
                UpdateGameOfLife();
                tickCounter = 0;
            }
        }
 
        private void UpdateGameOfLife()
        {
            var newState = new Dictionary<IntVec3, bool>(State);

            IntVec3 center = parent.Position;
            int radius = Props.gridSize / 2;

            for (int dx = -radius; dx <= radius; dx++)
            {
                for (int dz = -radius; dz <= radius; dz++)
                {
                    IntVec3 cell = center + new IntVec3(dx, 0, dz);
                    if (!cell.InBounds(map)) continue;

                    int neighbors = CountLiveNeighbors(cell);
                    bool isAlive = HasWall(cell);

                    if (neighbors == 4)
                    {
                        // State remains unchanged if 4 neighbors
                        newState[cell] = isAlive;
                    }
                    else if (isAlive)
                    {
                        // Cell survives if it has 2 neighbors
                        newState[cell] = neighbors == 2;
                    }
                    else
                    {
                        // Cell is born if it has 3, 4, or 5 neighbors
                        newState[cell] = neighbors >= 3 && neighbors <= 5;
                    }
                }
            }

            ApplyNewState(newState);
        }

        private int CountLiveNeighbors(IntVec3 cell)
        {
            int count = 0;
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dz = -1; dz <= 1; dz++)
                {
                    if (dx == 0 && dz == 0) continue;
                    IntVec3 neighbor = cell + new IntVec3(dx, 0, dz);
                    if (HasWall(neighbor)) count++;
                }
            }
            return count;
        }

        private bool HasWall(IntVec3 cell)
        {

            return cell.GetFirstBuilding(map)?.def.defName == "Wall";
        }

        private void ApplyNewState(Dictionary<IntVec3, bool> newState)
        {
            foreach (var kvp in newState)
            {
                IntVec3 cell = kvp.Key;
                bool shouldBeAlive = kvp.Value;

                if (shouldBeAlive && !HasWall(cell))
                {
                    if (cell.GetFirstBuilding(map) == this.parent)
                    {
                        continue;
                    }

                    Thing wall = ThingMaker.MakeThing(ThingDefOf.Wall, ThingDefOf.WoodLog);
                    GenSpawn.Spawn(wall, cell, map, Rot4.North);
                }
                else if (!shouldBeAlive && HasWall(cell))
                {
                    Thing wall = cell.GetFirstBuilding(map);
                    wall?.Destroy(DestroyMode.Vanish);
                }
            }


            State = newState;
        }
    }
}