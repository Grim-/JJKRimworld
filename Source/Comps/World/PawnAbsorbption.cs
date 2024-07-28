using System.Collections.Generic;
using System.Linq;
using Verse;

namespace JJK
{
    public class PawnAbsorbption : IExposable
    {
        public string PawnID;
        public Pawn PawnReference;
        public HashSet<PawnKindDef> AbsorbedCreatures = new HashSet<PawnKindDef>();
        public HashSet<Pawn> ActiveSummons = new HashSet<Pawn>();
        public int SummonLimit = 5;

        public PawnAbsorbption()
        {

        }

        public void SetPawnReference(Pawn pawn)
        {
            PawnReference = pawn;
            PawnID = pawn.ThingID;
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref PawnID, "PawnID");
            Scribe_Collections.Look(ref AbsorbedCreatures, "AbsorbedCreatures", LookMode.Def);
            Scribe_References.Look(ref PawnReference, "PawnReference");
            Scribe_Collections.Look(ref ActiveSummons, "ActiveSummons", LookMode.Reference);
            Scribe_Values.Look(ref SummonLimit, "SummonLimit", 5);
        }
        public void AbsorbCreature(PawnKindDef creatureKind)
        {
            if (!AbsorbedCreatures.Contains(creatureKind))
            {
                AbsorbedCreatures.Add(creatureKind);
                Log.Message($"Storing creature def: {creatureKind.defName}");
            }
        }

        public bool SummonCreature(PawnKindDef creatureKind)
        {
            if (AbsorbedCreatures.Contains(creatureKind))
            {
                // Check if a creature of this kind is already summoned
                if (ActiveSummons.Any(p => p.kindDef == creatureKind))
                {
                    return false; // Already summoned
                }

                IntVec3 spawnPosition = CellFinder.RandomClosewalkCellNear(PawnReference.Position, PawnReference.Map, 2);
                Pawn spawnedCreature = PawnGenerator.GeneratePawn(creatureKind, PawnReference.Faction);
                spawnedCreature.health.AddHediff(JJKDefOf.JJ_SummonedCreatureTag);
                spawnedCreature.SetFaction(PawnReference.Faction);


                GenSpawn.Spawn(spawnedCreature, spawnPosition, PawnReference.Map);
                AddToActiveSummons(spawnedCreature);
                return true;
            }

            return false;
        }

        public bool UnsummonCreature(PawnKindDef creature)
        {
            Pawn toRemove = null;
            foreach (var item in ActiveSummons)
            {
                if (item.kindDef == creature)
                {
                    toRemove = item;
                    break;
                }
            }

            if (toRemove != null)
            {
                ActiveSummons.Remove(toRemove);
                toRemove.DeSpawn(DestroyMode.Vanish);
                return true;
            }

            return false;
        }

        public void AddToActiveSummons(Pawn summonedCreature)
        {
            ActiveSummons.Add(summonedCreature);
        }

        public bool SummonIsActiveOfKind(PawnKindDef creature)
        {
            foreach (var item in ActiveSummons)
            {
                if (item.kindDef == creature)
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
                if (item.kindDef == creature)
                {
                    return item;
                }
            }
            return null;
        }

        public bool DeleteAbsorbedCreature(PawnKindDef creature)
        {
            if (HasSummonType(creature))
            {
                RemoveAbsorbedSummonType(creature);
                if (SummonIsActiveOfKind(creature))
                {
                    GetActiveSummonOfKind(creature).DeSpawn(DestroyMode.Vanish);
                }

                return true;
            }
            return false;
        }


        public bool HasSummonType(PawnKindDef creatureKind)
        {
            return AbsorbedCreatures.Contains(creatureKind);
        }

        public void RemoveAbsorbedSummonType(PawnKindDef creatureKind)
        {
            AbsorbedCreatures.Remove(creatureKind);
        }
        public bool CanAbsorbNewSummon()
        {
            return ActiveSummons.Count < SummonLimit;
        }

        public bool HasActiveSummons()
        {
            return ActiveSummons.Count > 0;
        }
    }
}
