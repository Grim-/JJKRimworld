using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace JJK
{
    public class TenShadowsGeneDef : GeneDef
    {
        public List<ShikigamiDef> startingShikigami;

        public TenShadowsGeneDef()
        {
            geneClass = typeof(TenShadowGene);
        }
    }

    public class TenShadowGene : Gene
    {

        private TenShadowsGeneDef Def => (TenShadowsGeneDef)def;

        private Dictionary<ShikigamiDef, ShikigamiMergeTracker> mergedShikigami = new Dictionary<ShikigamiDef, ShikigamiMergeTracker>();
        private List<ShikigamiDef> earnedShadows = new List<ShikigamiDef>();
        private Dictionary<ShikigamiDef, ShikigamiData> Shikigami = new Dictionary<ShikigamiDef, ShikigamiData>();
        public bool ShouldSummonTotalityDivineDog = false;
        private Pawn ParentPawn => pawn;


        public override void PostAdd()
        {
            base.PostAdd();

            if (Def.startingShikigami != null && Def.startingShikigami.Count > 0)
            {
                foreach (var item in Def.startingShikigami)
                {
                    UnlockShikigami(item);
                }
            }
            else
            {
                UnlockShikigami(JJKDefOf.Shikigami_DivineDogs);
            }
        }

        public override void Tick()
        {
            base.Tick();

            if (pawn.IsHashIntervalTick(2500))
            {
                DoShadowsRegenTick();
            }
        }

        protected virtual void DoShadowsRegenTick()
        {
            foreach (var item in Shikigami.Values)
            {
                foreach (var storedPawn in item.StoredPawns)
                {
                    if (PawnHealingUtility.HealHealthProblem(storedPawn))
                    {
                        //deduct some CE to regen damaged shadows
                    }
                }
            }
        }

        private float GetShadowRegenCost(Pawn ShadowPawn)
        {
            return 1f;
        }

        public bool CanSummonShikigamiKind(ShikigamiDef KindDef)
        {
            if (HasUnlockedShikigami(KindDef) && !IsShikigamiPermanentlyDead(KindDef))
            {
                return true;
            }
            return false;
        }

        public void UnlockShikigami(ShikigamiDef shikigamiDef)
        {
            if (!HasUnlockedShikigami(shikigamiDef))
            {
                earnedShadows.Add(shikigamiDef);
                GrantUserSummonAbility(shikigamiDef);

                Messages.Message($"{pawn.Label} has unlocked {shikigamiDef.defName} Shikigami.", MessageTypeDefOf.PositiveEvent);
            }
        }

        private void GrantUserSummonAbility(ShikigamiDef shikigamiDef)
        {
            if (shikigamiDef == null || shikigamiDef.summonAbility == null)
            {
                return;
            }

            if (pawn.abilities.GetAbility(shikigamiDef.summonAbility) == null)
            {
                pawn.abilities.GainAbility(shikigamiDef.summonAbility);
            }
        }

        public void UnlockALLShadows()
        {
            foreach (var item in DefDatabase<ShikigamiDef>.AllDefsListForReading)
            {
                UnlockShikigami(item);
            }
        }

        public bool HasUnlockedShikigami(ShikigamiDef shikigamiDef)
        {
            return earnedShadows.Contains(shikigamiDef);
        }

        public bool IsShikigamiPermanentlyDead(ShikigamiDef KindDef)
        {
            return HasShikigamiKind(KindDef) && Shikigami[KindDef].IsPermanentlyDead;
        }

        public void StartShikigamiCooldown(ShikigamiDef ShikigamiDef)
        {

        }

        public bool HasShikigamiKind(ShikigamiDef KindDef)
        {
            return Shikigami.ContainsKey(KindDef);
        }

        public ShikigamiData GetShikigamiData(ShikigamiDef KindDef)
        {
            if (HasShikigamiKind(KindDef))
            {
                return Shikigami[KindDef];
            }

            return null;
        }

        private ShikigamiData AddNewShikigamiData(ShikigamiDef KindDef)
        {
            ShikigamiData shikigamiData = new ShikigamiData(KindDef);
            Shikigami.Add(KindDef, shikigamiData);
            return shikigamiData;
        }
        public Pawn GetOrGenerateShikigami(ShikigamiDef KindDef, PawnKindDef pawnKind, IntVec3 Position, Map Map, bool alwaysGenerateNew = false)
        {
            Log.Message($"Attempting to summon a {KindDef.label} shikigami for {ParentPawn.Label}.");

            Pawn newPawn = null;

            if (!alwaysGenerateNew && HasShikigamiKind(KindDef) && Shikigami[KindDef].HasStoredPawnOfKind(pawnKind))
            {
                newPawn = Shikigami[KindDef].GetStoredPawnOfKind(pawnKind);
            }
            else
            {
                var shikigamiData = HasShikigamiKind(KindDef) ? Shikigami[KindDef] : AddNewShikigamiData(KindDef);
                newPawn = GenerateNewShikigami(KindDef, pawnKind, this.pawn, Map, Position);
                shikigamiData.PawnInstances.Add(newPawn);
            }

            SetupSummon(KindDef, this.pawn, newPawn);

            if (!newPawn.Spawned)
            {
                GenSpawn.Spawn(newPawn, Position, Map);
            }
            return newPawn;
        }

        public Pawn GenerateNewShikigami(ShikigamiDef KindDef, PawnKindDef pawnKindDef, Pawn Master, Map Map, IntVec3 Position)
        {
            if (pawnKindDef == null || Map == null)
            {
                return null;
            }

            Pawn shikigami = PawnGenerator.GeneratePawn(pawnKindDef, Master.Faction);

            SetupSummon(KindDef, Master, shikigami);
            return shikigami;
        }

        private void SetupSummon(ShikigamiDef KindDef, Pawn Master, Pawn Summon)
        {
            Summon.ageTracker.DebugSetAge(3444444);

            if (Summon.abilities == null)
            {
                Summon.abilities = new Pawn_AbilityTracker(Summon);
            }

            Hediff_Shikigami shikigamiHediff = (Hediff_Shikigami)Summon.health.GetOrAddHediff(JJKDefOf.JJK_Shikigami);
 
            if (shikigamiHediff != null)
            {
                shikigamiHediff.SetMaster(Master);
            }


            Comp_TenShadowsSummon shadowsSummon = Summon.GetComp<Comp_TenShadowsSummon>();
            if (shadowsSummon != null)
            {
                shadowsSummon.SetMaster(Master, KindDef);
                shadowsSummon.OnSummon();
            }

            if (Summon.Faction != Master.Faction)
            {
                Summon.SetFaction(Master.Faction);
            } 

            DraftingUtility.MakeDraftable(Summon);
            JJKUtility.TrainPawn(Summon, Master);

            ShikigamiData shikigamiData = GetShikigamiData(KindDef);

            if (shikigamiData != null)
            {
                if (!shikigamiData.PawnInstances.Contains(Summon))
                {
                    shikigamiData.PawnInstances.Add(Summon);
                }        
            }
        }
        public void MergeShikigami(ShikigamiDef KindDef, ShikigamiDef MergeIntoKindDef)
        {
            if (KindDef.mergeEffect?.workerClass == null)
                return;


            if (KindDef.summonAbility != null)
            {
                pawn.abilities.RemoveAbility(KindDef.summonAbility);
            }
        }

        public void UnmergeShikigami(ShikigamiDef KindDef)
        {
            if (!mergedShikigami.TryGetValue(KindDef, out var tracker))
                return;

            if (tracker.OriginalShikigami.summonAbility != null)
            {
                pawn.abilities.GainAbility(tracker.OriginalShikigami.summonAbility);
            }

            mergedShikigami.Remove(KindDef);
        }
        public override IEnumerable<Gizmo> GetGizmos()
        {
            if (Prefs.DevMode)
            {
                yield return new Command_Action()
                {
                    defaultLabel = "Unlock ALL Ten Shadows",
                    action = () =>
                    {
                        UnlockALLShadows();
                    }
                };

            }
        }

        public void UnsummonShikigami(ShikigamiDef KindDef)
        {
            if (KindDef == null || !HasShikigamiKind(KindDef))
                return;
            Log.Message($"{ParentPawn.Label} unsummoning Shikigami {KindDef}");

            ShikigamiData shikigamiData = GetShikigamiData(KindDef);

            shikigamiData.StoreActivePawns();
        }

        public List<Pawn> GetActiveSummonsOfKind(ShikigamiDef KindDef)
        {
            if (HasShikigamiKind(KindDef))
            {
                return Shikigami[KindDef].PawnInstances;
            }
            else
            {
                Log.Message($"{ParentPawn.Label} has no shikigami of kind {KindDef}");
                return null;
            }
        }

        public void OnShikigamiDeath(Pawn Pawn)
        {
            Comp_TenShadowsSummon shadowsSummon = Pawn.GetComp<Comp_TenShadowsSummon>();

            if (shadowsSummon != null)
            {
                if (HasShikigamiKind(shadowsSummon.ShikigamiDef))
                {
                    Shikigami[shadowsSummon.ShikigamiDef].PawnInstances.Remove(Pawn);
                }
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref Shikigami, "shikigami", LookMode.Def, LookMode.Deep);
            Scribe_Collections.Look(ref mergedShikigami, "mergedShikigami", LookMode.Def, LookMode.Deep);
            Scribe_Collections.Look(ref earnedShadows, "earnedShadows", LookMode.Def);
            Scribe_Values.Look(ref ShouldSummonTotalityDivineDog, "shouldSummonTotalityDivineDog", false);
        }
    }
}
