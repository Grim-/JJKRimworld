using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace JJK
{
    [DefOf]
    internal class JJKDefOf
    {

        public static StatDef JJK_CursedEnergy;
        public static StatDef JJK_CursedEnergyCost;
        public static StatDef JJK_CursedEnergyRegen;
        public static StatDef JJK_CursedEnergyRegenSpeed;
        public static StatDef JJK_CursedEnergyDamageBonus;

        public static StatDef JJK_RCTHealingBonus;
        public static StatDef JJK_RCTSpeedBonus;

        public static GeneDef Gene_JJKCursedEnergy;
        public static GeneDef Gene_JJKGrade1;
        public static GeneDef Gene_JJKGrade2;
        public static GeneDef Gene_JJKGrade3;
        public static GeneDef Gene_JJKHeavenlyPact;
        public static GeneDef Gene_Shoko;
        public static GeneDef Gene_JJKSixEyes;
        public static GeneDef Gene_JJKSpecialGrade;
        public static GeneDef Gene_JJKSpecialGrade_Monstrous;
        public static GeneDef Gene_JJKSukuna;
        public static GeneDef Gene_Kenjaku;
        public static GeneDef Gene_JJKLimitless;
        public static GeneDef Gene_JJKMahito;


        public static GeneDef Gene_JJKGeto;


        public static ThingDef JJK_Flyer;


        public static MentalStateDef TransfiguredState_Murderous;

        public static AbilityDef JJK_KenjakuPosess;
        public static AbilityDef Gojo_HollowPurple;
        public static ThingDef JJK_idleTransfigurationDoll;

        public static ThinkTreeDef JJK_EmptyConstantThinkTree;
        public static ThinkTreeDef JJK_SummonedCreature;
        public static ThinkTreeDef ZombieWorkSlave;


        public static JobDef JJK_DefendMaster;
        public static JobDef ChannelRCT;

        public static HediffDef JJK_KenjakuPossesion;
        public static HediffDef JJ_SummonedCreatureTag;
        public static HediffDef JJK_ThinkTreeOverrideSummonBehaviour;
        public static HediffDef RCTRegenHediff;
        public static HediffDef ZombieWorkSlaveHediff;
        public static HediffDef JJK_IdleTransfigurationCooldown;
        public static HediffDef JJK_IdleTransfigurationBeastStatBoost;


        public static BackstoryDef ZombieChildhoodStory;
        public static BackstoryDef ZombieAdulthoodStory;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        static JJKDefOf()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(JJKDefOf));
        }
    }
}
    

