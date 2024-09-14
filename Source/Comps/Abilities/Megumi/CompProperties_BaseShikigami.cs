using RimWorld;
using Verse;

namespace JJK
{
    public abstract class CompProperties_BaseShikigami : CompProperties_CursedAbilityProps
    {
        public float SummonCost = 20f;
    }

    public abstract class CompBaseShikigamiSummon : BaseCursedEnergyAbility
    {
        protected Gene_CursedEnergy CursedEnergy => parent.pawn.GetCursedEnergy();
        public new CompProperties_BaseShikigami Props => (CompProperties_BaseShikigami)props;
        public abstract void OnPawnTarget(Pawn Pawn, Map Map);
        public abstract void OnLocationTarget(IntVec3 Position, Map Map);
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
                    if (target.Pawn != null)
                    {
                        OnPawnTarget(target.Pawn, map);
                    }
                    else
                    {
                        OnLocationTarget(target.Cell, map);
                    }

                    CursedEnergy.ConsumeCursedEnergy(Props.SummonCost);
                }
                else
                {
                    Messages.Message("Not enough cursed energy to summon.", MessageTypeDefOf.NegativeEvent, false);
                }
            }
        }

        protected bool ShouldDisableGizmo = false;
        //public override bool GizmoDisabled(out string reason)
        //{
        //    return base.GizmoDisabled(out reason) && ShouldDisableGizmo;
        //}

        public abstract bool HasActive();

        public abstract void DestroyActive();


        public virtual void Summon(IntVec3 Position, Pawn TargetPawn)
        {

        }

        public virtual void UnSummon()
        {

        }
    }
}