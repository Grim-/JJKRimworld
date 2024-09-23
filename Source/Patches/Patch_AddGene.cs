using HarmonyLib;
using RimWorld;
using Verse;

namespace JJK
{
    [HarmonyPatch(typeof(Pawn_GeneTracker), nameof(Pawn_GeneTracker.AddGene), new System.Type[] { typeof(GeneDef), typeof(bool) })]
    public static class Patch_AddGene
    {

        static bool Prefix(Pawn_GeneTracker __instance, GeneDef geneDef, bool xenogene)
        {
            Pawn pawn = __instance.pawn;


            //dont let cursed energy be added more than once
            if (geneDef == JJKDefOf.Gene_JJKCursedEnergy && __instance.HasActiveGene(JJKDefOf.Gene_JJKCursedEnergy))
            {
                return false;
            }

            //no double kenjaku genes
            if (geneDef == JJKDefOf.Gene_Kenjaku && __instance.HasActiveGene(JJKDefOf.Gene_Kenjaku))
            {
                return false;
            }


            if (geneDef == JJKDefOf.Gene_JJKCursedEnergy)
            {
                if (!pawn.HasHeavenlyPact())
                {
                    if (Rand.Range(0, 100) < JJKMod.HeavenlyPactChance)
                    {
                        if (Rand.Bool)
                        {
                            if (!pawn.genes.HasActiveGene(JJKDefOf.Gene_JJKHeavenlyPact))
                            {
                                pawn.genes.AddGene(JJKDefOf.Gene_JJKHeavenlyPact, true);
                            }
                        }
                        else
                        {
                            if (!pawn.genes.HasActiveGene(JJKDefOf.Gene_JJKHeavenlyPactCursedEnergy))
                            {
                                pawn.genes.AddGene(JJKDefOf.Gene_JJKHeavenlyPactCursedEnergy, true);
                            }
                        }
                    }
                }


                if (!pawn.HasHeavenlyPact() && !pawn.HasSixEyes())
                {
                    if (Rand.Range(0, 100) < JJKMod.SixEyesChance)
                    {
                        if (!pawn.genes.HasActiveGene(JJKDefOf.Gene_JJKSixEyes))
                        {
                            pawn.genes.AddGene(JJKDefOf.Gene_JJKSixEyes, true);
                        }
                    }
                }

                if (!JJKGeneUtil.HasGradeGene(pawn))
                {
                    JJKGeneUtil.GiveRandomSorcererGrade(pawn);
                }
            }


            if (CanGrantHollowPurple(pawn, geneDef))
            {
                if (!pawn.HasAbility(JJKDefOf.Gojo_HollowPurple))
                {
                    pawn.abilities.GainAbility(JJKDefOf.Gojo_HollowPurple);
                }
            }

            //adding kenjaku gene
            if (geneDef == JJKDefOf.Gene_Kenjaku && !__instance.HasActiveGene(JJKDefOf.Gene_Kenjaku))
            {
                Log.Message($"Kenjaku gene added to a pawn that does not have an existing one");

                HediffComp_KenjakuPossession kenjakuComp = KenjakuUtil.GetOrAddPossessionHediff(pawn);
                if (kenjakuComp != null)
                {
                    kenjakuComp.InitializeOriginalPawn(__instance.pawn);
                    Log.Message($"Intialized Original Pawn");
                }
            }

            return true;
        }

        private static bool CanGrantHollowPurple(Pawn pawn, GeneDef geneDef)
        {
            return geneDef == JJKDefOf.Gene_JJKLimitless && pawn.HasSixEyes() || geneDef == JJKDefOf.Gene_JJKSixEyes && pawn.IsLimitlessUser();
        }
    }
}
    

