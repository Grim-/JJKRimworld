using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using Verse;

using RimWorld.Planet;
using UnityEngine;
using System;

namespace JJK
{
    public class AbsorbedCreatureManager : WorldComponent
    {
        private Dictionary<string, AbsorbedData> AbsorbData = new Dictionary<string, AbsorbedData>();

        public AbsorbedCreatureManager(World world) : base(world) { }

        public AbsorbedData GetAbsorbDataForPawn(Pawn pawn)
        {
            if (AbsorbData.TryGetValue(pawn.ThingID, out AbsorbedData data))
            {
                return data;
            }
            else
            {
                AbsorbedData NewData = new AbsorbedData();
                NewData.SetPawnReference(pawn);
                AbsorbData[pawn.ThingID] = NewData;
                return NewData;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                List<string> keys = AbsorbData.Keys.ToList();
                List<AbsorbedData> values = AbsorbData.Values.ToList();
                Scribe_Collections.Look(ref keys, "absorbedCreatureManagerKeys", LookMode.Value);
                Scribe_Collections.Look(ref values, "absorbedCreatureManagerValues", LookMode.Deep);
                Log.Message($"JJK: Saving AbsorbedCreatureManager data. Keys: {keys.Count}, Values: {values.Count}");
            }
            else if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                List<string> keys = new List<string>();
                List<AbsorbedData> values = new List<AbsorbedData>();
                Scribe_Collections.Look(ref keys, "absorbedCreatureManagerKeys", LookMode.Value);
                Scribe_Collections.Look(ref values, "absorbedCreatureManagerValues", LookMode.Deep);
                AbsorbData = new Dictionary<string, AbsorbedData>();
                if (keys != null && values != null && keys.Count == values.Count)
                {
                    for (int i = 0; i < keys.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(keys[i]))
                        {
                            AbsorbData[keys[i]] = values[i];
                        }
                    }
                }
                else
                {
                    Log.Error("JJK: Failed to load AbsorbedCreatureManager data. Keys or values are null or mismatched.");
                }
            }     
            else if(Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                ResolveCrossReferences();
            }
        }

        public override void FinalizeInit()
        {
            base.FinalizeInit();
        
        }

        private void ResolveCrossReferences()
        {
            Log.Message($"JJK: AbsorbedCreatureManager ResolveCrossReferences");
            foreach (var item in AbsorbData)
            {
                item.Value.ResolveCrossReferences();
            }    
        }

        public Pawn GetMasterForAbsorbedCreature(Pawn summon)
        {
            foreach (var data in AbsorbData)
            {
                foreach (var activeSummon in data.Value.ActiveSummons)
                {
                    if (activeSummon.Summon == null)
                    {
                        continue;
                    }

                    if (activeSummon.Summon.ThingID == summon.ThingID)
                    {
                        return activeSummon.Master;
                    }
                }
            }
            return null;
        }
    }
}

