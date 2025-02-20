﻿using System.Collections.Generic;
using System.Linq;
using Verse;

namespace JJK
{
    public class ShikigamiData : IExposable
    {
        public ShikigamiDef KindDef;

        //any already created but not currently summoned onto a map pawns
        private Dictionary<PawnKindDef, Pawn> storedPawnsByKind = new Dictionary<PawnKindDef, Pawn>();
        public List<Pawn> StoredPawns => storedPawnsByKind.Values.ToList();


        // Active pawns currently summoned on map
        private List<Pawn> activeShadows = new List<Pawn>();
        public List<Pawn> ActiveShadows => activeShadows;

        public bool IsPermanentlyDead = false;
        public int deathCooldownStartTick = -1;
        public bool IsOnDeathCooldown => deathCooldownStartTick > 0;

        public ShikigamiData()
        {
            storedPawnsByKind = new Dictionary<PawnKindDef, Pawn>();
            activeShadows = new List<Pawn>();
            IsPermanentlyDead = false;
        }

        public ShikigamiData(ShikigamiDef kindDef) : this()
        {
            KindDef = kindDef;
        }

        public void StorePawn(Pawn pawn)
        {
            if (pawn == null || pawn.Destroyed) 
                return;

            if (storedPawnsByKind.ContainsKey(pawn.kindDef))
            {
                storedPawnsByKind[pawn.kindDef] = pawn;
            }
            else
            {
                storedPawnsByKind.Add(pawn.kindDef, pawn);
            }
            if (pawn.TryGetComp(out Comp_TenShadowsSummon shadowsSummon))
            {
                shadowsSummon.OnUnSummon();
            }

            if (!Find.WorldPawns.Contains(pawn))
            {
                if (pawn.Spawned)
                {
                    pawn.DeSpawn();
                }

                Find.WorldPawns.PassToWorld(pawn, RimWorld.Planet.PawnDiscardDecideMode.KeepForever);
            }
            activeShadows.Remove(pawn);
        }

        public void AddActivePawn(Pawn Pawn)
        {
            if (!ActiveShadows.Contains(Pawn))
            {
                ActiveShadows.Add(Pawn);
            }
        }

        public void StoreActivePawns()
        {
            foreach (var item in activeShadows.ToList())
            {
                if (item.Destroyed)
                {
                    continue;
                }

                StorePawn(item);
            }

            activeShadows.Clear();
        }

        public bool HasStoredPawnOfKind(PawnKindDef kindDef)
        {
            return storedPawnsByKind.ContainsKey(kindDef) && storedPawnsByKind[kindDef] != null;
        }

        public Pawn GetStoredPawnOfKind(PawnKindDef def)
        {
            if (storedPawnsByKind.ContainsKey(def))
            {
                var pawn = storedPawnsByKind[def];
                if (pawn != null && !pawn.Destroyed)
                {
                    storedPawnsByKind.Remove(def);
                    if (!activeShadows.Contains(pawn))
                    {
                        activeShadows.Add(pawn);
                    }
                    return pawn;
                }
            }
            return null;
        }

        public void ExposeData()
        {
            Scribe_Defs.Look(ref KindDef, "kindDef");

            List<PawnKindDef> storedPawnKeys = null;
            List<Pawn> storedPawnValues = null;
 
            if (Scribe.mode == LoadSaveMode.Saving && storedPawnsByKind != null)
            {
                storedPawnKeys = storedPawnsByKind.Keys.ToList();
                storedPawnValues = storedPawnsByKind.Values.ToList();
            }

            Scribe_Collections.Look(ref storedPawnKeys, "storedPawnKeys", LookMode.Def);
            Scribe_Collections.Look(ref storedPawnValues, "storedPawnValues", LookMode.Reference);

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (storedPawnKeys != null && storedPawnValues != null &&
                    storedPawnKeys.Count == storedPawnValues.Count)
                {
                    storedPawnsByKind = new Dictionary<PawnKindDef, Pawn>();
                    for (int i = 0; i < storedPawnKeys.Count; i++)
                    {
                        if (storedPawnKeys[i] != null && storedPawnValues[i] != null)
                        {
                            storedPawnsByKind[storedPawnKeys[i]] = storedPawnValues[i];
                        }
                    }
                }
                else
                {
                    storedPawnsByKind = new Dictionary<PawnKindDef, Pawn>();
                }
            }

            Scribe_Collections.Look(ref activeShadows, "activePawns", LookMode.Reference);
            Scribe_Values.Look(ref IsPermanentlyDead, "isPermanentlyDead", false);
            Scribe_Values.Look(ref deathCooldownStartTick, "deathCooldownStartTick", -1);
        }
    }
}


