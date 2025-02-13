using RimWorld;
using Verse;
using Verse.AI;

namespace JJK
{
    public class CompProperties_TenShadowsSummon : CompProperties
    {
        public int maintainTicks = 2400;
        public float cursedEnergyMaintainCost = 10f;

        public CompProperties_TenShadowsSummon()
        {
            compClass = typeof(Comp_TenShadowsSummon);
        }
    }

    public class Comp_TenShadowsSummon : ThingComp, IHaveAMaster
    {
        public Pawn Master;
        protected TenShadowGene _TenShadowsUser;
        public TenShadowGene TenShadowsUser
        {
            get
            {
                if (_TenShadowsUser == null)
                {
                    _TenShadowsUser = Master.GetTenShadowsUser();
                }

                return _TenShadowsUser;
            }
        }
        public new CompProperties_TenShadowsSummon Props => (CompProperties_TenShadowsSummon)props;

        public ShikigamiDef ShikigamiDef;

        private ShikigamiData _ShikigamiData;
        public ShikigamiData ShikigamiData
        {
            get => _ShikigamiData;
        }

        public Pawn ParentPawn => this.parent as Pawn;

        Pawn IHaveAMaster.Master => Master;


        public IntVec3 LastPosition;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            DraftingUtility.MakeDraftable(ParentPawn);
        }


        public void SetMaster(Pawn NewMaster, ShikigamiDef ShikigamiDef, ShikigamiData shikigamiData)
        {
            SetMaster(NewMaster);
            SetData(shikigamiData);
            this.ShikigamiDef = ShikigamiDef;
        }

        public void SetMaster(Pawn NewMaster)
        {
            Master = NewMaster;
            if (this.parent.Faction != Master.Faction)
            {
                this.parent.SetFaction(Master.Faction);
            }
        }



        public void SetData(ShikigamiData shikigamiData)
        {
            this._ShikigamiData = shikigamiData;
        }

        public override void CompTick()
        {
            base.CompTick();
            LastPosition = this.parent.Position;
        }


        public override void Notify_Killed(Map prevMap, DamageInfo? dinfo = null)
        {
            OnBeforeDeath(prevMap, dinfo);
            base.Notify_Killed(prevMap, dinfo);
            OnAfterDeath(prevMap, dinfo);
        }


        public virtual void OnBeforeDeath(Map prevMap, DamageInfo? dinfo = null)
        {
            if (_TenShadowsUser != null)
            {
                _TenShadowsUser.OnShikigamiDeath(ParentPawn);
            }
        }
        public virtual void OnAfterDeath(Map prevMap, DamageInfo? dinfo = null)
        {

        }

        public virtual void OnSummon()
        {
            GrantAbilities();
        }
        public virtual void OnUnSummon()
        {
      
        }
        public virtual void GrantAbilities()
        {
            if (this.ParentPawn != null)
            {
                if (this.ParentPawn.abilities == null)
                {
                    this.ParentPawn.abilities = new Pawn_AbilityTracker(this.ParentPawn);
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
            Scribe_Deep.Look(ref _ShikigamiData, "shikigamiData");
        }

    }
}



