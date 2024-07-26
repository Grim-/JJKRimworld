using Verse;

namespace JJK
{
    public class Hediff_ZombieWorkSlave : HediffWithComps
    {
        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
            HediffComp_ZombieWorkSlaveEffect zombieComp = this.TryGetComp<HediffComp_ZombieWorkSlaveEffect>();
            zombieComp?.CompPostMake();
            
        }
    }



}


