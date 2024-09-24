using System.Collections.Generic;
using Verse;

namespace JJK
{
    public class Hediff_TenShadowsUser : Hediff
    {
        private Dictionary<PawnKindDef, ShikigamiData> Shikigami = new Dictionary<PawnKindDef, ShikigamiData>();
        public bool ShouldSummonTotalityDivineDog { get; private set; }

        public bool CanSummonShikigamiKind(PawnKindDef KindDef)
        {
            if (HasShikigamiKind(KindDef) && !IsShikigamiPermanentlyDead(KindDef))
            {
                return true;
            }

            return false;
        }


        public bool IsShikigamiPermanentlyDead(PawnKindDef KindDef)
        {
            return HasShikigamiKind(KindDef) && Shikigami[KindDef].IsPermanentlyDead;
        }

        public bool HasShikigamiKind(PawnKindDef KindDef)
        {
            return Shikigami.ContainsKey(KindDef);
        }



        public void AddShikigamiToAvailble(PawnKindDef KindDef)
        {
            if (!HasShikigamiKind(KindDef))
            {
                Shikigami.Add(KindDef, new ShikigamiData(KindDef));
            }
        }



        public Pawn SummonShikigami(PawnKindDef KindDef, IntVec3 Position, Map Map)
        {
            Log.Message($"Attempting to summon a {KindDef.label} shikigami for {pawn.Label}.");

            Pawn Pawn = JJKUtility.SpawnShikigami(KindDef, pawn, Map, Position);
            if (Pawn.TryGetComp(out Comp_OnDeathHandler compOnDeath))
            {
                Log.Message($"Registering OnDeath Handler for {Pawn.Label} (shikigami)");
                compOnDeath.OnDeath += OnShikigamiDeath;
            }
            return Pawn;
        }


        public void UnsummonShikigami(PawnKindDef KindDef)
        {
            if (KindDef == null)
            {
                Log.Error($"{pawn.Label} pawnkinddef was null while trying to unsummon a shikigami.");
                return;
            }

            if (HasShikigamiKind(KindDef))
            {
                List<Pawn> ActiveSummonsOfKind = GetActiveSummonsOfKind(KindDef);

                foreach (var activeSummonOfKind in ActiveSummonsOfKind)
                {
                    if (!activeSummonOfKind.Destroyed)
                    {
                        activeSummonOfKind.Destroy(DestroyMode.Vanish);
                    }
                }

                ActiveSummonsOfKind.Clear();
            }
            else
            {
                Log.Message($"{pawn.Label} has no shikigami of kind {KindDef}");
            }
        }




        public List<Pawn> GetActiveSummonsOfKind(PawnKindDef KindDef)
        {
            if (HasShikigamiKind(KindDef))
            {
                return Shikigami[KindDef].PawnInstances;
            }
            else
            {
                Log.Message($"{pawn.Label} has no shikigami of kind {KindDef}");
                return null;
            }
        }


        private void OnShikigamiDeath(Thing obj)
        {
            if (obj is Pawn Pawn)
            {
                if (Pawn.kindDef == JJKDefOf.JJK_DivineDogBlack || Pawn.kindDef == JJKDefOf.JJK_DivineDogWhite)
                {
                    ShouldSummonTotalityDivineDog = true;
                }
            }
        }
    }


    public class ShikigamiData
    {
        public PawnKindDef KindDef;
        public List<Pawn> PawnInstances;
        public bool IsPermanentlyDead = false;

        public ShikigamiData()
        {
            PawnInstances = new List<Pawn>();
            IsPermanentlyDead = false;
        }

        public ShikigamiData(PawnKindDef kindDef) : base()
        {
            KindDef = kindDef;
        }
    }
}


