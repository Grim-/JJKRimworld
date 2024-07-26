using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace JJK
{
    public class CompProperties_IdleTransfigurationShapeSelf : CompProperties_CursedAbilityProps
    {
        public List<TransfigurationOption> ShapeShiftOptions;

        public CompProperties_IdleTransfigurationShapeSelf()
        {
            compClass = typeof(CompAbilityEffect_IdleTransfigurationShapeSelf);
        }
    }

    public class CompAbilityEffect_IdleTransfigurationShapeSelf : BaseCursedEnergyAbility
    {
        public new CompProperties_IdleTransfigurationShapeSelf Props => (CompProperties_IdleTransfigurationShapeSelf)props;

        private List<BodyPartChange> changedParts = new List<BodyPartChange>();

        public override void ApplyAbility(LocalTargetInfo target, LocalTargetInfo dest)
        {
            if (target.Pawn == null) return;
        }

 
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (Props.ShapeShiftOptions != null && Props.ShapeShiftOptions.Any())
            {
                yield return new Gizmo_MultiOptions(GetAllOptions());
            }
        }

        private List<Gizmo_MultiOptions.Option> GetAllOptions()
        {
            var options = new List<Gizmo_MultiOptions.Option>();
            var groupedOptions = Props.ShapeShiftOptions.GroupBy(o => o.BodyPartDef);

            foreach (var group in groupedOptions)
            {
                options.Add(new Gizmo_MultiOptions.Option(
                    group.Key.label,
                    null, // or appropriate icon if available
                    () => { } // Empty action as this option just shows sub-options
                ));

                foreach (var option in group)
                {
                    options.Add(new Gizmo_MultiOptions.Option(
                        "  " + option.OptionLabel, // Indented to show it's a sub-option
                        null, // or appropriate icon if available
                        () => ApplyShapeShift(parent.pawn, option)
                    ));
                }

                if (changedParts.Any(c => c.BodyPartDef == group.Key))
                {
                    options.Add(new Gizmo_MultiOptions.Option(
                        $"  Revert {group.Key.label}",
                        null, // or appropriate icon if available
                        () => RevertChange(parent.pawn, group.Key)
                    ));
                }
            }

            return options;
        }

        private void ApplyShapeShift(Pawn target, TransfigurationOption option)
        {
            BodyPartRecord targetPart = target.RaceProps.body.GetPartsWithDef(option.BodyPartDef).RandomElementWithFallback();
            if (targetPart != null)
            {
                BodyPartChange change = new BodyPartChange
                {
                    BodyPartDef = option.BodyPartDef,
                    BodyPartIndex = targetPart.Index,
                    OriginalPartDef = targetPart.def,
                    NewHediffDef = option.HediffDef,
                    BodyPart = targetPart
                };

                changedParts.RemoveAll(c => c.BodyPartDef == option.BodyPartDef);
                changedParts.Add(change);

                Hediff newPart = HediffMaker.MakeHediff(option.HediffDef, target, targetPart);
                target.health.AddHediff(newPart, targetPart);


                Messages.Message($"{target.LabelShort} has shape-shifted their {targetPart.Label} into a {option.HediffDef.label}!", MessageTypeDefOf.PositiveEvent);
            }
            else
            {
                Messages.Message($"Failed to find a suitable body part on {target.LabelShort} for shape-shifting.", MessageTypeDefOf.RejectInput);
            }
        }

        private void RevertChange(Pawn target, BodyPartDef bodyPartDef)
        {
            BodyPartChange change = changedParts.FirstOrDefault(c => c.BodyPartDef == bodyPartDef);
            if (change != null && change.BodyPart != null)
            {
                // Remove the transformed part
                Hediff transformedPart = target.health.hediffSet.hediffs
                    .FirstOrDefault(h => h.Part == change.BodyPart && h.def == change.NewHediffDef);
                if (transformedPart != null)
                {
                    target.health.RemoveHediff(transformedPart);
                }

                // Fully heal the body part
                IEnumerable<Hediff> injuries = target.health.hediffSet.hediffs
                    .Where(h => h.Part == change.BodyPart && h.def.injuryProps != null);
                foreach (Hediff injury in injuries.ToList())
                {
                    target.health.RemoveHediff(injury);
                }

                // Restore any missing parts
                target.health.RestorePart(change.BodyPart);

                // If there was an original hediff (like a bionic), restore it
                if (change.OriginalHediffDef != null)
                {
                    Hediff originalHediff = HediffMaker.MakeHediff(change.OriginalHediffDef, target, change.BodyPart);
                    target.health.AddHediff(originalHediff);
                }

                changedParts.Remove(change);
                Messages.Message($"{target.LabelShort} has reverted the changes to their {bodyPartDef.label}!", MessageTypeDefOf.PositiveEvent);
            }
        }

        private void RestoreBodyParts(Pawn pawn)
        {
            foreach (var change in changedParts)
            {
                if (change.BodyPart == null)
                {
                    change.BodyPart = pawn.RaceProps.body.AllParts.FirstOrDefault(p => p.def == change.BodyPartDef && p.Index == change.BodyPartIndex);
                }
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Collections.Look(ref changedParts, "changedParts", LookMode.Deep);

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                RestoreBodyParts(parent.pawn);
            }
        }
    }

    public class BodyPartChange : IExposable
    {
        public BodyPartDef BodyPartDef;
        public int BodyPartIndex;
        public BodyPartDef OriginalPartDef;
        public HediffDef NewHediffDef;
        public HediffDef OriginalHediffDef;
        public BodyPartRecord BodyPart;

        public void ExposeData()
        {
            Scribe_Defs.Look(ref BodyPartDef, "bodyPartDef");
            Scribe_Values.Look(ref BodyPartIndex, "bodyPartIndex");
            Scribe_Defs.Look(ref OriginalPartDef, "originalPartDef");
            Scribe_Defs.Look(ref OriginalHediffDef, "originalHediffDef");
            Scribe_Defs.Look(ref NewHediffDef, "newHediffDef");
            // Note: BodyPart is not saved, it's restored in RestoreBodyParts
        }
    }
}