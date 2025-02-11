using RimWorld;
using System;
using Verse;
using Verse.AI;

namespace JJK
{
    public class CompProperties_AbilityDemondogSummon : CompProperties_BaseShikigami
    {
        public CompProperties_AbilityDemondogSummon()
        {
            compClass = typeof(CompAbilityEffect_DemondogSummon);
        }
    }

    public class CompAbilityEffect_DemondogSummon : CompBaseShikigamiSummon
    {
        public new CompProperties_AbilityDemondogSummon Props => (CompProperties_AbilityDemondogSummon)props;

        protected override void SummonShikigami(IntVec3 Position, Map Map)
        {
            TenShadowGene gene = TenShadowUser;
            if (gene == null || !gene.CanSummonShikigamiKind(Props.shikigamiDef))
                return;

            if (Props.shikigamiDef is TwinShikigamiDef twinShikigamiDef)
            {
                if (TenShadowUser.ShouldSummonTotalityDivineDog)
                {
                    TenShadowUser.GetOrGenerateShikigami(twinShikigamiDef, twinShikigamiDef.totalityShikigami, this.parent.pawn.Position, this.parent.pawn.Map);
                }
                else
                {
                    TenShadowUser.GetOrGenerateShikigami(twinShikigamiDef, twinShikigamiDef.shikigami, this.parent.pawn.Position, this.parent.pawn.Map);
                    TenShadowUser.GetOrGenerateShikigami(twinShikigamiDef, twinShikigamiDef.twinShikigami, this.parent.pawn.Position, this.parent.pawn.Map);
                }
            }
            else
            {
                TenShadowUser.GetOrGenerateShikigami(Props.shikigamiDef, Props.shikigamiDef.shikigami, this.parent.pawn.Position, this.parent.pawn.Map);
            }
        }
    }
}