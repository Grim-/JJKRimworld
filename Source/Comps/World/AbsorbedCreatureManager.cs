//using RimWorld;
//using RimWorld.Planet;
//using System.Collections.Generic;
//using System.Linq;
//using Verse;

//using RimWorld.Planet;
//using UnityEngine;
//using System;

//namespace JJK
//{
//    public class AbsorbedCreatureManager : GameComponent
//    {
//        public Dictionary<string, AbsorbedData> AbsorbData = new Dictionary<string, AbsorbedData>();
//        private List<string> tempKeys = null;
//        private List<AbsorbedData> tempValues = null;
//        public AbsorbedCreatureManager(Game game) : base()
//        {
//            Log.Message("JJK: AbsorbedCreatureManager GameComponent initialized");
//        }

//        public AbsorbedData GetAbsorbDataForPawn(Pawn pawn)
//        {
//            string key = pawn.ThingID;
//            //Log.Message($"JJK: GetAbsorbDataForPawn called for pawn {key}");
//            //Log.Message($"JJK: AbsorbData count: {AbsorbData.Count}");

//            if (AbsorbData.TryGetValue(key, out AbsorbedData data))
//            {
//                //Log.Message($"JJK: Found data for pawn {key}");
//                return data;
//            }

//            //Log.Message($"JJK: No data found for pawn {key}");
//            return null;
//        }
//        public override void FinalizeInit()
//        {
//            base.FinalizeInit();
//            Log.Message($"JJK: FinalizeInit called. AbsorbData count: {AbsorbData.Count}");
//        }
//        public AbsorbedData CreateAbsorbDataForPawn(Pawn pawn)
//        {
//            string key = pawn.ThingID;
//            AbsorbedData NewData = new AbsorbedData();
//            NewData.Master = pawn;
//            AbsorbData[key] = NewData;
//            Log.Message($"JJK: Created new AbsorbedData for pawn {key}");
//            return NewData;
//        }

//        public bool HasAbsorbDataForPawn(Pawn pawn)
//        {
//            return AbsorbData.ContainsKey(pawn.ThingID);
//        }

//        public override void ExposeData()
//        {
//            base.ExposeData();
//            Log.Message($"JJK: ExposeData called. Mode: {Scribe.mode}");

//            //if (Scribe.mode == LoadSaveMode.Saving)
//            //{
//            //    tempKeys = AbsorbData.Keys.ToList();
//            //    tempValues = AbsorbData.Values.ToList();
//            //}
//            Scribe_Collections.Look(ref AbsorbData, "absorbedCreatureManagerData", LookMode.Value, LookMode.Deep, ref tempKeys, ref tempValues);
//            //Scribe_Collections.Look(ref tempKeys, "keys", LookMode.Value) ;
//            //Scribe_Collections.Look(ref tempValues, "values", LookMode.Deep);

//            //if (Scribe.mode == LoadSaveMode.PostLoadInit)
//            //{
//            //    foreach (var data in AbsorbData.Values)
//            //    {
//            //        data.ActiveSummons.RemoveAll(s => s.Summon == null || s.Master == null);
//            //    }
//            //}
//            Log.Message($"JJK: ExposeData finished. Keys: {AbsorbData.Keys.Count}, Values: {AbsorbData.Values.Count}");
//        }
//        public override void LoadedGame()
//        {
//            base.LoadedGame();
//            Log.Message("JJK: LoadedGame called");
//           // ReconstructDictionary();
//        }
//        //private void ReconstructDictionary()
//        //{
//        //    Log.Message($"JJK: Reconstructing dictionary. Keys: {tempKeys?.Count}, Values: {tempValues?.Count}");
//        //    AbsorbData = new Dictionary<string, AbsorbedData>();

//        //    if (tempKeys != null && tempValues != null && tempKeys.Count == tempValues.Count)
//        //    {
//        //        for (int i = 0; i < tempKeys.Count; i++)
//        //        {
//        //            AbsorbData[tempKeys[i]] = tempValues[i];
//        //            Log.Message($"JJK: Reconstructed entry for {tempKeys[i]}");
//        //        }
//        //    }
//        //    else
//        //    {
//        //        Log.Error("JJK: Failed to reconstruct AbsorbData dictionary. Keys or values are null or counts don't match.");
//        //    }

//        //    Log.Message($"JJK: Dictionary reconstruction complete. AbsorbData count: {AbsorbData.Count}");
//        //}
//        public override void StartedNewGame()
//        {
//            base.StartedNewGame();
//            Log.Message("JJK: StartedNewGame called");
//            //AbsorbData = new Dictionary<string, AbsorbedData>();
//        }
//        public Pawn GetMasterForAbsorbedCreature(Pawn summon)
//        {
//            foreach (var data in AbsorbData.Values)
//            {
//                if (data.ActiveSummons.Any(s => s.Summon == summon))
//                {
//                    return data.Master;
//                }
//            }
//            return null;
//        }
//    }
//    //public class AbsorbedCreatureManager : GameComponent
//    //{
//    //    private Dictionary<Pawn, AbsorbedData> AbsorbData = new Dictionary<Pawn, AbsorbedData>();
//    //    private List<Pawn> List;
//    //    private List<AbsorbedData> List2;
//    //    public AbsorbedCreatureManager(Game game) : base()
//    //    {
//    //        Log.Message("JJK: AbsorbedCreatureManager GameComponent initialized");
//    //    }

//    //    public AbsorbedData GetAbsorbDataForPawn(Pawn pawn)
//    //    {
//    //        if (pawn == null)
//    //        {
//    //            Log.Error("JJK: GetAbsorbDataForPawn called with null pawn");
//    //            return null;
//    //        }

//    //        Log.Message($"JJK: GetAbsorbDataForPawn called for pawn {pawn.ThingID}");
//    //        Log.Message($"JJK: AbsorbData count: {AbsorbData.Count}");

//    //        if (AbsorbData.TryGetValue(pawn, out AbsorbedData data))
//    //        {
//    //            Log.Message($"JJK: Found data for pawn {pawn.ThingID}");
//    //            return data;
//    //        }

//    //        Log.Message($"JJK: No data found for pawn {pawn.ThingID}");
//    //        return null;
//    //    }
//    //    public override void FinalizeInit()
//    //    {
//    //        base.FinalizeInit();
//    //        Log.Message($"JJK: FinalizeInit called. AbsorbData count: {AbsorbData.Count}");
//    //    }
//    //    public AbsorbedData CreateAbsorbDataForPawn(Pawn pawn)
//    //    {
//    //        AbsorbedData NewData = new AbsorbedData();
//    //        NewData.Master = pawn;
//    //        AbsorbData[pawn] = NewData;
//    //        return NewData;
//    //    }

//    //    public bool HasAbsorbDataForPawn(Pawn pawn)
//    //    {
//    //        return AbsorbData.ContainsKey(pawn);
//    //    }

//    //    public override void ExposeData()
//    //    {
//    //        base.ExposeData();

//    //        Log.Message($"JJK: ExposeData called. Mode: {Scribe.mode}");
//    //        Log.Message($"JJK: AbsorbData count before: {AbsorbData.Count}");

//    //        Scribe_Collections.Look(ref AbsorbData, "absorbedCreatureManagerData", LookMode.Reference, LookMode.Deep, ref List, ref List2);

//    //        Log.Message($"JJK: AbsorbData count after: {AbsorbData.Count}");

//    //        if (Scribe.mode == LoadSaveMode.PostLoadInit)
//    //        {
//    //            Log.Message("JJK: PostLoadInit - Checking AbsorbData");
//    //            foreach (var kvp in AbsorbData)
//    //            {
//    //                Log.Message($"JJK: Key: {kvp.Key?.ThingID}, Value: {kvp.Value?.Master?.ThingID}");
//    //            }
//    //        }
//    //    }

//    //    public Pawn GetMasterForAbsorbedCreature(Pawn summon)
//    //    {
//    //        foreach (var data in AbsorbData)
//    //        {
//    //            foreach (var activeSummon in data.Value.ActiveSummons)
//    //            {
//    //                if (activeSummon.Summon == null)
//    //                {
//    //                    continue;
//    //                }

//    //                if (activeSummon.Summon.thingIDNumber == summon.thingIDNumber)
//    //                {
//    //                    return activeSummon.Master;
//    //                }
//    //            }
//    //        }
//    //        return null;
//    //    }
//    //}
//}

