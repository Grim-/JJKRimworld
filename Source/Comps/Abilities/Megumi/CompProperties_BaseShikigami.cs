using RimWorld;
using Verse;

namespace JJK
{
    public abstract class CompProperties_BaseShikigami : CompProperties_CursedAbilityProps
    {
        public float SummonCost = 20f;
        public EffecterDef SummonEffecter;
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


        public virtual void CreateSummonVFX(IntVec3 Position, Map Map)
        {
            if (Props.SummonEffecter != null)
            {
                Props.SummonEffecter.Spawn(Position, Map);
            }
        }


        public virtual void Summon(IntVec3 Position, Pawn TargetPawn)
        {

        }

        public virtual void UnSummon()
        {

        }
    }


    public class Verb_StealCursedTechnique : Verb_MeleeAttackDamage
    {
        protected override DamageWorker.DamageResult ApplyMeleeDamageToTarget(LocalTargetInfo target)
        {
            // First, apply the normal melee damage
            DamageWorker.DamageResult damageResult = base.ApplyMeleeDamageToTarget(target);

            // Now, let's add our custom behavior
            if (target.Thing is Pawn targetPawn)
            {

                targetPawn.health.GetOrAddHediff(JJKDefOf.JJK_SplitSoul);

                //// Check if the target pawn has any cursed techniques (you'll need to define what this means in your mod)
                //Hediff cursedTechnique = targetPawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.YourCursedTechniqueHediff);
                //if (cursedTechnique != null)
                //{
                //    // Remove the cursed technique from the target
                //    targetPawn.health.RemoveHediff(cursedTechnique);

                //    // Optionally, add the cursed technique to the attacker
                //    if (this.CasterPawn != null)
                //    {
                //        Hediff stolenTechnique = HediffMaker.MakeHediff(HediffDefOf.YourCursedTechniqueHediff, this.CasterPawn);
                //        this.CasterPawn.health.AddHediff(stolenTechnique);

                //        // Display a message
                //        Messages.Message("CursedTechniqueStolen".Translate(this.CasterPawn.LabelShort, targetPawn.LabelShort), MessageTypeDefOf.NeutralEvent);
                //    }
                //}
            }

            return damageResult;
        }
    }
}