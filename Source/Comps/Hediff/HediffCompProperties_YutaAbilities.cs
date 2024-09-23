using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace JJK
{
    public class HediffCompProperties_YutaAbilities : HediffCompProperties
    {
        public HediffCompProperties_YutaAbilities()
        {
            compClass = typeof(HediffComp_YutaAbilities);
        }
    }

    public class HediffComp_YutaAbilities : HediffComp
    {
        private List<AbilityDef> _StoredAbilities = new List<AbilityDef>();
        public List<AbilityDef> StoredAbilities => _StoredAbilities;

        public bool HasGranted = false;

        public void GrantRiikaAbilities(Pawn Pawn)
        {
            if (HasGranted)
            {
                return;
            }


            HediffComp_YutaAbilities yutaAbilities = Pawn.GetYutaAbilityStorage();

            if (yutaAbilities != null)
            {
                foreach (var item in yutaAbilities.StoredAbilities.ToList())
                {
                    if (!Pawn.HasAbility(item))
                    {
                        Pawn.abilities.GainAbility(item);
                    }
                }

                HasGranted = true;
            }
        }

        public void RemoveRiikaAbilities(Pawn Pawn)
        {
            if (!HasGranted)
            {
                return;
            }

            HediffComp_YutaAbilities yutaAbilities = Pawn.GetYutaAbilityStorage();

            if (yutaAbilities != null)
            {
                foreach (var item in yutaAbilities.StoredAbilities.ToList())
                {
                    if (Pawn.HasAbility(item))
                    {
                        Pawn.abilities.RemoveAbility(item);
                    }
                }

                HasGranted = false;
            }
        }


        public void AddAbility(AbilityDef ability)
        {
            if (!StoredAbilities.Contains(ability))
            {
                StoredAbilities.Add(ability);
            }
        }

        public void RemoveAbility(AbilityDef ability)
        {
            if (StoredAbilities.Contains(ability))
            {
                StoredAbilities.Remove(ability);
            }
        }
    }


    public static class YutaUtil
    {
        public static HediffComp_YutaAbilities GetYutaAbilityStorage(this Pawn pawn)
        {
            Hediff hediff = pawn.health.GetOrAddHediff(JJKDefOf.JJK_YutaAbilityStore);
            return hediff.TryGetComp<HediffComp_YutaAbilities>();
        }
    }
}

