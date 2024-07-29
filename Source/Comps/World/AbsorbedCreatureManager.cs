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
                NewData.PawnID = pawn.ThingID;
                AbsorbData[pawn.ThingID] = NewData;
                return NewData;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Collections.Look(ref AbsorbData, "absorbedCreatureManagerData", LookMode.Value, LookMode.Deep);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                ResolveCrossReferences();
            }
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

