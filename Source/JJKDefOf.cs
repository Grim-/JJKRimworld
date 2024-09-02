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

        //stats
        public static StatDef JJK_CursedEnergy;
        public static StatDef JJK_CursedEnergyCost;
        public static StatDef JJK_CursedEnergyRegen;
        public static StatDef JJK_CursedEnergyRegenSpeed;
        public static StatDef JJK_CursedEnergyDamageBonus;
        public static StatDef JJK_DomainStrength;
        public static StatDef JJK_RCTHealingBonus;
        public static StatDef JJK_RCTSpeed;
        public static StatDef JJK_BlackFlashTriggerChance;

        /// <summary>
        /// genes
        /// </summary>
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
        //public static GeneDef Gene_JJKGeto;


        public static ThingDef JJK_DismantleProjectile;
        public static ThingDef JJK_idleTransfigurationDoll;
        public static ThingDef JJK_Flyer;

        public static MentalStateDef TransfiguredState_Murderous;

        public static AbilityDef JJK_KenjakuPosess;
        public static AbilityDef Gojo_HollowPurple;

        public static ThinkTreeDef JJK_EmptyConstantThinkTree;
        public static ThinkTreeDef JJK_SummonedCreature;
        public static ThinkTreeDef ZombieWorkSlave;

        public static TraitDef JJK_SukunaTrait;

        public static JobDef JJK_DefendMaster;
        public static JobDef ChannelRCT;
        public static JobDef JJK_CursedSpeechLure;
        public static JobDef JJK_DemondogAttackAndVanish;



        public static HediffDef JJK_Shikigami;
        public static HediffDef JJK_BlackFlash;
        public static HediffDef JJK_CursedEnergySurge;
        public static HediffDef JJK_CursedReinforcementHediff;
        public static HediffDef JJK_InfiniteDomainComa;
        public static HediffDef JJK_SimpleShadowDomain;
        public static HediffDef JJK_HollowWickerBasket;
        public static HediffDef JJK_KenjakuPossesion;
        public static HediffDef JJ_SummonedCreatureTag;
        public static HediffDef JJK_ThinkTreeOverrideSummonBehaviour;
        public static HediffDef JJK_RCTRegenHediff;
        public static HediffDef JJK_ZombieWorkSlaveHediff;
        public static HediffDef JJK_IdleTransfigurationCooldown;
        public static HediffDef JJK_IdleTransfigurationBeastStatBoost;
        public static HediffDef JJK_CursedTechniqueBurnout;
        public static HediffDef JJK_CursedTechniqueStrain;
        
        public static HediffDef JJK_PlayfulCloudConcussion;

        public static PawnKindDef JJK_DemonDogWhite;
        public static PawnKindDef JJK_DemonDogBlack;

        public static FleckDef JJK_BlackSmoke;
        public static ThingDef JJK_PlayfulCloudKnockbackFlyer;


        public static DamageDef JJK_TwistDamage;
        public static DamageDef JJK_CrushDamage;
        public static DamageDef JJK_Dismantle;

        public static EffecterDef JJK_Deflect;
        public static EffecterDef JJK_aura;
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
    

