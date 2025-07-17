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

    public class TenShadowGene : Gene
    {

        private TenShadowsGeneDef Def => (TenShadowsGeneDef)def;

        private Dictionary<ShikigamiDef, ShikigamiMergeTracker> mergedShikigami = new Dictionary<ShikigamiDef, ShikigamiMergeTracker>();


        private List<ShikigamiDef> earnedShadows = new List<ShikigamiDef>();
        public List<ShikigamiDef> EarnedShadows => earnedShadows;


        private Dictionary<ShikigamiDef, ShikigamiData> shikigami = new Dictionary<ShikigamiDef, ShikigamiData>();
        public Dictionary<ShikigamiDef, ShikigamiData> Shikigami => shikigami;

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


        public FormationUtils.FormationType FormationType = FormationUtils.FormationType.Column;
        public float FormationRadius = 3f;
        public bool IsFollowMode = true;



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
            foreach (var item in shikigami)
            {
                ShikigamiDef currentDef = item.Key;
                foreach (var storedPawn in item.Value.StoredPawns)
                {
                    float regenCost = GetShadowRegenCost(currentDef, storedPawn);
                    if (PawnHealingUtility.HealHealthProblem(storedPawn) && CursedEnergy.HasCursedEnergy(regenCost))
                    {                
                        ParentPawn.GetCursedEnergy()?.ConsumeCursedEnergy(regenCost);
                    }
                }
            }
        }

        public void SetFormationType(FormationUtils.FormationType formationType)
        {
            this.FormationType = formationType;
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
            return HasShikigamiKind(KindDef) && shikigami[KindDef].IsPermanentlyDead;
        }

        public void StartShikigamiCooldown(ShikigamiDef ShikigamiDef)
        {

        }

        public bool HasShikigamiKind(ShikigamiDef KindDef)
        {
            return shikigami.ContainsKey(KindDef);
        }

        public ShikigamiData GetShikigamiData(ShikigamiDef KindDef)
        {
            if (HasShikigamiKind(KindDef))
            {
                return shikigami[KindDef];
            }

            return null;
        }

        private ShikigamiData AddNewShikigamiData(ShikigamiDef KindDef)
        {
            ShikigamiData shikigamiData = new ShikigamiData(KindDef);
            shikigami.Add(KindDef, shikigamiData);
            return shikigamiData;
        }
        public Pawn GetOrGenerateShikigami(ShikigamiDef KindDef, PawnKindDef pawnKind, IntVec3 Position, Map Map, bool alwaysGenerateNew = false)
        {
            Log.Message($"Attempting to summon a {KindDef.label} shikigami for {ParentPawn.Label}.");

            Pawn newPawn = null;

            if (!alwaysGenerateNew && HasShikigamiKind(KindDef) && shikigami[KindDef].HasStoredPawnOfKind(pawnKind))
            {
                newPawn = shikigami[KindDef].GetStoredPawnOfKind(pawnKind);
            }
            else
            {
                var shikigamiData = HasShikigamiKind(KindDef) ? shikigami[KindDef] : AddNewShikigamiData(KindDef);
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

            ShikigamiData shikigamiData = GetShikigamiData(KindDef);
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

            //// Formation Type Selection
            //yield return new Command_Action
            //{
            //    defaultLabel = $"Formation: {FormationType}",
            //    defaultDesc = "Change the formation type for your shadows",
            //    icon = TexCommand.HoldOpen,
            //    action = () =>
            //    {
            //        List<FloatMenuOption> options = new List<FloatMenuOption>();
            //        foreach (FormationUtils.FormationType type in Enum.GetValues(typeof(FormationUtils.FormationType)))
            //        {
            //            options.Add(new FloatMenuOption(type.ToString(), () =>
            //            {
            //                SetFormationType(type);
            //            }));
            //        }
            //        Find.WindowStack.Add(new FloatMenu(options));
            //    }
            //};

            //// Formation Radius Adjustment
            //yield return new Command_Action
            //{
            //    defaultLabel = $"Radius: {FormationRadius:F1}",
            //    defaultDesc = "Adjust the formation radius",
            //    icon = TexCommand.GatherSpotActive,
            //    action = () =>
            //    {
            //        List<FloatMenuOption> options = new List<FloatMenuOption>();
            //        float[] radiusOptions = { 1f, 2f, 3f, 4f, 5f, 6f };
            //        foreach (float radius in radiusOptions)
            //        {
            //            options.Add(new FloatMenuOption($"{radius:F1}", () =>
            //            {
            //                FormationRadius = radius;
            //            }));
            //        }
            //        Find.WindowStack.Add(new FloatMenu(options));
            //    }
            //};

            //// Follow Toggle
            //yield return new Command_Toggle
            //{
            //    defaultLabel = "Follow Mode",
            //    defaultDesc = "Toggle whether shadows follow in formation",
            //    icon = TexCommand.ForbidOn,
            //    isActive = () => IsFollowMode,
            //    toggleAction = () =>
            //    {
            //        IsFollowMode = !IsFollowMode;
            //        if (IsFollowMode)
            //        {
            //            Messages.Message($"{pawn.Label}'s shadows will now follow in formation.", MessageTypeDefOf.NeutralEvent);
            //        }
            //        else
            //        {
            //            Messages.Message($"{pawn.Label}'s shadows will hold position.", MessageTypeDefOf.NeutralEvent);
            //        }
            //    }
            //};
        }

        public List<Pawn> GetAllActiveShadows()
        {
            var List = new List<Pawn>();

            foreach (var shikigami in shikigami.Values)
            {
                foreach (var item in shikigami.ActiveShadows)
                {
                    List.Add(item);
                }
            }
            return List;
        }

        public void UnsummonShikigami(ShikigamiDef KindDef)
        {
            if (KindDef == null || !HasShikigamiKind(KindDef))
                return;
            Log.Message($"{ParentPawn.Label} unsummoning Shikigami {KindDef}");

            ShikigamiData shikigamiData = GetShikigamiData(KindDef);

            if (shikigamiData != null)
            {
                shikigamiData.StoreActivePawns();
            }
        }

        public List<Pawn> GetActiveSummonsOfKind(ShikigamiDef KindDef)
        {
            if (HasShikigamiKind(KindDef))
            {
                return shikigami[KindDef].ActiveShadows;
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
                    ShikigamiData shikigamiData = GetShikigamiData(shadowsSummon.ShikigamiDef);

                    if (shikigamiData != null)
                    {
                        shikigamiData.ActiveShadows.Remove(Pawn);
                    }           
                }
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref shikigami, "shikigami", LookMode.Def, LookMode.Deep);
            Scribe_Collections.Look(ref mergedShikigami, "mergedShikigami", LookMode.Def, LookMode.Deep);
            Scribe_Collections.Look(ref earnedShadows, "earnedShadows", LookMode.Def);
            Scribe_Values.Look(ref ShouldSummonTotalityDivineDog, "shouldSummonTotalityDivineDog", false);
        }
    }
}
