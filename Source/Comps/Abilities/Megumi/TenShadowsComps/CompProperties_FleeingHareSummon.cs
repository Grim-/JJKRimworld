using Verse;

namespace JJK
{
    public class CompProperties_FleeingHareSummon : CompProperties
    {
        public int copyAmount = 20;
        public float regenCost = 10f;

        public CompProperties_FleeingHareSummon()
        {
            compClass = typeof(Comp_FleeingHareSummon);
        }
    }

    public class Comp_FleeingHareSummon : Comp_TenShadowsSummon
    {
        public Pawn MainFleeingHare;

        new private CompProperties_FleeingHareSummon Props => (CompProperties_FleeingHareSummon)props;


        public void SetMainFleeingHare(Pawn Pawn)
        {
            MainFleeingHare = Pawn;
        }

        public override void Notify_Killed(Map prevMap, DamageInfo? dinfo = null)
        {
            IntVec3 deathPosition = this.parent.Position;
            if (TenShadowsUser != null)
            {
                if (MainFleeingHare != null && this.parent == MainFleeingHare)
                {
                    TenShadowsUser.UnsummonShikigami(this.ShikigamiDef);
                }
                else
                {
                    TenShadowsUser.OnShikigamiDeath(ParentPawn);
                    if (TenShadowsUser.GetActiveSummonsOfKind(ShikigamiDef).Count < Props.copyAmount && TenShadowsUser.CursedEnergy.HasCursedEnergy(Props.regenCost))
                    {
                        TenShadowsUser.GetOrGenerateShikigami(ShikigamiDef, ShikigamiDef.shikigami, deathPosition, prevMap, true);
                    }
                }
            }

            base.Notify_Killed(prevMap, dinfo);
        }
    }
}



