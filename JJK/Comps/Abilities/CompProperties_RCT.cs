using JJK;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Noise;
using static HarmonyLib.Code;

namespace JJK
{
    public class CompProperties_RCT : CompProperties_RCTBase
    {
        //public int PartRegenTickCount = 250;
        //public float CECostPerTick = 0.005f;
        //public float CEPartRegenCost = 0.1f;
        //public float CEHediffCost = 0.05f;

        //public float CEPartRegenSeverityBase = 0.1f;
        //public float CEProblemRegenSeverityBase = 0.05f;

        //public bool CanCureAddiction = false;
        //public bool ShouldRemoveImplants = false;
        public CompProperties_RCT()
        {
            compClass = typeof(CompAbilityEffect_RCT);
        }
    }
    public class CompAbilityEffect_RCT : CompAbilityEffect_RCTBase
    {
        new CompProperties_RCT Props => (CompProperties_RCT)props;

        //private bool IsCurrentlyCasting = false;
        private int CurrentTick = 0;
        public override void ApplyAbility(LocalTargetInfo target, LocalTargetInfo dest)
        {
            Hediff CurrentRCTHediff = parent.pawn.health.hediffSet.GetFirstHediffOfDef(JJK.JJKDefOf.RCTRegenHediff);
            bool HasRCTActive = CurrentRCTHediff != null;
            IsCurrentlyCasting = !IsCurrentlyCasting;
            CurrentTick = 0;

            if (IsCurrentlyCasting)
            {
                if (!HasRCTActive)
                {
                    AddRCTHediff(parent.pawn);

                }

            }
            else
            {
                parent.pawn.health.RemoveHediff(CurrentRCTHediff);
            }
        }


        public override void CompTick()
        {
            base.CompTick();
            CurrentTick++;

            float RCTSpeedBonus = parent.pawn.GetStatValue(JJKDefOf.JJK_RCTSpeedBonus);

            Gene_CursedEnergy CursedEnergyResourceGene = parent.pawn.genes?.GetFirstGeneOfType<Gene_CursedEnergy>();

            Hediff CurrentRCTHediff = parent.pawn.health.hediffSet.GetFirstHediffOfDef(JJK.JJKDefOf.RCTRegenHediff);
            bool HasRCTActive = CurrentRCTHediff != null;

            //float TickCost = Props.CECostPerTick * parent.pawn.GetStatValue(JJKDefOf.JJK_CursedEnergyCost);
            //float PartCost = Props.CEPartRegenCost * parent.pawn.GetStatValue(JJKDefOf.JJK_CursedEnergyCost);

            if (IsCurrentlyCasting)
            {
                if (HasRCTActive)
                {
                    if (CurrentTick % Props.PartRegenTickCount / (int)RCTSpeedBonus == 0)
                    {
                        OnTick(CursedEnergyResourceGene);
                        EffecterDefOf.ShamblerRaise.SpawnAttached(parent.pawn, parent.pawn.Map);
                        CurrentTick = 0;
                    }
                }
                else
                {
                    IsCurrentlyCasting = false;
                    CurrentTick = 0;
                }

            }
        }

        protected override void StopCasting()
        {
            base.StopCasting();
            CurrentTick = 0;
        }

        private void OnTick(Gene_CursedEnergy CursedEnergy)
        {
            if (CursedEnergy.Value >= GetTickCost())
            {
                if (CursedEnergy.Value >= GetPartCost())
                {
                    HealTargetPawn(CursedEnergy, parent.pawn);
                }
            }
            else
            {
                RemoveRCTHediff(parent.pawn);
            }

            parent.pawn.GetCursedEnergy()?.ConsumeCursedEnergy(parent.pawn, GetTickCost());
        }

        public override float GetCost()
        {
            return Props.cursedEnergyCost;
        }

        //private void RegeneratePart(Gene_CursedEnergy CursedEnergy, float Cost)
        //{
        //    List<Hediff_MissingPart> MissingParts = GetMissingPartsPrioritized();

        //    if (MissingParts != null && MissingParts.Count > 0)
        //    {
        //        Hediff_MissingPart HighestPrio = MissingParts.FirstOrDefault();

        //        if (HighestPrio != null)
        //        {
        //            float CostForPart = (Props.CEPartRegenSeverityBase * HighestPrio.Severity);
        //            Log.Message($"Hediff cost {CostForPart} Base :  {Props.CEProblemRegenSeverityBase} Severity {HighestPrio.Severity}");
        //            if (CursedEnergy.Value >= CostForPart)
        //            {
        //                HealthUtility.Cure(HighestPrio);
        //                parent.pawn.GetCursedEnergy()?.ConsumeCursedEnergy(parent.pawn, CostForPart);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        Hediff problem = GetOtherProblems();


        //        if (problem != null)
        //        {
        //            float CostForProblem = (Props.CEProblemRegenSeverityBase * problem.Severity);

        //            Log.Message($"Hediff cost {CostForProblem} Base :  {Props.CEProblemRegenSeverityBase} Severity {problem.Severity}");

        //            if (CursedEnergy.Value >= CostForProblem)
        //            {
        //                HealthUtility.Cure(problem);
        //                parent.pawn.GetCursedEnergy()?.ConsumeCursedEnergy(parent.pawn, CostForProblem);

        //            }

        //        }
        //    }

        //}


        public override string CompInspectStringExtra()
        {
            string String = base.CompInspectStringExtra();

            if (IsCurrentlyCasting) String = String + "RCT Active";
            return String;
        }

        //private List<Hediff_MissingPart> GetMissingPartsPrioritized()
        //{
        //    List<Hediff_MissingPart> MissingParts = parent.pawn.health.hediffSet.hediffs.Where((HeDiff) =>
        //    {
        //        return HeDiff is Hediff_MissingPart;

        //    }).Cast<Hediff_MissingPart>().OrderByDescending(x => x.TendPriority).ToList();

        //    return MissingParts;
        //}

        //private Hediff GetOtherProblems()
        //{
        //    return parent.pawn.health.hediffSet.hediffs.Where(hediff =>
        //    {
        //        bool RemoveImplant = Props.ShouldRemoveImplants && hediff.GetType() == typeof(Hediff_Implant);
        //        bool CureAddiction = Props.CanCureAddiction && hediff.GetType() == typeof(Hediff_Addiction);

        //        return CureAddiction &&
        //              RemoveImplant;
        //    })
        //    .OrderByDescending(x => x.TendPriority)
        //    .FirstOrDefault();
        //}




    }


}

    //public class CompAbilityEffect_RCT : CompAbilityEffect_RCTBase
    //{
    //    protected new CompProperties_RCT Props => (CompProperties_RCT)props;


    //    private Hediff CurrentRCTHediff => parent.pawn.health.hediffSet.GetFirstHediffOfDef(JJKDefOf.RCTRegenHediff);

    //    protected override bool ShouldStopCasting()
    //    {
    //        return CurrentRCTHediff == null;
    //    }

    //    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    //    {
    //        return base.CompGetGizmosExtra();
    //    }

    //    public override void ApplyAbility(LocalTargetInfo target, LocalTargetInfo dest)
    //    {
    //        IsCurrentlyCasting = !IsCurrentlyCasting;
    //        CurrentTick = 0;

    //        if (IsCurrentlyCasting)
    //        {
    //            if (CurrentRCTHediff == null)
    //            {
    //                AddRCTHediff(parent.pawn);
    //            }
    //        }
    //        else
    //        {
    //            parent.pawn.health.RemoveHediff(CurrentRCTHediff);
    //        }
    //    }

    //    private void AddRCTHediff(Pawn Pawn)
    //    {
    //        Hediff NewRCT = HediffMaker.MakeHediff(JJKDefOf.RCTRegenHediff, Pawn);
    //        BodyPartRecord brainPart = Pawn.health.hediffSet.GetBrain() ??
    //            Pawn.health.hediffSet.GetNotMissingParts().FirstOrDefault(x => x.def == BodyPartDefOf.Head);

    //        if (brainPart != null)
    //        {
    //            Pawn.health.AddHediff(NewRCT, brainPart);
    //        }
    //        else
    //        {
    //            Messages.Message("RCT cannot be applied to " + Pawn.LabelShort + ". RCT stems from the brain, which is missing.",
    //                  Pawn, MessageTypeDefOf.NegativeEvent);
    //        }
    //    }

    //    protected override Pawn GetTarget()
    //    {
    //        return parent.pawn;
    //    }
    //}






//public class CompAbilityEffect_RCT : BaseCursedEnergyAbility
//{
//    new CompProperties_RCT Props => (CompProperties_RCT)props;

//    private bool IsCurrentlyCasting = false;
//    private int CurrentTick = 0;

//    public override void CompTick()
//    {
//        base.CompTick();
//        CurrentTick++;

//        float RCTSpeedBonus = parent.pawn.GetStatValue(JJKDefOf.JJK_RCTSpeedBonus);

//        Gene_CursedEnergy CursedEnergyResourceGene = parent.pawn.genes?.GetFirstGeneOfType<Gene_CursedEnergy>();

//        Hediff CurrentRCTHediff = parent.pawn.health.hediffSet.GetFirstHediffOfDef(JJK.JJKDefOf.RCTRegenHediff);
//        bool HasRCTActive = CurrentRCTHediff != null;

//        float TickCost = Props.CECostPerTick * parent.pawn.GetStatValue(JJKDefOf.JJK_CursedEnergyCost);
//        float PartCost = Props.CEPartRegenCost * parent.pawn.GetStatValue(JJKDefOf.JJK_CursedEnergyCost);

//        if (IsCurrentlyCasting)
//        {
//            if (HasRCTActive)
//            {
//                if (CurrentTick % Props.PartRegenTickCount / (int)RCTSpeedBonus == 0)
//                {
//                    OnTick(CursedEnergyResourceGene, TickCost, PartCost);
//                    EffecterDefOf.DryadSpawn.SpawnAttached(parent.pawn, parent.pawn.Map);
//                    CurrentTick = 0;
//                }
//            }
//            else
//            {
//                IsCurrentlyCasting = false;
//                CurrentTick = 0;
//            }

//        }
//    }

//    private void OnTick(Gene_CursedEnergy CursedEnergy, float TickCost, float PartCost)
//    {
//        Hediff CurrentRCTHediff = parent.pawn.health.hediffSet.GetFirstHediffOfDef(JJK.JJKDefOf.RCTRegenHediff);

//        if (CursedEnergy.Value >= TickCost)
//        {
//            if (CursedEnergy.Value >= PartCost)
//            {      
//                RegeneratePart(CursedEnergy, PartCost);
//            }
//        }
//        else
//        {
//            if (CurrentRCTHediff != null)
//            {
//                parent.pawn.health.RemoveHediff(CurrentRCTHediff);
//            }

//           // IsCurrentlyCasting = false;
//        }



//        parent.pawn.GetCursedEnergy()?.ConsumeCursedEnergy(parent.pawn, TickCost);
//    }

//    public override float GetCost()
//    {
//        return Props.cursedEnergyCost;
//    }

//    private void RegeneratePart(Gene_CursedEnergy CursedEnergy, float Cost)
//    {
//        List<Hediff_MissingPart> MissingParts = GetMissingPartsPrioritized();

//        if (MissingParts != null && MissingParts.Count > 0)
//        {
//            Hediff_MissingPart HighestPrio = MissingParts.FirstOrDefault();

//            if (HighestPrio != null)
//            {
//                float CostForPart = Cost + (Props.CEPartRegenSeverityBase * HighestPrio.Severity);
//                Log.Message($"Hediff cost {CostForPart} Base :  {Props.CEProblemRegenSeverityBase} Severity {HighestPrio.Severity}");
//                if (CursedEnergy.Value >= CostForPart)
//                {
//                    HealthUtility.Cure(HighestPrio);
//                    parent.pawn.GetCursedEnergy()?.ConsumeCursedEnergy(parent.pawn, Cost);
//                }
//            }
//        }
//        else
//        {
//            Hediff problem = GetOtherProblems();


//            if (problem != null)
//            {
//                float CostForProblem = Cost + (Props.CEProblemRegenSeverityBase * problem.Severity);

//                Log.Message($"Hediff cost {CostForProblem} Base :  {Props.CEProblemRegenSeverityBase} Severity {problem.Severity}");

//                if (CursedEnergy.Value >= CostForProblem)
//                {
//                    HealthUtility.Cure(problem);
//                    parent.pawn.GetCursedEnergy()?.ConsumeCursedEnergy(parent.pawn, Cost);

//                }

//            }
//        }

//    }


//    private List<Hediff_MissingPart> GetMissingPartsPrioritized()
//    {
//        List<Hediff_MissingPart> MissingParts = parent.pawn.health.hediffSet.hediffs.Where((HeDiff) =>
//        {
//            return HeDiff is Hediff_MissingPart;

//        }).Cast<Hediff_MissingPart>().OrderByDescending(x => x.TendPriority).ToList();

//        return MissingParts;
//    }

//    private Hediff GetOtherProblems()
//    {
//        return parent.pawn.health.hediffSet.hediffs.Where(hediff =>
//        {
//            bool RemoveImplant = Props.ShouldRemoveImplants && hediff.GetType() == typeof(Hediff_Implant);
//            bool CureAddiction = Props.CanCureAddiction && hediff.GetType() == typeof(Hediff_Addiction);

//            return CureAddiction &&
//                  RemoveImplant;
//        })
//        .OrderByDescending(x => x.TendPriority)
//        .FirstOrDefault();
//    }


//    public override void ApplyAbility(LocalTargetInfo target, LocalTargetInfo dest)
//    {
//        Hediff CurrentRCTHediff = parent.pawn.health.hediffSet.GetFirstHediffOfDef(JJK.JJKDefOf.RCTRegenHediff);
//        bool HasRCTActive = CurrentRCTHediff != null;
//        IsCurrentlyCasting = !IsCurrentlyCasting;
//        CurrentTick = 0;

//        if (IsCurrentlyCasting)
//        {
//            if (!HasRCTActive)
//            {
//                AddRCTHediff(parent.pawn);

//            }

//        }
//        else
//        {
//            parent.pawn.health.RemoveHediff(CurrentRCTHediff);
//        }
//    }

//    private void AddRCTHediff(Pawn Pawn)
//    {
//        Hediff NewRCT = HediffMaker.MakeHediff(JJK.JJKDefOf.RCTRegenHediff, parent.pawn);

//        // Get the brain part
//        BodyPartRecord brainPart = Pawn.health.hediffSet.GetBrain();

//        // If brain is not found, try to get the head
//        if (brainPart == null)
//        {
//            brainPart = Pawn.health.hediffSet.GetNotMissingParts()
//                .FirstOrDefault(x => x.def == BodyPartDefOf.Head);
//        }

//        // Add the hediff to the brain or head
//        if (brainPart != null)
//        {
//            Pawn.health.AddHediff(NewRCT, brainPart);
//        }
//        else
//        {
//            Messages.Message("RCT cannot be applied to " + Pawn.LabelShort + ". RCT stems from the brain, which is missing.",
//                  Pawn, MessageTypeDefOf.NegativeEvent);
//        }
//    }


//}


//}

