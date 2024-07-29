using System.Collections.Generic;
using System.Linq;
using Verse;

namespace JJK
{
    public class AbsorbedData : IExposable
    {
        public string PawnID;
        public Pawn Master => JJKUtility.FindPawnById(PawnID);
        public List<PawnKindDef> AbsorbedCreatures = new List<PawnKindDef>();
        public List<ActiveSummonData> ActiveSummons = new List<ActiveSummonData>();
        public int SummonLimit = 5;

        public AbsorbedData()
        {
        }

        public bool IsActiveSummon(Pawn creature)
        {
            foreach (var item in ActiveSummons)
            {
                if (item.Summon.ThingID == creature.ThingID)
                {
                    return true;
                }

            }

            return false;
        }

        public void SetPawnReference(Pawn pawn)
        {
            PawnID = pawn.ThingID;
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref PawnID, "PawnID");
           // Scribe_References.Look(ref Master, "Master");
            Scribe_Collections.Look(ref AbsorbedCreatures, "AbsorbedCreatures", LookMode.Def);
            Scribe_Collections.Look(ref ActiveSummons, "ActiveSummons", LookMode.Deep);
            Scribe_Values.Look(ref SummonLimit, "SummonLimit", 5);
        }
        public void ResolveCrossReferences()
        {
            var pawn = Find.WorldPawns.AllPawnsAlive.FirstOrDefault(p => p.ThingID == PawnID) ??
                     Find.CurrentMap?.mapPawns.AllPawns.FirstOrDefault(p => p.ThingID == PawnID);
            Log.Message($"JJK: AResolveCrossReferences Master {Master}");

            //SetPawnReference(pawn);

            foreach (var summon in ActiveSummons)
            {
                summon.ResolveCrossReferences();
            }
        }
        public void AbsorbCreature(PawnKindDef creatureKind)
        {
            if (!AbsorbedCreatures.Contains(creatureKind))
            {
                AbsorbedCreatures.Add(creatureKind);
                Log.Message($"{Master.LabelShort} absorbed {creatureKind.defName}");
            }
        }

        public bool CreateCreatureOfKind(PawnKindDef creatureKind)
        {
            if (AbsorbedCreatures.Contains(creatureKind))
            {
                IntVec3 spawnPosition = CellFinder.RandomClosewalkCellNear(Master.Position, Master.Map, 2);
                Pawn spawnedCreature = PawnGenerator.GeneratePawn(creatureKind, RimWorld.Faction.OfPlayer);
                spawnedCreature.health.AddHediff(JJKDefOf.JJ_SummonedCreatureTag);
                GenSpawn.Spawn(spawnedCreature, spawnPosition, Master.Map);
                AddToActiveSummons(spawnedCreature);
                Log.Message($"{Master.LabelShort} summoned {spawnedCreature.LabelShort} (ThingID: {spawnedCreature.ThingID})");
                return true;
            }
            return false;
        }

        private PawnKindDef GetDef(string DefName)
        {
            return DefDatabase<PawnKindDef>.GetNamed(DefName);
        }

        public bool UnsummonCreature(Pawn creature)
        {
            if (IsActiveSummon(creature))
            {
                foreach (var item in ActiveSummons.ToList())
                {
                    if (item.Summon.ThingID == creature.ThingID)
                    {
                        Log.Message($"JJK: {creature.Label} {creature.ThingID} FOUND DESTROYING AND REMOVING FROM ACTIVE SUMMONS.");
                        
                        item.Summon.DeSpawn(DestroyMode.Vanish);
                        ActiveSummons.Remove(item);
                        return true;
                    }
                }
            }
            return false;
        }

        public void AddToActiveSummons(Pawn summonedCreature)
        {
            ActiveSummons.Add(new ActiveSummonData
            {
                Summon = summonedCreature,
                Master = Master,
                Def = summonedCreature.kindDef
            });
            Log.Message($"JJK: Added {summonedCreature.LabelShort} (ThingID: {summonedCreature.ThingID}, KindDef: {summonedCreature.kindDef}) to active summons of {Master.LabelShort}");
            Log.Message($"JJK: Current active summons count: {ActiveSummons.Count}");
        }

        public bool SummonIsActiveOfKind(PawnKindDef creature)
        {
            foreach (var item in ActiveSummons)
            {
                if (item.Summon.kindDef == creature)
                {
                    return true;
                }
            }

            return false;
        }

        public Pawn GetActiveSummonOfKind(PawnKindDef creature)
        {
            foreach (var item in ActiveSummons)
            {
                if (item.Summon.kindDef == creature)
                {
                    return item.Summon;
                }
            }

            return null;
        }

        public bool DeleteAbsorbedCreature(PawnKindDef creature)
        {
            if (HasAbsorbedCreatureKind(creature))
            {
                RemoveAbsorbedSummonType(creature);
                var summonToRemove = ActiveSummons.FirstOrDefault(s => s.Def == creature);
                if (summonToRemove != null)
                {
                    summonToRemove.Summon.DeSpawn(DestroyMode.Vanish);
                    ActiveSummons.Remove(summonToRemove);
                }
                return true;
            }
            return false;
        }

        public bool HasAbsorbedCreatureKind(PawnKindDef creatureKind)
        {
            return AbsorbedCreatures.Contains(creatureKind);
        }

        public void RemoveAbsorbedSummonType(PawnKindDef creatureKind)
        {
            AbsorbedCreatures.Remove(creatureKind);
        }

        public bool CanAbsorbNewSummon()
        {
            return AbsorbedCreatures.Count < SummonLimit;
        }
    }

    public class ActiveSummonData : IExposable
    {
        public Pawn Master;
        public Pawn Summon;

        public Def Def;

        public void ExposeData()
        {
            Scribe_References.Look(ref Master, "MasterID");
            Scribe_References.Look(ref Summon, "SummonID");
            Scribe_Defs.Look(ref Def, "Def");
        }

        public void ResolveCrossReferences()
        {
            Log.Message($"JJK: Resolving cross-references for ActiveSummonData. " +
                        $"Master found: {Master != null}, " +
                        $"Summon found: {Summon != null}, " +
                        $"Def: {Def?.defName ?? "null"}");
        }


        //private Pawn FindPawnById(string thingId)
        //{
        //    return Find.WorldPawns.AllPawnsAlive.FirstOrDefault(p => p.ThingID == thingId) ??
        //           Find.CurrentMap?.mapPawns.AllPawns.FirstOrDefault(p => p.ThingID == thingId);
        //}

    }
}