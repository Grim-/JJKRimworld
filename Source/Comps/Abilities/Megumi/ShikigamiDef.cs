using RimWorld;
using System.Collections.Generic;
using Verse;

namespace JJK
{
    public class ShikigamiDef : Def
    {
        public PawnKindDef shikigami;
        public AbilityDef summonAbility;
        public ShikigamiMergeEffectDef mergeEffect;
        public List<AbilityDef> shikigamiAbilities;
    }
}