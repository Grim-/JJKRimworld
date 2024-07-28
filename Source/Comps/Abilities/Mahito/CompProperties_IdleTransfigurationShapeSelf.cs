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
                yield return new Gizmo_MultiLabelButton(GetAllOptions());
            }
        }

        private List<Gizmo_MultiOption> GetAllOptions()
        {
            var options = new List<Gizmo_MultiOption>();
            var groupedOptions = Props.ShapeShiftOptions.GroupBy(o => o.BodyPartDef);

            foreach (var group in groupedOptions)
            {
                bool isPartChanged = IsPartChanged(parent.pawn, group.Key);

                if (isPartChanged)
                {
                    // Add revert option
                    options.Add(new Gizmo_MultiOption(
                        "  Revert to original",
                        null,
                        () => RevertChange(parent.pawn, group.Key)
                    ));
                }
                else
                {
                    // Add shape-shift options
                    foreach (var option in group)
                    {
                        options.Add(new Gizmo_MultiOption(
                            "  " + option.OptionLabel,
                            null,
                            () => ApplyShapeShift(parent.pawn, option)
                        ));
                    }
                }
            }

            return options;
        }

        private void ApplyShapeShift(Pawn target, TransfigurationOption option)
        {
            BodyPartRecord targetPart = target.RaceProps.body.GetPartsWithDef(option.BodyPartDef).RandomElementWithFallback();
            if (targetPart != null)
            {
                // Check if parent part is transformed and revert if necessary
                RevertParentPartIfTransformed(target, targetPart);

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

                if (targetPart.)
                {

                }

                parent.pawn.GetCursedEnergy()?.ConsumeCursedEnergy(Props.cursedEnergyCost);
                Messages.Message($"{target.LabelShort} has shape-shifted their {targetPart.Label} into a {option.HediffDef.label}!", MessageTypeDefOf.PositiveEvent);
            }
            else
            {
                Messages.Message($"Failed to find a suitable body part on {target.LabelShort} for shape-shifting.", MessageTypeDefOf.RejectInput);
            }
        }

        private void RevertParentPartIfTransformed(Pawn target, BodyPartRecord part)
        {
            if (part.parent != null)
            {
                BodyPartChange parentChange = changedParts.FirstOrDefault(c => c.BodyPart == part.parent);
                if (parentChange != null)
                {
                    RevertChange(target, parentChange.BodyPartDef);
                }
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

                // Remove any injuries on the part
                IEnumerable<Hediff> injuries = target.health.hediffSet.hediffs
                    .Where(h => h.Part == change.BodyPart && h.def.injuryProps != null);
                foreach (Hediff injury in injuries.ToList())
                {
                    target.health.RemoveHediff(injury);
                }

                // Restore the original part
                target.health.RestorePart(change.BodyPart);

                // Restore original hediffs if any
                if (change.OriginalHediffDef != null)
                {
                    RestoreOriginalHediffs(target, change);
                }

                changedParts.Remove(change);
                Messages.Message($"{target.LabelShort} has reverted the changes to their {bodyPartDef.label}!", MessageTypeDefOf.PositiveEvent);

                // Recursively revert child parts
                RevertChildParts(target, change.BodyPart);
            }
        }

        private void RevertChildParts(Pawn target, BodyPartRecord part)
        {
            foreach (BodyPartRecord childPart in part.parts)
            {
                BodyPartChange childChange = changedParts.FirstOrDefault(c => c.BodyPart == childPart);
                if (childChange != null)
                {
                    RevertChange(target, childChange.BodyPartDef);
                }
            }
        }

        private bool IsPartChanged(Pawn target, BodyPartDef BodyPart)
        {
            return changedParts.Find(x => x.BodyPartDef == BodyPart) != null;
        }

        //private void RevertChange(Pawn target, BodyPartDef bodyPartDef)
        //{
        //    BodyPartChange change = changedParts.FirstOrDefault(c => c.BodyPartDef == bodyPartDef);
        //    if (change != null && change.BodyPart != null)
        //    {
        //        // Remove the transformed part
        //        Hediff transformedPart = target.health.hediffSet.hediffs
        //            .FirstOrDefault(h => h.Part == change.BodyPart && h.def == change.NewHediffDef);
        //        if (transformedPart != null)
        //        {
        //            target.health.RemoveHediff(transformedPart);
        //        }

        //        IEnumerable<Hediff> injuries = target.health.hediffSet.hediffs
        //            .Where(h => h.Part == change.BodyPart && h.def.injuryProps != null);
        //        foreach (Hediff injury in injuries.ToList())
        //        {
        //            target.health.RemoveHediff(injury);
        //        }

    
        //        target.health.RestorePart(change.BodyPart);

    
        //        if (change.OriginalHediffDef != null)
        //        {
        //            RestoreOriginalHediffs(target, change);
        //        }

        //        changedParts.Remove(change);
        //        Messages.Message($"{target.LabelShort} has reverted the changes to their {bodyPartDef.label}!", MessageTypeDefOf.PositiveEvent);
        //    }
        //}

        private void RestoreOriginalHediffs(Pawn target, BodyPartChange change)
        {
            Hediff originalHediff = HediffMaker.MakeHediff(change.OriginalHediffDef, target, change.BodyPart);
            target.health.AddHediff(originalHediff);
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
}