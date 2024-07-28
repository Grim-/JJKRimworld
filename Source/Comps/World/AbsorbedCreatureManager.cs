using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using Verse;

using RimWorld.Planet;
using UnityEngine;

namespace JJK
{
    public class AbsorbedCreatureManager : WorldComponent
    {
        private Dictionary<string, PawnAbsorbption> Summoners = new Dictionary<string, PawnAbsorbption>();

        public AbsorbedCreatureManager(World world) : base(world) { }

        public PawnAbsorbption GetOrCreateSummoner(Pawn pawn)
        {
            if (!Summoners.TryGetValue(pawn.ThingID, out PawnAbsorbption summoner))
            {
                summoner = new PawnAbsorbption();
                summoner.SetPawnReference(pawn);
                Summoners[pawn.ThingID] = summoner;
            }
            return summoner;
        }

        public void AbsorbCreature(Pawn summoner, Pawn creature)
        {
            GetOrCreateSummoner(summoner).AbsorbCreature(creature.kindDef);
        }



        public Pawn GetSummonerFor(Pawn Creature)
        {
            foreach (var summoner in Summoners)
            {
                Pawn pawn = summoner.Value.GetActiveSummonOfKind(Creature.kindDef);
                if (pawn != null && pawn.ThingID == Creature.ThingID)
                {
                    return summoner.Value.PawnReference;
                }
            }

            return null;
        }


        public bool IsSummoner(Pawn Summoner)
        {
            return Summoners.ContainsKey(Summoner.ThingID);
        }



        public override void ExposeData()
        {
            base.ExposeData();

            if (Scribe.mode == LoadSaveMode.Saving)
            {
                List<string> keys = Summoners.Keys.ToList();
                List<PawnAbsorbption> values = Summoners.Values.ToList();
                Scribe_Collections.Look(ref keys, "SummonerKeys", LookMode.Value);
                Scribe_Collections.Look(ref values, "SummonerValues", LookMode.Deep);
            }
            else if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                List<string> keys = new List<string>();
                List<PawnAbsorbption> values = new List<PawnAbsorbption>();
                Scribe_Collections.Look(ref keys, "SummonerKeys", LookMode.Value);
                Scribe_Collections.Look(ref values, "SummonerValues", LookMode.Deep);

                Summoners = new Dictionary<string, PawnAbsorbption>();
                if (keys != null && values != null && keys.Count == values.Count)
                {
                    for (int i = 0; i < keys.Count; i++)
                    {
                        Summoners[keys[i]] = values[i];
                    }
                }
            }
        }



        public override void FinalizeInit()
        {
            base.FinalizeInit();
            LongEventHandler.ExecuteWhenFinished(ReconnectPawnReferences);
        }

        //maybe not so terribly good? But FinalizeInit is often called before the pawns are even loaded?
        private void ReconnectPawnReferences()
        {
            foreach (var kvp in Summoners.ToList())
            {
                string summonerId = kvp.Key;
                PawnAbsorbption summoner = kvp.Value;
                Pawn pawn = FindPawnByThingID(summonerId);
                if (pawn != null)
                {
                    summoner.SetPawnReference(pawn);
                    Log.Message($"Reconnected pawn with ID {summonerId} for Summoner.");
                }
            }
        }
        private Pawn FindPawnByThingID(string thingID)
        {
            Pawn pawn = Find.World.worldPawns.AllPawnsAlive.FirstOrDefault(p => p.ThingID == thingID);
            if (pawn != null) return pawn;

            // Check all maps
            foreach (Map map in Find.Maps)
            {
                pawn = map.mapPawns.AllPawns.FirstOrDefault(p => p.ThingID == thingID);
                if (pawn != null) return pawn;
            }

            // Check caravans
            foreach (Caravan caravan in Find.WorldObjects.Caravans)
            {
                pawn = caravan.pawns.InnerListForReading.FirstOrDefault(p => p.ThingID == thingID);
                if (pawn != null) return pawn;
            }

            return null;
        }
        private void CleanupDestroyedPawns()
        {
            foreach (var summoner in Summoners.Values)
            {
                summoner.ActiveSummons.RemoveWhere(p => p == null || p.Destroyed);
            }

            Summoners = Summoners.Where(kvp => kvp.Value.PawnReference != null && !kvp.Value.PawnReference.Destroyed)
                                  .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
    }
}
