using System.Collections.Generic;
using System.Linq;
using Verse;

namespace JJK
{
    public static class PawnHealingUtility
    {
        public static bool RestoreMissingPart(Pawn target)
        {
            List<Hediff_MissingPart> missingParts = GetMissingPartsPrioritized(target);
            if (missingParts.Count > 0)
            {
                Hediff_MissingPart highestPrio = missingParts.First();
                HealthUtility.Cure(highestPrio);
                return true;
            }
            return false;
        }

        public static void HealHealthProblem(Pawn target)
        {
            Hediff problem = GetMostSevereHealthProblem(target);
            if (problem != null)
            {
                Hediff problemToHeal = target.health.hediffSet.hediffs
                  .Where(x => !(x is Hediff_MissingPart) && x.Visible && x.def != JJKDefOf.ZombieWorkSlaveHediff)
                  .OrderByDescending(x => x.Severity)
                  .FirstOrDefault();

                if (problemToHeal != null)
                {
                    HealthUtility.Cure(problemToHeal);
                }
            }
        }
        public static Hediff GetMostSevereHealthProblem(Pawn pawn)
        {
            return pawn.health.hediffSet.hediffs
                .Where(x => !(x is Hediff_MissingPart) && x.Visible)
                .OrderByDescending(x => x.Severity)
                .FirstOrDefault();
        }

        public static List<Hediff_MissingPart> GetMissingPartsPrioritized(Pawn pawn)
        {
            return pawn.health.hediffSet.hediffs
                .OfType<Hediff_MissingPart>()
                .OrderByDescending(x => x.Part.def.hitPoints)
                .ThenByDescending(x => x.Part.def.GetMaxHealth(pawn))
                .ToList();
        }
    }



}

