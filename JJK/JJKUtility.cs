using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace JJK
{
    public static class JJKUtility
    {
        public static bool IsLimitlessUser(this Pawn pawn)
        {
            if (pawn?.genes == null)
            {
                // If pawn or its genes are null, it cannot be a blue user
                return false;
            }

            return pawn.genes.HasActiveGene(JJKDefOf.Gene_JJKLimitless);
        }

        public static bool HasSixEyes(this Pawn pawn)
        {
            if (pawn?.genes == null)
            {
                // If pawn or its genes are null, it cannot be a blue user
                return false;
            }

            return pawn.genes.HasActiveGene(JJKDefOf.Gene_JJKSixEyes);
        }
        public static bool HasAbility(this Pawn pawn, AbilityDef AbiityDef)
        {
            if (pawn?.abilities == null)
            {
                return false;
            }

            return pawn.abilities.AllAbilitiesForReading.Find(x=> x.def == AbiityDef) != null;
        }

        public static Gene_CursedEnergy GetCursedEnergy(this Pawn pawn)
        {
            return pawn.genes?.GetFirstGeneOfType<Gene_CursedEnergy>();
        }
        public static Gene_Kenjaku GetKenjakuGene(this Pawn pawn)
        {
            return pawn.genes?.GetFirstGeneOfType<Gene_Kenjaku>();
        }
    }
}

