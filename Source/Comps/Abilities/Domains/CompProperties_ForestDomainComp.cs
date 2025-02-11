using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace JJK
{
    public class CompProperties_ForestDomainComp : CompProperties_DomainComp
    {
        public ThingDef TreeDef = ThingDefOf.Plant_TreeOak;
        public List<string> RandomHediffs;

        public CompProperties_ForestDomainComp()
        {
            compClass = typeof(CompForestDomain);
        }

    }

    public class CompForestDomain : CompDomainEffect
    {
        private List<Thing> spawnedTrees = new List<Thing>();
        public new CompProperties_ForestDomainComp Props => (CompProperties_ForestDomainComp)props;

        public override void ActivateDomain()
        {
            base.ActivateDomain();
            if (spawnedTrees.Count == 0)
            {
                SpawnTrees();
            }
        }
        public override void DeactivateDomain()
        {
            base.DeactivateDomain();

            DestroyTrees();
        }

        private void SpawnTrees()
        {
            if (parent?.Map == null || areaCells == null) return;
            foreach (IntVec3 cell in areaCells)
            {
                if (cell.Standable(parent.Map) && cell.GetFirstBuilding(parent.Map) == null)
                {
                    Plant tree = (Plant)GenSpawn.Spawn(Props.TreeDef, cell, parent.Map);
                    spawnedTrees.Add(tree);

                    float desiredGrowth = 0.5f;
                    tree.Growth = Random.Range(0.1f, 1f);
                }
            }
        }

        private void DestroyTrees()
        {
            foreach (Thing tree in spawnedTrees.ToList())
            {
                if (tree?.Destroyed == false)
                {
                    tree.Destroy();
                }
            }
            spawnedTrees.Clear();
        }

        public override void OnTick()
        {
            base.OnTick();

            List<Pawn> pawnsInRadius = GetPawnsInDomain();

            foreach (var pawn in pawnsInRadius)
            {
                if (!pawn.Dead && !pawn.IsImmuneToDomainSureHit() && pawn.ThingID != _DomainCaster.ThingID)
                {
                    string SelectedHediffName = Props.RandomHediffs[Random.Range(0, Props.RandomHediffs.Count)];

                    HediffDef hediffDef = DefDatabase<HediffDef>.GetNamed(SelectedHediffName);

                    if (hediffDef != null)
                    {
                        Hediff hediffInstance = pawn.health.GetOrAddHediff(hediffDef);
                        hediffInstance.Severity += 0.1f;
                    }            
                }
            }


            //remove a random spawned tree and increase the overall poison severity,

            Thing SelectedTree = spawnedTrees[Random.Range(0, spawnedTrees.Count)];

            if (SelectedTree != null && !SelectedTree.Destroyed)
            {
                SelectedTree.Destroy();
            }
        }

        public override void ApplyDomainEffects()
        {

        }

        public override void RemoveDomainEffects()
        {
            DestroyTrees();
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Collections.Look(ref spawnedTrees, "spawnedTrees", LookMode.Reference);

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                Log.Message($"POST LOAD INIT FOR FOREST DOMAIN COMP TREE REFERENCES IN LIST {spawnedTrees.Count}");
                spawnedTrees = spawnedTrees.Where(t => t != null && !t.Destroyed).ToList();
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (respawningAfterLoad)
            {
                // Clean up any destroyed trees from our list
                spawnedTrees = spawnedTrees.Where(t => t != null && !t.Destroyed).ToList();

                if (IsDomainActive && spawnedTrees.Count == 0)
                {
                    SpawnTrees();
                }
            }
        }
    }

}