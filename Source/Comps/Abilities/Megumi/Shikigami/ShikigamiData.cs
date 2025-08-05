using System.Collections.Generic;
using System.Linq;
using Verse;

namespace JJK
{
    public class ShikigamiData : IExposable
    {
        public ShikigamiDef KindDef;

        private Dictionary<PawnKindDef, string> storedPawnIdsByKind = new Dictionary<PawnKindDef, string>();

        private List<Pawn> activeShadows = new List<Pawn>();
        public List<Pawn> ActiveShadows => activeShadows;

        public bool IsPermanentlyDead = false;
        public int deathCooldownStartTick = -1;
        public bool IsOnDeathCooldown => deathCooldownStartTick > 0;

        public ShikigamiData()
        {
            storedPawnIdsByKind = new Dictionary<PawnKindDef, string>();
            activeShadows = new List<Pawn>();
            IsPermanentlyDead = false;
        }

        public ShikigamiData(ShikigamiDef kindDef) : this()
        {
            KindDef = kindDef;
        }

        public void StorePawn(Pawn pawn, ThingOwner<Pawn> container)
        {
            if (pawn == null || pawn.Destroyed)
                return;

            storedPawnIdsByKind[pawn.kindDef] = pawn.GetUniqueLoadID();

            if (pawn.TryGetComp(out Comp_TenShadowsSummon shadowsSummon))
            {
                shadowsSummon.OnUnSummon();
            }

            if (pawn.Spawned)
            {
                pawn.DeSpawn();
            }

            if (!container.Contains(pawn))
            {
                container.TryAdd(pawn, true);
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

        public void StoreActivePawns(ThingOwner<Pawn> container)
        {
            foreach (var item in activeShadows.ToList())
            {
                if (item.Destroyed)
                {
                    continue;
                }

                StorePawn(item, container);
            }

            activeShadows.Clear();
        }

        public bool HasStoredPawnOfKind(PawnKindDef kindDef)
        {
            return storedPawnIdsByKind.ContainsKey(kindDef);
        }

        public Pawn GetStoredPawnOfKind(PawnKindDef def, ThingOwner<Pawn> container)
        {
            if (storedPawnIdsByKind.ContainsKey(def))
            {
                string pawnId = storedPawnIdsByKind[def];
                Pawn pawn = container.InnerListForReading.FirstOrDefault(p => p.GetUniqueLoadID() == pawnId);

                if (pawn != null && !pawn.Destroyed)
                {
                    container.Remove(pawn);
                    storedPawnIdsByKind.Remove(def);

                    if (!activeShadows.Contains(pawn))
                    {
                        activeShadows.Add(pawn);
                    }
                    return pawn;
                }
            }
            return null;
        }

        public bool ContainsPawn(Pawn pawn)
        {
            if (activeShadows.Contains(pawn))
                return true;

            string pawnId = pawn.GetUniqueLoadID();
            return storedPawnIdsByKind.ContainsValue(pawnId);
        }

        public void ExposeData()
        {
            Scribe_Defs.Look(ref KindDef, "kindDef");

            if (Scribe.mode == LoadSaveMode.Saving)
            {
                var tempDict = new Dictionary<PawnKindDef, string>(storedPawnIdsByKind);
                Scribe_Collections.Look(ref tempDict, "storedPawnIdsByKind", LookMode.Def, LookMode.Value);
            }
            else
            {
                Scribe_Collections.Look(ref storedPawnIdsByKind, "storedPawnIdsByKind", LookMode.Def, LookMode.Value);
            }

            Scribe_Collections.Look(ref activeShadows, "activePawns", LookMode.Reference);
            Scribe_Values.Look(ref IsPermanentlyDead, "isPermanentlyDead", false);
            Scribe_Values.Look(ref deathCooldownStartTick, "deathCooldownStartTick", -1);

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (storedPawnIdsByKind == null)
                {
                    storedPawnIdsByKind = new Dictionary<PawnKindDef, string>();
                }
                if (activeShadows == null)
                {
                    activeShadows = new List<Pawn>();
                }
            }
        }
    }
}