using Verse;
using Verse.AI;

namespace JJK
{
    public class CompProperties_TenShadowsSummon : CompProperties
    {
        public CompProperties_TenShadowsSummon()
        {
            compClass = typeof(Comp_TenShadowsSummon);
        }
    }

    public class Comp_TenShadowsSummon : ThingComp
    {
        public Pawn Master;
        protected TenShadowGene TenShadowsUser;


        public ShikigamiDef ShikigamiDef;

        public Pawn ParentPawn => this.parent as Pawn;

        public void SetMaster(Pawn NewMaster, ShikigamiDef ShikigamiDef)
        {
            Master = NewMaster;
            TenShadowsUser = Master.GetTenShadowsUser();
            this.ShikigamiDef = ShikigamiDef;
        }

        public override void Notify_Killed(Map prevMap, DamageInfo? dinfo = null)
        {
            if (TenShadowsUser != null)
            {
                TenShadowsUser.OnShikigamiDeath(ParentPawn);
            }

            base.Notify_Killed(prevMap, dinfo);
        }

        public virtual void OnSummon()
        {
            GrantAbilities();
        }

        public virtual void GrantAbilities()
        {
            if (this.ParentPawn != null)
            {
                if (this.ParentPawn.abilities == null)
                {
                    this.ParentPawn.abilities = new RimWorld.Pawn_AbilityTracker(this.ParentPawn);
                }

                foreach (var item in ShikigamiDef.shikigamiAbilities)
                {
                    if (this.ParentPawn.abilities.GetAbility(item) == null)
                    {
                        this.ParentPawn.abilities.GainAbility(item);
                    }
                }
            }
        }


        public virtual void OnTargetSummonAction(Pawn Master, Thing Target)
        {

        }

        public virtual void OnLocationSummonAction(Pawn Master, IntVec3 Target)
        {

        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_References.Look(ref Master, "master");
            Scribe_Defs.Look(ref ShikigamiDef, "shikigamiDef");
        }
    }

}



