using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace JJK
{
    public class CompProperties_IdleTransfigurationShapeSelf : CompProperties_CursedAbilityProps
    {
        public int MaintainCostTicks = 2500;
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


        protected int CurrentTick = 0;


        public override void ApplyAbility(LocalTargetInfo target, LocalTargetInfo dest)
        {
            if (target.Pawn == null) return;
        }

        public override void CompTick()
        {
            base.CompTick();


            CurrentTick++;

            if (CurrentTick >= Props.MaintainCostTicks)
            {

                RemoveMaintenanceCosts();
                CurrentTick = 0;
            }
        }


        private void RemoveMaintenanceCosts()
        {
            float TotalRequired = 0;
            float CurrentCE = parent.pawn.GetCursedEnergy().Value;

            foreach (var Item in changedParts)
            {
                TotalRequired += Item.MaintainCost;
            }

            if (CurrentCE >= TotalRequired)
            {
                parent.pawn.GetCursedEnergy().ConsumeCursedEnergy(TotalRequired);
            }
            else
            {
                // Not enough Cursed Energy, start reverting changes
                float RemainingCE = CurrentCE;
                List<BodyPartChange> PartsToRevert = new List<BodyPartChange>();

                foreach (var Item in changedParts)
                {
                    if (RemainingCE >= Item.MaintainCost)
                    {
                        RemainingCE -= Item.MaintainCost;
                    }
                    else
                    {
                        PartsToRevert.Add(Item);
                    }
                }

                foreach (var PartToRevert in PartsToRevert)
                {
                    RevertChange(parent.pawn, PartToRevert.BodyPartDef);
                    Messages.Message($"Reverting {PartToRevert.BodyPartDef.LabelShort} not enough cursed energy to maintain.", MessageTypeDefOf.NegativeEvent);
                }

                parent.pawn.GetCursedEnergy().ConsumeCursedEnergy(RemainingCE);
            }
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
            var Options = new List<Gizmo_MultiOption>();
            var GroupedOptions = Props.ShapeShiftOptions.GroupBy(o => o.BodyPartDef);
            var ChangedParts = new HashSet<BodyPartDef>();

            //show button revert any changed parts
            foreach (var change in changedParts)
            {
                BodyPartDef PartDef = change.BodyPartDef;
                if (!ChangedParts.Contains(PartDef))
                {
                    ChangedParts.Add(PartDef);
                    Options.Add(new Gizmo_MultiOption(
                        $"Revert {PartDef.label} to original",
                        null,
                        () => RevertChange(parent.pawn, PartDef)
                    ));
                }
            }

            // now list all transformations that dont use a changed part
            foreach (var group in GroupedOptions)
            {
                BodyPartDef PartDef = group.Key;
                BodyPartRecord PartRecord = parent.pawn.RaceProps.body.GetPartsWithDef(PartDef).FirstOrDefault();
                if (PartRecord != null && IsHandOrFinger(PartRecord))
                {
                    PartRecord = GetArm(PartRecord);
                    PartDef = PartRecord.def;
                }

                if (!ChangedParts.Contains(PartDef))
                {
                    foreach (var option in group)
                    {
                        Options.Add(new Gizmo_MultiOption(
                            $"{option.OptionLabel}",
                            null,
                            () => ApplyShapeShift(parent.pawn, option)
                        ));
                    }
                }
            }

            return Options;
        }

        private void ApplyShapeShift(Pawn target, TransfigurationOption option)
        {
            List<BodyPartRecord> targetParts = GetMirroredBodyParts(target, option.BodyPartDef);
            if (targetParts.Any())
            {
                foreach (var targetPart in targetParts)
                {
                    BodyPartRecord partToChange = IsHandOrFinger(targetPart) ? GetArm(targetPart) : targetPart;

                    RevertParentPartIfTransformed(target, partToChange);

                    BodyPartChange change = new BodyPartChange
                    {
                        BodyPartDef = partToChange.def,
                        BodyPartIndex = partToChange.Index,
                        OriginalPartDef = partToChange.def,
                        NewHediffDef = option.HediffDef,
                        BodyPart = partToChange,
                        MaintainCost = option.CursedEnergyMaintainCost
                    };

                    if (IsPartChanged(target, partToChange.def))
                    {
                        RevertChange(target, partToChange.def);
                    }

                    changedParts.RemoveAll(c => c.BodyPart == partToChange);
                    changedParts.Add(change);

                    Hediff newPart = HediffMaker.MakeHediff(option.HediffDef, target, partToChange);
                    target.health.AddHediff(newPart, partToChange);
                }

                parent.pawn.GetCursedEnergy()?.ConsumeCursedEnergy(Props.cursedEnergyCost);
                string partLabel = targetParts.Count > 1 ? targetParts[0].def.label : targetParts[0].LabelShort;
                Messages.Message($"{target.LabelShort} has shape-shifted their {partLabel} into {option.HediffDef.label}!", MessageTypeDefOf.PositiveEvent);
            }
            else
            {
                Messages.Message($"Failed to find suitable body parts on {target.LabelShort} for shape-shifting.", MessageTypeDefOf.RejectInput);
            }
        }

        private void RevertChange(Pawn Target, BodyPartDef BodyPartDef)
        {
            List<BodyPartChange> ChangesToRevert = changedParts.Where(c => c.BodyPartDef == BodyPartDef).ToList();
            foreach (var Change in ChangesToRevert)
            {
                // Restore the default limb
                BodyPartRecord defaultPart = Target.def.race.body.GetPartsWithDef(Change.OriginalPartDef).FirstOrDefault(p => p.Index == Change.BodyPartIndex);
                if (defaultPart != null)
                {
                    Target.health.RestorePart(defaultPart);
                }
                else
                {
                    Log.Error($"Failed to find default body part for {Change.OriginalPartDef.defName} at index {Change.BodyPartIndex}");
                }

                // Recursively revert child parts
                RevertChildParts(Target, Change.BodyPart);
            }

            changedParts.RemoveAll(c => ChangesToRevert.Contains(c));

            if (ChangesToRevert.Any())
            {
                string PartLabel = BodyPartDef.label;
                Messages.Message($"{Target.LabelShort} has reverted the changes to their {PartLabel}!", MessageTypeDefOf.PositiveEvent);
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
        private List<BodyPartRecord> GetMirroredBodyParts(Pawn pawn, BodyPartDef bodyPartDef)
        {
            List<BodyPartRecord> parts = pawn.RaceProps.body.GetPartsWithDef(bodyPartDef).ToList();

            if (!bodyPartDef.IsMirroredPart || parts.Count <= 1)
            {
                return parts;
            }

            // For mirrored parts, we need to find the counterpart
            List<BodyPartRecord> mirroredParts = new List<BodyPartRecord>();
            foreach (var part in parts)
            {
                mirroredParts.Add(part);

                // Find the counterpart (if not already added)
                var counterpart = parts.FirstOrDefault(p => p != part && p.height == part.height&& p.depth== part.depth);
                if (counterpart != null && !mirroredParts.Contains(counterpart))
                {
                    mirroredParts.Add(counterpart);
                }
            }

            return mirroredParts;
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
        private bool IsHandOrFinger(BodyPartRecord part)
        {
            return part.def.defName.ToLower().Contains("hand") || part.def.defName.ToLower().Contains("finger");
        }

        private BodyPartRecord GetArm(BodyPartRecord part)
        {
            while (part != null && !part.def.defName.ToLower().Contains("arm"))
            {
                part = part.parent;
            }
            return part ?? throw new System.InvalidOperationException("Could not find corresponding arm for hand/finger.");
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