using RimWorld;
using Verse;

namespace JJK
{
    public class CompProperties_TenShadowsMaxElephantSummon : CompProperties
    {
        public CompProperties_TenShadowsMaxElephantSummon()
        {
            compClass = typeof(Comp_MaxElephantSummon);
        }
    }

    public class Comp_MaxElephantSummon : Comp_TenShadowsSummon
    {


        public override void Notify_Killed(Map prevMap, DamageInfo? dinfo = null)
        {
            base.Notify_Killed(prevMap, dinfo);
        }

        public override void OnTargetSummonAction(Pawn Master, Thing Target)
        {
            base.OnTargetSummonAction(Master, Target);

            this.parent.Position = Master.Position.RandomAdjacentCell8Way();
            this.ParentPawn.Notify_Teleported();

            if (Target != null)
            {
                Ability ability = this.ParentPawn.abilities.GetAbility(JJKDefOf.JJK_MaxElephant_Deluge);
                if (ability != null)
                {
                    ability.QueueCastingJob(new LocalTargetInfo(Target.Position), new LocalTargetInfo(Target.Position));
                }
            }
        }
    }
}



