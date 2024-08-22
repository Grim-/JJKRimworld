using Verse;

namespace JJK
{
    public abstract class ThingCompExt : ThingComp
    {
        public virtual DamageWorker.DamageResult Notify_ApplyMeleeDamageToTarget(LocalTargetInfo target, DamageWorker.DamageResult DamageWorkerResult)
        {
            return DamageWorkerResult;
        }


        public virtual void Notify_EquipOwnerUsedVerb(Pawn pawn, Verb verb)
        {
  
        }

    }
}