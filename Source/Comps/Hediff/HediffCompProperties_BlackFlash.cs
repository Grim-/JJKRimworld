using RimWorld;
using System.Collections.Generic;
using Verse;

namespace JJK
{
    public class HediffCompProperties_BlackFlash : HediffCompProperties
    {
        public float TriggerChance = 0.3f;
        public float DamageMultiplier = 1.5f;
        public int AmountOfTimesToTrigger = 1;
        public List<BlackFlashBonusAbilities> BonusAbilities;

        public HediffCompProperties_BlackFlash()
        {
            compClass = typeof(BlackFlash);
        }
    }
    public class BlackFlash : HediffComp
    {
        public HediffCompProperties_BlackFlash Props => (HediffCompProperties_BlackFlash)props;
        public override string CompDescriptionExtra => base.CompDescriptionExtra + $"\r\n You have a {BlackFlashTriggerChance * 100}% chance to trigger a black flash on melee attack.";
        public float BlackFlashTriggerChance => parent.pawn.GetStatValue(JJKDefOf.JJK_BlackFlashTriggerChance);

        public override void Notify_PawnPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            base.Notify_PawnPostApplyDamage(dinfo, totalDamageDealt);

            float Rand = UnityEngine.Random.Range(0, 1f);


            Log.Message($"Black flash trigger rand [{Rand}] chance [{BlackFlashTriggerChance}] IsLessThanOrEqual {Rand <= BlackFlashTriggerChance}");


            if (Rand <= BlackFlashTriggerChance)
            {
                if (dinfo.IntendedTarget != null && !dinfo.IntendedTarget.Destroyed)
                {
                    float BlackFlashDamage = totalDamageDealt * Props.DamageMultiplier;

                    for (int i = 0; i < Props.AmountOfTimesToTrigger; i++)
                    {
                        DamageInfo BF = new DamageInfo(dinfo);
                        BF.SetAmount(BlackFlashDamage);
                        dinfo.IntendedTarget.TakeDamage(BF);
                        Effecter effecter = EffecterDefOf.Skip_Entry.SpawnAttached(parent.pawn, parent.pawn.MapHeld, 1f);     
                    }

                }

                TriggerBlackFlash(parent.pawn, Props.BonusAbilities);
            }
        }


        public static void TriggerBlackFlash(Pawn Pawn, List<BlackFlashBonusAbilities> BonusAbilities)
        {
            if (!Pawn.health.hediffSet.HasHediff(JJKDefOf.JJK_CursedEnergySurge))
            {
                Pawn.health.AddHediff(JJKDefOf.JJK_CursedEnergySurge);
                Messages.Message("Black Flash!", MessageTypeDefOf.PositiveEvent);

                if (BonusAbilities != null)
                {
                    foreach (var item in BonusAbilities)
                    {
                        if (item.Ability != null && !Pawn.HasAbility(item.Ability) && Rand.Chance(item.Chance))
                        {
                            Pawn.abilities.GainAbility(item.Ability);
                        }
                    }
                }
            }
        }
    }


    public class BlackFlashBonusAbilities
    {
        public float Chance = 0.5f;
        public AbilityDef Ability;
    }
}

