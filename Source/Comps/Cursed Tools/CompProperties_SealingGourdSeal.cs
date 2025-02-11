using RimWorld;
using Verse;

namespace JJK
{
    public class CompProperties_SealingGourdSeal : CompProperties_CursedAbilityProps
    {
        public CompProperties_SealingGourdSeal()
        {
            compClass = typeof(CompAbilityEffect_SealingGourdSeal);
        }
    }

    public class CompAbilityEffect_SealingGourdSeal : BaseCursedEnergyAbility
    {
        private CompStoredPawn StorageComp => parent.pawn.equipment.Primary?.GetComp<CompStoredPawn>();

        public override void ApplyAbility(LocalTargetInfo target, LocalTargetInfo dest)
        {
            if (StorageComp == null) return;

            if (StorageComp.HasStoredPawn())
            {
                ReleasePawn();
            }
            else if (target.Pawn != null)
            {
                SealPawn(target.Pawn);
            }
        }

        private void SealPawn(Pawn targetPawn)
        {
            if (StorageComp.HasStoredPawn()) return;

            StorageComp.StorePawn(targetPawn);
            targetPawn.DeSpawn(DestroyMode.Vanish);
            Messages.Message("SealingGourdSealed".Translate(targetPawn.LabelCap), MessageTypeDefOf.PositiveEvent);
        }

        private void ReleasePawn()
        {
            if (!StorageComp.HasStoredPawn()) return;

            Pawn releasedPawn = StorageComp.ReleasePawn();
            GenSpawn.Spawn(releasedPawn, parent.pawn.Position, parent.pawn.Map);
            Messages.Message("SealingGourdReleased".Translate(releasedPawn.LabelCap), MessageTypeDefOf.PositiveEvent);
        }

        public override string ExtraLabelMouseAttachment(LocalTargetInfo target)
        {
            return StorageComp?.HasStoredPawn() == true ?
                "SealingGourdReleaseLabel".Translate() :
                "SealingGourdSealLabel".Translate();
        }
    }
}