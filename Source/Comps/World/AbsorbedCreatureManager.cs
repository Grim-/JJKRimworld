using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using Verse;

using RimWorld.Planet;
using System.Collections.Generic;
using Verse;

namespace JJK
{
    public class AbsorbedCreatureManager : WorldComponent
    {
        private Dictionary<int, Summoner> Summoners = new Dictionary<int, Summoner>();

        public AbsorbedCreatureManager(World world) : base(world) { }

        public Summoner GetOrCreateSummoner(Pawn pawn)
        {
            if (!Summoners.TryGetValue(pawn.thingIDNumber, out Summoner summoner))
            {
                summoner = new Summoner(pawn);
                Summoners[pawn.thingIDNumber] = summoner;
            }
            return summoner;
        }

        public void AbsorbCreature(Pawn summoner, Pawn creature)
        {
            GetOrCreateSummoner(summoner).AbsorbCreature(creature.kindDef);
        }

        public bool SummonCreature(Pawn summoner, Pawn creatureToSummon)
        {
            Summoner summonerData = GetOrCreateSummoner(summoner);
            if (summonerData.SummonCreature(creatureToSummon.kindDef))
            {
                IntVec3 spawnPosition = CellFinder.RandomClosewalkCellNear(summoner.Position, summoner.Map, 2);
                Pawn spawnedCreature = PawnGenerator.GeneratePawn(creatureToSummon.kindDef, summoner.Faction);
                GenSpawn.Spawn(spawnedCreature, spawnPosition, summoner.Map);
                summonerData.AddActiveSummon(spawnedCreature);
                return true;
            }
            return false;
        }

        public bool UnsummonCreature(Pawn summoner, Pawn creature)
        {
            Summoner summonerData = GetOrCreateSummoner(summoner);
            if (summonerData != null && summonerData.UnsummonCreature(creature))
            {
                if (creature.Spawned)
                {
                    creature.DeSpawn(DestroyMode.Vanish);
                }
                return true;
            }
            return false;
        }

        public bool DeleteAbsorbedCreature(Pawn summoner, Pawn creature)
        {
            if (HasAbsorbedCreatue(summoner, creature))
            {
                Summoners[summoner.thingIDNumber].RemoveSummon(creature.kindDef);
                return true;
            }
            return false;
        }

        public bool HasActiveSummons(Pawn summoner)
        {
            return GetOrCreateSummoner(summoner).HasActiveSummons();
        }

        public bool HasAbsorbedCreatue(Pawn summoner, Pawn Creature)
        {
            if (HasSummoner(summoner))
            {
                return Summoners[summoner.thingIDNumber].HasSummonType(Creature.kindDef);
            }
            return false;
        }

        public bool IsActiveAbsorbedCreature(Pawn summoner, Pawn Creature)
        {
            if (HasSummoner(summoner))
            {
                return Summoners[summoner.thingIDNumber].SummonIsActive(Creature.kindDef);
            }
            return false;
        }

        public bool HasSummoner(Pawn Summoner)
        {
            return Summoners.ContainsKey(Summoner.thingIDNumber);
        }

        public List<Pawn> GetAbsorbedCreatures(Pawn summoner)
        {
            var kindDefs = GetOrCreateSummoner(summoner).AbsorbedCreatures;
            return kindDefs.Select(kd => PawnGenerator.GeneratePawn(kd)).ToList();
        }

        public HashSet<Pawn> GetActiveSummons(Pawn summoner)
        {
            return new HashSet<Pawn>(GetOrCreateSummoner(summoner).ActiveSummons);
        }

        public override void ExposeData()
        {
            Scribe_Collections.Look(ref Summoners, "Summoners", LookMode.Value, LookMode.Deep);
        }

        public override void FinalizeInit()
        {
            base.FinalizeInit();
            CleanupDestroyedPawns();
        }

        private void CleanupDestroyedPawns()
        {
            List<int> SummonersToRemove = new List<int>();

            foreach (var kvp in Summoners)
            {
                Summoner summoner = kvp.Value;
                summoner.ActiveSummons.RemoveWhere(p => p == null || p.Destroyed);

                if (summoner.PawnReference == null || summoner.PawnReference.Destroyed)
                {
                    SummonersToRemove.Add(kvp.Key);
                }
            }

            foreach (int SummonerID in SummonersToRemove)
            {
                Summoners.Remove(SummonerID);
            }
        }
    }

    public class Summoner : IExposable
    {
        public Pawn PawnReference;
        public HashSet<PawnKindDef> AbsorbedCreatures = new HashSet<PawnKindDef>();
        public HashSet<Pawn> ActiveSummons = new HashSet<Pawn>();

        public Summoner(Pawn pawn)
        {
            PawnReference = pawn;
        }

        public void ExposeData()
        {
            Scribe_References.Look(ref PawnReference, "PawnReference");
            Scribe_Collections.Look(ref AbsorbedCreatures, "AbsorbedCreatures", LookMode.Def);
            Scribe_Collections.Look(ref ActiveSummons, "ActiveSummons", LookMode.Reference);
        }

        public void AbsorbCreature(PawnKindDef creatureKind)
        {
            AbsorbedCreatures.Add(creatureKind);
        }

        public bool SummonCreature(PawnKindDef creatureKind)
        {
            return AbsorbedCreatures.Contains(creatureKind);
        }

        public void AddActiveSummon(Pawn summonedCreature)
        {
            ActiveSummons.Add(summonedCreature);
        }

        public bool SummonIsActive(PawnKindDef creatureKind)
        {
            return ActiveSummons.Any(p => p.kindDef == creatureKind);
        }

        public bool HasSummonType(PawnKindDef creatureKind)
        {
            return AbsorbedCreatures.Contains(creatureKind);
        }

        public void RemoveSummon(PawnKindDef creatureKind)
        {
            AbsorbedCreatures.Remove(creatureKind);
        }

        public bool UnsummonCreature(Pawn creature)
        {
            return ActiveSummons.Remove(creature);
        }

        public bool HasActiveSummons()
        {
            return ActiveSummons.Count > 0;
        }
    }
}

//using RimWorld.Planet;
//using System.Collections.Generic;
//using Verse;

//namespace JJK
//{
//    public class AbsorbedCreatureManager : WorldComponent
//    {
//        private Dictionary<int, Summoner> summoners = new Dictionary<int, Summoner>();

//        public AbsorbedCreatureManager(World world) : base(world) { }

//        public Summoner GetOrCreateSummoner(Pawn pawn)
//        {
//            if (!summoners.TryGetValue(pawn.thingIDNumber, out Summoner summoner))
//            {
//                summoner = new Summoner(pawn);
//                summoners[pawn.thingIDNumber] = summoner;
//            }
//            return summoner;
//        }

//        public void AbsorbCreature(Pawn summoner, Pawn creature)
//        {
//            GetOrCreateSummoner(summoner).AbsorbCreature(creature);
//        }

//        public bool SummonCreature(Pawn summoner, Pawn creatureToSummon)
//        {
//            Summoner summonerData = GetOrCreateSummoner(summoner);
//            if (summonerData.SummonCreature(creatureToSummon))
//            {
//                IntVec3 spawnPosition = CellFinder.RandomClosewalkCellNear(summoner.Position, summoner.Map, 2);
//                Pawn spawnedCreature = GenSpawn.Spawn(creatureToSummon, spawnPosition, summoner.Map) as Pawn;

//                if (spawnedCreature != null)
//                {
//                    spawnedCreature.SetFaction(summoner.Faction);    
//                    return true;
//                }
//            }
//            return false;
//        }

//        public bool UnsummonCreature(Pawn summoner, Pawn creature)
//        {
//            Summoner summonerData = GetOrCreateSummoner(summoner);
//            if (summonerData != null && summonerData.UnsummonCreature(creature))
//            {
//                if (creature.Spawned)
//                {
//                    creature.DeSpawn(DestroyMode.Vanish);
//                }
//                return true;
//            }
//            return false;
//        }

//        public bool DeleteAbsorbedCreature(Pawn summoner, Pawn creature)
//        {
//            if (HasAbsorbedCreatue(summoner, creature))
//            {
//                summoners[summoner.thingIDNumber].RemoveSummon(creature);
//                return true;
//            }

//            return false;
//        }

//        public bool HasActiveSummons(Pawn summoner)
//        {
//            return GetOrCreateSummoner(summoner).HasActiveSummons();
//        }

//        public bool HasAbsorbedCreatue(Pawn summoner, Pawn Creature)
//        {
//            if (HasSummoner(summoner))
//            {
//                return summoners[summoner.thingIDNumber].HasSummonType(Creature);
//            }

//            return false;
//        }

//        public bool IsActiveAbsorbedCreature(Pawn summoner, Pawn Creature)
//        {
//            if (HasSummoner(summoner))
//            {
//                return summoners[summoner.thingIDNumber].SummonIsActive(Creature);
//            }

//            return false;
//        }

//        public bool HasSummoner(Pawn Summoner)
//        {
//            return summoners.ContainsKey(Summoner.thingIDNumber);
//        }

//        public List<Pawn> GetAbsorbedCreatures(Pawn summoner)
//        {
//            return new List<Pawn>(GetOrCreateSummoner(summoner).AbsorbedCreatures);
//        }

//        public HashSet<Pawn> GetActiveSummons(Pawn summoner)
//        {
//            return new HashSet<Pawn>(GetOrCreateSummoner(summoner).ActiveSummons);
//        }

//        public override void ExposeData()
//        {
//            Scribe_Collections.Look(ref summoners, "summoners", LookMode.Value, LookMode.Deep);
//        }

//        public override void FinalizeInit()
//        {
//            base.FinalizeInit();
//            CleanupDestroyedPawns();
//        }

//        private void CleanupDestroyedPawns()
//        {
//            List<int> summonersToRemove = new List<int>();

//            foreach (var kvp in summoners)
//            {
//                Summoner summoner = kvp.Value;
//                summoner.AbsorbedCreatures.RemoveAll(p => p == null || p.Destroyed);
//                summoner.ActiveSummons.RemoveWhere(p => p == null || p.Destroyed);

//                if (summoner.PawnReference == null || summoner.PawnReference.Destroyed)
//                {
//                    summonersToRemove.Add(kvp.Key);
//                }
//            }

//            foreach (int summonerID in summonersToRemove)
//            {
//                summoners.Remove(summonerID);
//            }
//        }
//    }


//    public class Summoner : IExposable
//    {
//        public Pawn PawnReference;
//        public List<Pawn> AbsorbedCreatures = new List<Pawn>();
//        public HashSet<Pawn> ActiveSummons = new HashSet<Pawn>();

//        public Summoner(Pawn pawn)
//        {
//            PawnReference = pawn;
//        }

//        public void ExposeData()
//        {
//            Scribe_References.Look(ref PawnReference, "pawnReference");
//            Scribe_Collections.Look(ref AbsorbedCreatures, "absorbedCreatures", LookMode.Reference);
//            Scribe_Collections.Look(ref ActiveSummons, "activeSummons", LookMode.Reference);
//        }

//        public void AbsorbCreature(Pawn creature)
//        {
//            if (!AbsorbedCreatures.Contains(creature))
//            {
//                AbsorbedCreatures.Add(creature);
//            }
//        }

//        public bool SummonCreature(Pawn creature)
//        {
//            if (AbsorbedCreatures.Contains(creature) && !ActiveSummons.Contains(creature))
//            {
//                ActiveSummons.Add(creature);
//                return true;
//            }
//            return false;
//        }

//        public bool SummonIsActive(Pawn Creature)
//        {
//            return ActiveSummons.Contains(Creature);
//        }

//        public bool HasSummonType(Pawn creature)
//        {
//            foreach (var item in AbsorbedCreatures)
//            {
//                if (item.def == creature.def)
//                {
//                    return true;
//                }
//            }

//            return false;
//        }


//        public void RemoveSummon(Pawn creature)
//        {
//            if (AbsorbedCreatures.Contains(creature))
//            {
//                AbsorbedCreatures.Remove(creature);
//            }
//        }

//        public bool UnsummonCreature(Pawn creature)
//        {
//            return ActiveSummons.Remove(creature);
//        }

//        public bool HasActiveSummons()
//        {
//            return ActiveSummons.Count > 0;
//        }
//    }
//}