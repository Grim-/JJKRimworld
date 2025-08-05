using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
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

    public class TenShadowGene : Gene, IThingHolder
    {
        private TenShadowsGeneDef Def => (TenShadowsGeneDef)def;

        private ShikigamiTracker shikigamiTracker;
        private UnlockedShadowsTracker unlockedShadowsTracker;
        private MergeTracker mergeTracker;

        public bool ShouldSummonTotalityDivineDog = false;
        private Pawn ParentPawn => pawn;

        public Gene_CursedEnergy _CursedEnergy;
        public Gene_CursedEnergy CursedEnergy
        {
            get
            {
                if (_CursedEnergy == null)
                {
                    _CursedEnergy = ParentPawn.GetCursedEnergy();
                }
                return _CursedEnergy;
            }
        }

        public TenShadowGene()
        {
            shikigamiTracker = new ShikigamiTracker(this);
            unlockedShadowsTracker = new UnlockedShadowsTracker();
            mergeTracker = new MergeTracker();
        }

        public IThingHolder ParentHolder => pawn;

        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            outChildren.Add(shikigamiTracker);
        }

        public ThingOwner GetDirectlyHeldThings()
        {
            return null;
        }

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
            if (CursedEnergy == null)
            {
                return;
            }

            Log.Message("Ticking Ten Shadow Regen");

            foreach (var storedPawn in shikigamiTracker.GetDirectlyHeldThings())
            {
                ShikigamiDef currentDef = shikigamiTracker.GetShikigamiDefForPawn(storedPawn as Pawn);
                if (currentDef != null)
                {
                    float regenCost = GetShadowRegenCost(currentDef, storedPawn as Pawn);
                    if (PawnHealingUtility.HealHealthProblem(storedPawn as Pawn) && CursedEnergy.HasCursedEnergy(regenCost))
                    {
                        ParentPawn.GetCursedEnergy()?.ConsumeCursedEnergy(regenCost);
                    }
                }
            }
        }

        private float GetShadowRegenCost(ShikigamiDef kindDef, Pawn shadowPawn)
        {
            if (kindDef == null || shadowPawn == null)
                return 0f;

            float baseCost = kindDef.maintainCost * 0.5f;
            float modifiedCost = baseCost * kindDef.regenCostModifier;
            float healthPercentage = shadowPawn.health.summaryHealth.SummaryHealthPercent;
            float severityMultiplier = 1f + (1f - healthPercentage);
            return modifiedCost * severityMultiplier;
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
                unlockedShadowsTracker.UnlockShikigami(shikigamiDef);
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
            return unlockedShadowsTracker.HasUnlockedShikigami(shikigamiDef);
        }

        public bool IsShikigamiPermanentlyDead(ShikigamiDef KindDef)
        {
            return shikigamiTracker.HasShikigamiKind(KindDef) &&
                   shikigamiTracker.GetShikigamiData(KindDef).IsPermanentlyDead;
        }

        public Pawn GetOrGenerateShikigami(ShikigamiDef KindDef, PawnKindDef pawnKind, IntVec3 Position, Map Map, bool alwaysGenerateNew = false)
        {
            Log.Message($"Attempting to summon a {KindDef.label} shikigami for {ParentPawn.Label}.");

            Pawn newPawn = null;

            if (!alwaysGenerateNew && shikigamiTracker.HasShikigamiKind(KindDef))
            {
                newPawn = shikigamiTracker.GetStoredPawn(KindDef, pawnKind);
            }

            if (newPawn == null)
            {
                var shikigamiData = shikigamiTracker.HasShikigamiKind(KindDef) ?
                    shikigamiTracker.GetShikigamiData(KindDef) :
                    shikigamiTracker.AddNewShikigamiData(KindDef);

                newPawn = GenerateNewShikigami(KindDef, pawnKind, this.pawn, Map, Position);
                shikigamiData.ActiveShadows.Add(newPawn);
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

            ShikigamiData shikigamiData = shikigamiTracker.GetShikigamiData(KindDef);
            if (shikigamiData != null)
            {
                shikigamiData.AddActivePawn(Summon);
            }

            if (Summon.TryGetComp(out Comp_TenShadowsSummon shadowsSummon))
            {
                shadowsSummon.SetMaster(Master, KindDef, shikigamiData);
                shadowsSummon.OnSummon();
            }

            if (Summon.Faction != Master.Faction)
            {
                Summon.SetFaction(Master.Faction);
            }

            DraftingUtility.MakeDraftable(Summon);
            JJKUtility.TrainPawn(Summon, Master);
            Summon.playerSettings.followDrafted = true;
            Summon.playerSettings.followFieldwork = true;
            Summon.playerSettings.Master = Master;
            Summon.relations.AddDirectRelation(PawnRelationDefOf.Bond, Master);
        }

        public void MergeShikigami(ShikigamiDef KindDef, ShikigamiDef MergeIntoKindDef)
        {
            if (KindDef.mergeEffect?.workerClass == null)
                return;

            mergeTracker.MergeShikigami(KindDef, MergeIntoKindDef);

            if (KindDef.summonAbility != null)
            {
                pawn.abilities.RemoveAbility(KindDef.summonAbility);
            }
        }

        public void UnmergeShikigami(ShikigamiDef KindDef)
        {
            if (!mergeTracker.IsShikigamiMerged(KindDef))
                return;

            var mergeData = mergeTracker.GetMergeData(KindDef);
            if (mergeData?.OriginalShikigami.summonAbility != null)
            {
                pawn.abilities.GainAbility(mergeData.OriginalShikigami.summonAbility);
            }

            mergeTracker.UnmergeShikigami(KindDef);
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

        public List<Pawn> GetAllActiveShadows()
        {
            var list = new List<Pawn>();
            foreach (var kvp in shikigamiTracker.ShikigamiData)
            {
                list.AddRange(kvp.Value.ActiveShadows);
            }
            return list;
        }

        public void UnsummonShikigami(ShikigamiDef KindDef)
        {
            if (KindDef == null || !shikigamiTracker.HasShikigamiKind(KindDef))
                return;

            Log.Message($"{ParentPawn.Label} unsummoning Shikigami {KindDef}");
            shikigamiTracker.StoreAllActivePawns(KindDef);
        }

        public List<Pawn> GetActiveSummonsOfKind(ShikigamiDef KindDef)
        {
            return shikigamiTracker.GetActiveSummonsOfKind(KindDef);
        }

        public void OnShikigamiDeath(Pawn Pawn)
        {
            Comp_TenShadowsSummon shadowsSummon = Pawn.GetComp<Comp_TenShadowsSummon>();
            if (shadowsSummon != null && shikigamiTracker.HasShikigamiKind(shadowsSummon.ShikigamiDef))
            {
                ShikigamiData shikigamiData = shikigamiTracker.GetShikigamiData(shadowsSummon.ShikigamiDef);
                if (shikigamiData != null)
                {
                    shikigamiData.ActiveShadows.Remove(Pawn);
                }
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look(ref shikigamiTracker, "shikigamiTracker", this);
            Scribe_Deep.Look(ref unlockedShadowsTracker, "unlockedShadowsTracker");
            Scribe_Deep.Look(ref mergeTracker, "mergeTracker");
            Scribe_Values.Look(ref ShouldSummonTotalityDivineDog, "shouldSummonTotalityDivineDog", false);

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (shikigamiTracker == null)
                {
                    shikigamiTracker = new ShikigamiTracker(this);
                }
                if (unlockedShadowsTracker == null)
                {
                    unlockedShadowsTracker = new UnlockedShadowsTracker();
                }
                if (mergeTracker == null)
                {
                    mergeTracker = new MergeTracker();
                }
            }
        }
    }
}