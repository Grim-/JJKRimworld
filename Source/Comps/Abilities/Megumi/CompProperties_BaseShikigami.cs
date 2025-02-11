using RimWorld;
using Verse;

namespace JJK
{
    public class CompProperties_BaseShikigami : CompProperties_CursedAbilityProps
    {
        public float SummonCost = 20f;
        public EffecterDef SummonEffecter;
        public ShikigamiDef shikigamiDef;

        public CompProperties_BaseShikigami()
        {
            compClass = typeof(CompBaseShikigamiSummon);
        }
    }

    public class CompBaseShikigamiSummon : BaseCursedEnergyAbility
    {
        protected Gene_CursedEnergy CursedEnergy => parent.pawn.GetCursedEnergy();
        public new CompProperties_BaseShikigami Props => (CompProperties_BaseShikigami)props;

        protected TenShadowGene TenShadowUser => parent.pawn.GetTenShadowsUser();
        protected bool ShouldDisableGizmo = false;

        public virtual void OnPawnTarget(Pawn Pawn, Map Map)
        {
            SummonShikigami(Pawn.Position, Map);

            var shikigamis = TenShadowUser.GetActiveSummonsOfKind(Props.shikigamiDef);
            if (shikigamis != null && shikigamis.Count > 0)
            {
                foreach (var shikigami in shikigamis)
                {
                    StartOnPawnTargetAction(shikigami, Pawn, Map);
                }
            }
        }

        public virtual void OnLocationTarget(IntVec3 Position, Map Map)
        {
            SummonShikigami(Position, Map);

            var shikigamis = TenShadowUser.GetActiveSummonsOfKind(Props.shikigamiDef);
            if (shikigamis != null && shikigamis.Count > 0)
            {
                foreach (var shikigami in shikigamis)
                {
                    StartOnLocationTargetAction(shikigami, Position, Map);
                }
            }
        }

        protected virtual void SummonShikigami(IntVec3 Position, Map Map)
        {
           TenShadowUser.GetOrGenerateShikigami(Props.shikigamiDef, Props.shikigamiDef.shikigami, Position, Map);
        }

        protected virtual void StartOnPawnTargetAction(Pawn shikigami, Pawn TargetPawn, Map Map)
        {
            Comp_TenShadowsSummon shadowsSummon = shikigami.GetComp<Comp_TenShadowsSummon>();

            if (shadowsSummon != null)
            {
                shadowsSummon.OnTargetSummonAction(TenShadowUser.pawn, TargetPawn);
            }
        }
        protected virtual void StartOnLocationTargetAction(Pawn shikigami, IntVec3 Position, Map Map)
        {
            Comp_TenShadowsSummon shadowsSummon = shikigami.GetComp<Comp_TenShadowsSummon>();

            if (shadowsSummon != null)
            {
                shadowsSummon.OnLocationSummonAction(TenShadowUser.pawn, Position);
            }
        }

        public override void ApplyAbility(LocalTargetInfo target, LocalTargetInfo dest)
        {
            if (HasActive())
            {
                DestroyActive();
            }
            else
            {
                Map map = parent.pawn.Map;

                if (CursedEnergy.HasCursedEnergy(Props.SummonCost))
                {
                    CursedEnergy.ConsumeCursedEnergy(Props.SummonCost);

                    if (target.Pawn != null)
                    {
                        OnPawnTarget(target.Pawn, map);
                    }
                    else
                    {
                        OnLocationTarget(target.Cell, map);
                    }
                }
                else
                {
                    Messages.Message("Not enough cursed energy to summon.", MessageTypeDefOf.NegativeEvent, false);
                }
            }
        }

        public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
        {
            return HasActive() || base.Valid(target, throwMessages);
        }
        public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
        {
            return HasActive() || base.CanApplyOn(target, dest);
        }

        public virtual bool HasActive()
        {
            return TenShadowUser.GetActiveSummonsOfKind(Props.shikigamiDef) != null && TenShadowUser.GetActiveSummonsOfKind(Props.shikigamiDef).Count > 0;
        }

        public virtual void DestroyActive()
        {
            TenShadowUser.UnsummonShikigami(Props.shikigamiDef);
        }

        public virtual void CreateSummonVFX(IntVec3 Position, Map Map)
        {
            if (Props.SummonEffecter != null)
            {
                Props.SummonEffecter.Spawn(Position, Map);
            }
        }

    }

}