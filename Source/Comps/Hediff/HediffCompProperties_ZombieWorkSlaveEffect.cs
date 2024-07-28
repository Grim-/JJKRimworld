using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace JJK
{
    public class HediffCompProperties_ZombieWorkSlaveEffect : HediffCompProperties
    {
        public int RegenerationTicks = 250;
        public int TicksBeforeBerserkWithoutMaster = 2000;
        public Color Color = new Color(0.2f, 0.7f, 0.2f, 1f);

        public HediffCompProperties_ZombieWorkSlaveEffect()
        {
            compClass = typeof(HediffComp_ZombieWorkSlaveEffect);
        }
    }


    public class HediffComp_ZombieWorkSlaveEffect : HediffComp
    {
        public HediffCompProperties_ZombieWorkSlaveEffect Props => (HediffCompProperties_ZombieWorkSlaveEffect)props;

        Need_Suppression _Suppression;
        Need_Suppression Suppression
        {
            get
            {
                if (_Suppression == null)
                {
                    _Suppression = parent.pawn.needs.TryGetNeed<Need_Suppression>();
                }

                return _Suppression;
            }
        }


        protected Faction OriginalFaction;

        protected Pawn Master;
        protected int CurrentTick = 0;
        protected int TicksWithoutMaster = 0;
        private Color originalColor;
        private bool colorChanged = false;
        public void SetSlaveMaster(Pawn Pawn)
        {
            Master = Pawn;
            ApplyZombieColor(Pawn);
            CurrentTick = 0;
            TicksWithoutMaster = 0;
        }


        public override void CompPostPostRemoved()
        {
            base.CompPostPostRemoved();
            RestoreOriginalColor(Pawn);
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            Pawn pawn = parent.pawn;
            if (pawn == null) return;

            CurrentTick++;
            if (CurrentTick >= Props.RegenerationTicks)
            {
                OnRegenTick(pawn);
                CurrentTick = 0;
            }

            if (Master.Dead)
            {
                TicksWithoutMaster++;
                if (TicksWithoutMaster >= Props.TicksBeforeBerserkWithoutMaster)
                {
                    Log.Message($"Zombify: Master dead for {Props.TicksBeforeBerserkWithoutMaster}.");
                    TriggerBerserk(pawn);
                }
            }
            else
            {
                TicksWithoutMaster = 0;
            }


            HandleNeedsAndSupress(pawn);
        }

        private void OnRegenTick(Pawn pawn)
        {
            Gene_CursedEnergy MasterCursedEnergy = Master.GetCursedEnergy();

            if (MasterCursedEnergy != null)
            {
                MasterCursedEnergy.ConsumeCursedEnergy(0.002f);
            }

            if (!PawnHealingUtility.RestoreMissingPart(pawn))
            {
                PawnHealingUtility.HealHealthProblem(pawn);
            }

        }

        private void HandleNeedsAndSupress(Pawn pawn)
        {
            // Automatically satisfy needs
            foreach (Need need in pawn.needs.AllNeeds)
            {
                need.CurLevel = need.MaxLevel;
            }

            if (Suppression != null && Suppression.CurLevelPercentage <= 0.94f)
            {
                SlaveRebellionUtility.IncrementSuppression(Suppression, pawn, pawn, 5);
            }
        }

        private void ApplyZombieColor(Pawn pawn)
        {
            //originalColor = pawn.Drawer.renderer.BodyGraphic.color;
            //pawn.Drawer.renderer.BodyGraphic.color = Props.Color;
            //pawn.Drawer.renderer.SetAllGraphicsDirty();
            //colorChanged = true;
        }

        private void RestoreOriginalColor(Pawn pawn)
        {
            //pawn.Drawer.renderer.BodyGraphic.color = originalColor;
            //pawn.Drawer.renderer.SetAllGraphicsDirty();
            //colorChanged = false;
        }


        private void TriggerBerserk(Pawn pawn)
        {
            pawn.guest.SetGuestStatus(OriginalFaction, GuestStatus.Guest);
            pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Berserk, null, true, true);
           // pawn.health.RemoveHediff(parent);
        }
        public override void CompExposeData()
        {
            base.CompExposeData();

            Scribe_References.Look(ref Master, "pawnSlaveMaster");
            Scribe_Values.Look(ref CurrentTick, "slaveCurrentRegenTick");
            Scribe_Values.Look(ref TicksWithoutMaster, "slaveTickWithoutMaster");
            Scribe_References.Look(ref OriginalFaction, "slaveOriginalFaction");
        }
    }

}


