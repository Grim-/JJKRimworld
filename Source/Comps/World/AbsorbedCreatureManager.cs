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
        private Dictionary<Pawn, AbsorbedData> AbsorbData = new Dictionary<Pawn, AbsorbedData>();
        private List<Pawn> List;
        private List<AbsorbedData> List2;
        public AbsorbedCreatureManager(World world) : base(world) { }

        public AbsorbedData GetAbsorbDataForPawn(Pawn pawn)
        {
            if (AbsorbData.TryGetValue(pawn, out AbsorbedData data))
            {
                return data;
            }

            return null;
        }

        public AbsorbedData CreateAbsorbDataForPawn(Pawn pawn)
        {
            AbsorbedData NewData = new AbsorbedData();
            NewData.Master = pawn;
            AbsorbData[pawn] = NewData;
            return NewData;
        }

        public bool HasAbsorbDataForPawn(Pawn pawn)
        {
            return AbsorbData.ContainsKey(pawn);
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Collections.Look<Pawn, AbsorbedData>(ref AbsorbData, "absorbedCreatureManagerData", LookMode.Reference, LookMode.Deep, ref List, ref List2);

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                foreach (var item in AbsorbData)
                {
                    Log.Message(item.Key);
                    Log.Message(item.Value.Master);
                    Log.Message(item.Value.ActiveSummons.Count);
                    foreach (var smn in item.Value.ActiveSummons)
                    {
                        Log.Message(smn);
                        Log.Message(smn.Summon);
                        Log.Message(smn.Master);
                    }
                }
            }
            //Scribe_Collections.Look(ref AbsorbData, "absorbedCreatureManagerData", LookMode.Reference, LookMode.Deep);
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

