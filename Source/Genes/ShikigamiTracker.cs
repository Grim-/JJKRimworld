using System.Collections.Generic;
using Verse;

namespace JJK
{
    public class ShikigamiTracker : IThingHolder, IExposable
    {
        protected TenShadowGene Gene;
        private ThingOwner<Pawn> storedShikigamiPawns;
        private Dictionary<ShikigamiDef, ShikigamiData> shikigamiData = new Dictionary<ShikigamiDef, ShikigamiData>();

        public Dictionary<ShikigamiDef, ShikigamiData> ShikigamiData => shikigamiData;

        public ShikigamiTracker()
        {
            storedShikigamiPawns = new ThingOwner<Pawn>(this, false, LookMode.Deep);
            shikigamiData = new Dictionary<ShikigamiDef, ShikigamiData>();
        }

        public ShikigamiTracker(TenShadowGene gene) : this()
        {
            Gene = gene;
        }

        public IThingHolder ParentHolder => Gene;

        public void ExposeData()
        {
            Scribe_Deep.Look(ref storedShikigamiPawns, "storedShikigamiPawns", this);
            Scribe_Collections.Look(ref shikigamiData, "shikigamiData", LookMode.Def, LookMode.Deep);

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (storedShikigamiPawns == null)
                {
                    storedShikigamiPawns = new ThingOwner<Pawn>(this, false, LookMode.Deep);
                }
                if (shikigamiData == null)
                {
                    shikigamiData = new Dictionary<ShikigamiDef, ShikigamiData>();
                }
            }
        }

        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
        }

        public ThingOwner GetDirectlyHeldThings()
        {
            return storedShikigamiPawns;
        }

        public bool HasShikigamiKind(ShikigamiDef kindDef)
        {
            return shikigamiData.ContainsKey(kindDef);
        }

        public ShikigamiData GetShikigamiData(ShikigamiDef kindDef)
        {
            if (HasShikigamiKind(kindDef))
            {
                return shikigamiData[kindDef];
            }
            return null;
        }

        public ShikigamiData AddNewShikigamiData(ShikigamiDef kindDef)
        {
            ShikigamiData data = new ShikigamiData(kindDef);
            shikigamiData.Add(kindDef, data);
            return data;
        }

        public void StorePawn(ShikigamiDef kindDef, Pawn pawn)
        {
            if (!HasShikigamiKind(kindDef))
                return;

            shikigamiData[kindDef].StorePawn(pawn, storedShikigamiPawns);
        }

        public Pawn GetStoredPawn(ShikigamiDef kindDef, PawnKindDef pawnKind)
        {
            if (!HasShikigamiKind(kindDef))
                return null;

            return shikigamiData[kindDef].GetStoredPawnOfKind(pawnKind, storedShikigamiPawns);
        }

        public void StoreAllActivePawns(ShikigamiDef kindDef)
        {
            if (!HasShikigamiKind(kindDef))
                return;

            shikigamiData[kindDef].StoreActivePawns(storedShikigamiPawns);
        }

        public List<Pawn> GetActiveSummonsOfKind(ShikigamiDef kindDef)
        {
            if (HasShikigamiKind(kindDef))
            {
                return shikigamiData[kindDef].ActiveShadows;
            }
            return null;
        }

        public ShikigamiDef GetShikigamiDefForPawn(Pawn pawn)
        {
            foreach (var kvp in shikigamiData)
            {
                if (kvp.Value.ContainsPawn(pawn))
                {
                    return kvp.Key;
                }
            }
            return null;
        }
    }
}