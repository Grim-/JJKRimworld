using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace JJK
{
    public class CompProperties_DomainComp : CompProperties
    {
        public float DomainCastCost = 10;
        public float DomainCostPerTick = 4;
        public ThingDef WallDef;
        public int TicksBetweenApplication = 100;
        public float AreaRadius = 10f;

        public CompProperties_DomainComp()
        {
            compClass = typeof(CompDomainEffect);
        }
    }

    public abstract class CompDomainEffect : ThingComp
    {
        public CompProperties_DomainComp Props => (CompProperties_DomainComp)props;

        protected bool _IsDomainActive = false;
        public bool IsDomainActive => _IsDomainActive;

        protected int CurrentTick = 0;
        protected Pawn _DomainCaster;
        public Pawn DomainCaster => _DomainCaster;

        protected HashSet<IntVec3> areaCells;
        protected HashSet<IntVec3> wallCells;
        private HashSet<IntVec3> AddedFilth = new HashSet<IntVec3>();
        protected List<Thing> constructedWalls;
        public HashSet<IntVec3> GetDomainAreaCells => areaCells;

        private Dictionary<IntVec3, TerrainDef> originalTerrain = new Dictionary<IntVec3, TerrainDef>();
        private Dictionary<IntVec3, List<Thing>> originalThings = new Dictionary<IntVec3, List<Thing>>();
        private Dictionary<IntVec3, List<Thing>> originalWallCellContents = new Dictionary<IntVec3, List<Thing>>();
        private bool terrainChanged = false;
        private bool wallsConstructed = false;

        public CompDomainEffect() 
        {

        }
        public virtual void ActivateDomain()
        {
            if (!_IsDomainActive)
            {
                _IsDomainActive = true;
                ConstructDomainWall();
                ChangeTerrain();
                ApplyDomainEffects();
                if (parent?.Map != null)
                {
                    MoteMaker.MakeStaticMote(parent.Position, parent.Map, ThingDefOf.Mote_ChargingCablesPulse);
                }
            }
        }

        public virtual void DeactivateDomain()
        {
            if (_IsDomainActive)
            {
                _IsDomainActive = false;
                RemoveDomainWall();
                RevertTerrain();
                RemoveDomainEffects();
                if (parent?.Map != null)
                {
                    MoteMaker.MakeStaticMote(parent.Position, parent.Map, ThingDefOf.Mote_ChargingCablesPulse);
                }
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref _IsDomainActive, "isDomainActive", false);
            Scribe_Values.Look(ref CurrentTick, "currentTick", 0);
            Scribe_References.Look(ref _DomainCaster, "domainCaster");
            Scribe_Collections.Look(ref constructedWalls, "constructedWalls", LookMode.Reference);
            Scribe_Collections.Look(ref originalTerrain, "originalTerrain", LookMode.Value, LookMode.Def);
            Scribe_Collections.Look(ref originalWallCellContents, "originalWallCellContents", LookMode.Value, LookMode.Deep);
            Scribe_Values.Look(ref wallsConstructed, "wallsConstructed", false);
            Scribe_Collections.Look(ref originalThings, "originalThings", LookMode.Value, LookMode.Deep);
            Scribe_Values.Look(ref terrainChanged, "terrainChanged", false);

            if (Scribe.mode == LoadSaveMode.Saving)
            {
                List<Vector3> areaPositions = areaCells?.Select(c => c.ToVector3()).ToList();
                Scribe_Collections.Look(ref areaPositions, "areaCells", LookMode.Value);
            }
            else if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                List<Vector3> areaPositions = null;
                Scribe_Collections.Look(ref areaPositions, "areaCells", LookMode.Value);
                if (areaPositions != null)
                {
                    areaCells = new HashSet<IntVec3>(areaPositions.Select(v => v.ToIntVec3()));
                }
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            CalculateAreaCells();
            CalculateWallCells();

            if (respawningAfterLoad && _IsDomainActive)
            {
                if (terrainChanged)
                {
                    ReapplyTerrainChanges();
                }
                else
                {
                    ChangeTerrain();
                    CreateFilth();
                }
            }
        }

        public override void PostDeSpawn(Map map)
        {
            RemoveDomainWall();
            RemoveFilth();
            RevertTerrain();
            RemoveDomainWall();
            base.PostDeSpawn(map);
        }

        public override void PostDraw()
        {
            base.PostDraw();
            if (_IsDomainActive)
            {
                DrawDomainRadius();
            }
        }

        private void DrawDomainRadius()
        {
            GenDraw.DrawRadiusRing(parent.Position, Props.AreaRadius);
        }

        public void SetCaster(Pawn Pawn)
        {
            _DomainCaster = Pawn;
        }

        public void SupressEffectForCells(IEnumerable<IntVec3> cells)
        {

        }

        private void ChangeTerrain()
        {
            foreach (IntVec3 cell in GenRadial.RadialCellsAround(parent.Position, Props.AreaRadius, true))
            {
                if (cell.InBounds(parent.Map))
                {
                    // Store original terrain
                    TerrainDef currentTerrain = cell.GetTerrain(parent.Map);
                    if (!originalTerrain.ContainsKey(cell))
                    {
                        originalTerrain[cell] = currentTerrain;
                    }

                    // Store original things (plants, etc.)
                    List<Thing> cellThings = cell.GetThingList(parent.Map)
                        .Where(t => t.def.category == ThingCategory.Plant)
                        .ToList();
                    if (cellThings.Any())
                    {
                        originalThings[cell] = cellThings;
                        foreach (Thing thing in cellThings)
                        {
                            thing.DeSpawn();
                        }
                    }

                    // Change terrain to shallow water
                    parent.Map.terrainGrid.SetTerrain(cell, TerrainDefOf.WaterShallow);
                }
            }
            terrainChanged = true;
        }

        private void RevertTerrain()
        {
            if (terrainChanged)
            {
                foreach (var kvp in originalTerrain)
                {
                    IntVec3 cell = kvp.Key;
                    TerrainDef terrain = kvp.Value;

                    if (cell.InBounds(parent.Map))
                    {
                        // Restore original terrain
                        parent.Map.terrainGrid.SetTerrain(cell, terrain);

                        // Restore original things
                        if (originalThings.TryGetValue(cell, out List<Thing> cellThings))
                        {
                            foreach (Thing thing in cellThings)
                            {
                                if (!thing.Spawned)
                                {
                                    GenSpawn.Spawn(thing, cell, parent.Map);
                                }
                            }
                        }
                    }
                }
                originalTerrain.Clear();
                originalThings.Clear();
                terrainChanged = false;
            }
        }

        private void ReapplyTerrainChanges()
        {
            foreach (var kvp in originalTerrain)
            {
                if (kvp.Key.InBounds(parent.Map))
                {
                    parent.Map.terrainGrid.SetTerrain(kvp.Key, TerrainDefOf.WaterShallow);
                }
            }
        }



        private void CreateFilth()
        {
            AddedFilth.Clear();
            foreach (IntVec3 cell in GetCellsInDomain())
            {
                if (FilthMaker.CanMakeFilth(cell, parent.Map, ThingDefOf.Filth_Blood))
                {
                    if (FilthMaker.TryMakeFilth(cell, parent.Map, ThingDefOf.Filth_Blood))
                    {
                        AddedFilth.Add(cell);
                    }
                }
            }
        }

        private void RemoveFilth()
        {
            foreach (IntVec3 cell in AddedFilth)
            {
                FilthMaker.RemoveAllFilth(cell, parent.Map);
            }
            AddedFilth.Clear();
        }
        public override void CompTick()
        {
            base.CompTick();
            if (_IsDomainActive)
            {
                if (_DomainCaster != null && _DomainCaster.Dead)
                {
                    DestroyDomain();
                    return;
                }

                CurrentTick++;

                if (CurrentTick >= Props.TicksBetweenApplication)
                {
                    OnTick();
                    CurrentTick = 0;
                }
            }
        }

        public virtual void OnTick()
        {
            if (_DomainCaster == null)
            {
                return;
            }

            Gene_CursedEnergy _CursedEnergy = _DomainCaster.GetCursedEnergy();

            if (_CursedEnergy == null)
            {
                return;
            }
        }

        protected virtual void CalculateAreaCells()
        {
            if (parent?.Map == null)
            {
                Log.Error("CalculateAreaCells: parent.Map is null");
                return;
            }

            areaCells = new HashSet<IntVec3>(GenRadial.RadialCellsAround(parent.Position, Props.AreaRadius, true)
                .Where(c => c.InBounds(parent.Map)));
        }

        protected virtual void CalculateWallCells()
        {
            //Log.Message($"CalculateWallCells called. Parent: {parent}, Map: {parent?.Map}, AreaRadius: {Props.AreaRadius}");
            if (parent?.Map == null || areaCells == null)
            {
                Log.Error("CalculateWallCells: parent.Map is null or areaCells is null");
                return;
            }

            wallCells = new HashSet<IntVec3>();
            IntVec3 center = parent.Position;
            IEnumerable<IntVec3> outerRing = GenRadial.RadialCellsAround(center, Props.AreaRadius, true);
            IEnumerable<IntVec3> innerRing = GenRadial.RadialCellsAround(center, Props.AreaRadius - 1, true);
            IEnumerable<IntVec3> edgeCells = outerRing.Except(innerRing);

            foreach (IntVec3 cell in edgeCells)
            {
                if (cell.InBounds(parent.Map))
                {
                    wallCells.Add(cell);
                }
            }
        }



        public virtual void DestroyDomain()
        {
            DeactivateDomain();
            parent.Destroy();
        }

        public virtual void ConstructDomainWall()
        {
            CalculateAreaCells();
            CalculateWallCells();

            constructedWalls = new List<Thing>();

            foreach (IntVec3 cell in wallCells)
            {
                if (cell.InBounds(parent.Map))
                {
                    // Store original cell contents
                    List<Thing> cellContents = cell.GetThingList(parent.Map)
                        .Where(t => t.def.category != ThingCategory.Pawn)
                        .ToList();
                    if (cellContents.Any())
                    {
                        originalWallCellContents[cell] = cellContents;
                        foreach (Thing thing in cellContents)
                        {
                            thing.DeSpawn();
                        }
                    }

                    // Construct the wall
                    Thing wall = ThingMaker.MakeThing(ThingDefOf.RaisedRocks);
                    if (wall != null)
                    {
                        Thing spawnedWall = GenSpawn.Spawn(wall, cell, parent.Map);
                        if (spawnedWall != null)
                        {
                            constructedWalls.Add(spawnedWall);
                        }
                    }
                }
            }
            wallsConstructed = true;
        }


        public virtual void RemoveDomainWall()
        {
            if (wallsConstructed)
            {
                foreach (Thing wall in constructedWalls)
                {
                    if (!wall.Destroyed)
                    {
                        wall.Destroy();
                    }
                }
                constructedWalls.Clear();

                // Restore original cell contents
                foreach (var kvp in originalWallCellContents)
                {
                    IntVec3 cell = kvp.Key;
                    List<Thing> cellContents = kvp.Value;

                    if (cell.InBounds(parent.Map))
                    {
                        foreach (Thing thing in cellContents)
                        {
                            if (!thing.Spawned)
                            {
                                GenSpawn.Spawn(thing, cell, parent.Map);
                            }
                        }
                    }
                }
                originalWallCellContents.Clear();
                wallsConstructed = false;
            }
        }

        public override void PostDrawExtraSelectionOverlays()
        {
            base.PostDrawExtraSelectionOverlays();
            if (_IsDomainActive && areaCells != null)
            {
                GenDraw.DrawFieldEdges(areaCells.ToList(), Color.cyan);
            }
        }
        public virtual IEnumerable<IntVec3> GetCellsInDomain()
        {
            return GenRadial.RadialCellsAround(parent.Position, Props.AreaRadius, true);
        }

        public virtual List<Pawn> GetPawnsInDomain()
        {
            return GetCellsInDomain().SelectMany(c => c.GetThingList(parent.Map))
                .OfType<Pawn>().ToList();
        }

        public virtual List<Thing> GetThingsInDomain()
        {
            return GetCellsInDomain().SelectMany(c => c.GetThingList(parent.Map))
                .OfType<Thing>().ToList();
        }

        public virtual bool CanDomainAffect(Pawn TargetPawn)
        {
            return !TargetPawn.health.hediffSet.HasHediff(JJKDefOf.JJK_HollowWickerBasket) || !TargetPawn.health.hediffSet.HasHediff(JJKDefOf.JJK_SimpleShadowDomain);
        }

        public abstract void ApplyDomainEffects();
        public abstract void RemoveDomainEffects();


    }


}