using System.Collections.Generic;
using System.Linq;
using Verse;

namespace JJK
{
    public class AbsorbedData : IExposable
    {
        public Pawn Master;
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

         public void ExposeData()
        {
            Scribe_References.Look(ref Master, "Master");
            Scribe_Collections.Look(ref AbsorbedCreatures, "AbsorbedCreatures", LookMode.Def);
            Scribe_Collections.Look(ref ActiveSummons, "ActiveSummons", LookMode.Deep);
            Scribe_Values.Look(ref SummonLimit, "SummonLimit", 5);
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
                GenSpawn.Spawn(spawnedCreature, spawnPosition, Master.Map);
                AddToActiveSummons(spawnedCreature);
                Log.Message($"JJK: {Master.LabelShort} summoned {spawnedCreature.LabelShort} (ThingID: {spawnedCreature.ThingID})");
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
                        
                        item.Summon.Destroy(DestroyMode.Vanish);
                        //ActiveSummons.Remove(item);
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

        public PawnKindDef Def;

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
    }
}