using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace JJK
{
    //[HarmonyPatch(typeof(Toils_Recipe), "FinishRecipeAndStartStoringProduct")]
    //public static class RecipePatchClass
    //{
    //    [HarmonyPostfix]
    //    public static void Postfix(Toil __result)
    //    {
    //        __result.AddFinishAction(() =>
    //        {
    //            Pawn actor = __result.actor;
    //            if (actor?.CurJob?.bill == null) return;

    //            Bill bill = actor.CurJob.bill;
    //            Thing billGiver = actor.CurJob.GetTarget(TargetIndex.A).Thing;

    //            string jobInfo = $"DoBill job completed by {actor.LabelShort}:\n" +
    //                             $"Recipe: {bill.recipe.defName}\n" +
    //                             $"Bill Giver: {billGiver.def.defName} (at {billGiver.Position})\n" +
    //                             $"Products made: {bill.recipe.ProducedThingDef}";

    //            if (bill is Bill_Production billProduction)
    //            {
    //                jobInfo += $"\nRepeat mode: {billProduction.repeatMode}";
    //                if (billProduction.repeatMode == BillRepeatModeDefOf.RepeatCount)
    //                {
    //                    jobInfo += $"\nRemaining repeat count: {billProduction.repeatCount}";
    //                }
    //            }

    //            Log.Message(jobInfo);
    //        });
    //    }
    //}
}
    

