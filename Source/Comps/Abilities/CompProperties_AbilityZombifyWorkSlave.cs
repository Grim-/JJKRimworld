using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI.Group;

namespace JJK
{
    public class CompProperties_AbilityZombifyWorkSlave : CompProperties_AbilityEffect
    {
        public CompProperties_AbilityZombifyWorkSlave()
        {
            compClass = typeof(CompAbilityEffect_ZombifyWorkSlave);
        }
    }

    public class CompAbilityEffect_ZombifyWorkSlave : CompAbilityEffect
    {
        public new CompProperties_AbilityZombifyWorkSlave Props => (CompProperties_AbilityZombifyWorkSlave)props;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);

            Pawn targetPawn = target.Pawn;
            if (targetPawn != null)
            {
                ZombieUtility.ForceIntoZombieSlave(targetPawn, this.parent.pawn);

                Hediff zombieHediff = HediffMaker.MakeHediff(JJKDefOf.ZombieWorkSlaveHediff, targetPawn);
                targetPawn.health.AddHediff(zombieHediff);


                Hediff addedHeDiff = targetPawn.health.hediffSet.GetFirstHediffOfDef(JJKDefOf.ZombieWorkSlaveHediff);


                if (addedHeDiff != null)
                {
                    // The hediff needs to be added to the pawn before we can access its comps
                    HediffComp_ZombieWorkSlaveEffect zombieWorkSlaveEffect = addedHeDiff.TryGetComp<HediffComp_ZombieWorkSlaveEffect>();
                    if (zombieWorkSlaveEffect != null)
                    {
                        zombieWorkSlaveEffect.SetSlaveMaster(parent.pawn);
                        Log.Message($"Zombify: Master set for {targetPawn.LabelShort} to {parent.pawn.LabelShort}");
                    }
                    else
                    {
                        Log.Error($"Zombify: Failed to get HediffComp_ZombieWorkSlaveEffect for {targetPawn.LabelShort}");
                    }
                }

               
            }
        }
    }

}


