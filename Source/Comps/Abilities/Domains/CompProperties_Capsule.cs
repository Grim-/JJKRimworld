using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace JJK
{
    public class CompProperties_Capsule : CompProperties
    {
        public IntVec2 size;

        public CompProperties_Capsule()
        {
            compClass = typeof(CompCapsule);
        }
    }

    public class CompCapsule : ThingComp
    {
        private List<StoredThing> storedThings = new List<StoredThing>();
        private List<StoredRoof> storedRoofs = new List<StoredRoof>();
        public List<StoredThing> StoredThings => storedThings.ToList();
        public List<StoredRoof> StoredRoofs => storedRoofs.ToList();

        public enum CapsuleState
        {
            Unconfigured,
            Packed,
            Unpacked
        }

        private CapsuleState state = CapsuleState.Unconfigured;
        public CapsuleState State => state;

        public CompProperties_Capsule Props => (CompProperties_Capsule)props;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Collections.Look(ref storedThings, "storedThings", LookMode.Deep);
            Scribe_Collections.Look(ref storedRoofs, "storedRoofs", LookMode.Deep);
            Scribe_Values.Look(ref state, "state", CapsuleState.Unconfigured);
        }

        public override void PostDrawExtraSelectionOverlays()
        {
            base.PostDrawExtraSelectionOverlays();
            GenDraw.DrawFieldEdges(GetCenteredRect().EdgeCells.ToList(), Color.red);
        }

        public void TransferData(CompCapsule other)
        {
            this.storedThings = other.storedThings.ToList();
            this.storedRoofs = other.storedRoofs.ToList();
            this.state = other.State;
        }

        public override void Notify_Equipped(Pawn pawn)
        {
            base.Notify_Equipped(pawn);

            if (state == CapsuleState.Unpacked)
            {
                this.Pack();
            }
        }

        public void Pack()
        {
            if (state == CapsuleState.Packed) return;

            Map map = parent.Map;
            CellRect capsuleRect = GetCenteredRect();
            IntVec3 capsuleCenter = capsuleRect.CenterCell;

            if (state == CapsuleState.Unconfigured)
            {
                // Initial configuration
                foreach (IntVec3 cell in capsuleRect)
                {
                    // Store things
                    foreach (Thing thing in cell.GetThingList(map).ToList())
                    {
                        if (thing != parent && ShouldStoreThing(thing))
                        {
                            storedThings.Add(new StoredThing(thing, thing.Position - capsuleCenter));
                        }
                    }

                    // Store roof information
                    RoofDef roofDef = map.roofGrid.RoofAt(cell);
                    if (roofDef != null)
                    {
                        storedRoofs.Add(new StoredRoof(roofDef, cell - capsuleCenter));
                    }
                }
            }

            // Pack all stored things
            foreach (StoredThing storedThing in storedThings)
            {
                if (storedThing.Thing.Spawned)
                {
                    storedThing.Thing.DeSpawn();
                }
            }

            // Remove roofs
            foreach (StoredRoof storedRoof in storedRoofs)
            {
                IntVec3 roofCell = capsuleCenter + storedRoof.RelativePosition;
                map.roofGrid.SetRoof(roofCell, null);
            }

            state = CapsuleState.Packed;
        }

        private bool ShouldStoreThing(Thing thing)
        {
            // Store buildings, floors, and other relevant things
            return thing.def.building != null || thing.def.IsBuildingArtificial || thing is Mineable;
        }

        public void Unpack()
        {
            if (state != CapsuleState.Packed || !CanUnpack()) return;

            Map map = parent.Map;
            IntVec3 capsuleCenter = parent.Position;

            UnpackThings(capsuleCenter, map);
            RebuildRoofs(capsuleCenter, map);

            state = CapsuleState.Unpacked;
        }


        private void UnpackThings(IntVec3 capsuleCenter, Map map)
        {
            // Unpack stored things
            foreach (StoredThing storedThing in storedThings)
            {
                if (!storedThing.Thing.Spawned && !storedThing.Thing.Destroyed)
                {
                    IntVec3 newPos = capsuleCenter + storedThing.RelativePosition;
                    GenSpawn.Spawn(storedThing.Thing, newPos, map, storedThing.Thing.Rotation);
                }
            }
        }
        private void RebuildRoofs(IntVec3 capsuleCenter, Map map)
        {
            // Rebuild roofs
            foreach (StoredRoof storedRoof in storedRoofs)
            {
                IntVec3 roofCell = capsuleCenter + storedRoof.RelativePosition;
                map.roofGrid.SetRoof(roofCell, storedRoof.RoofDef);
            }
        }

        public bool CanUnpack()
        {
            if (state != CapsuleState.Packed) return false;

            Map map = parent.Map;
            IntVec3 capsuleCenter = parent.Position;

            foreach (StoredThing storedThing in storedThings)
            {
                IntVec3 newPos = capsuleCenter + storedThing.RelativePosition;
                if (!newPos.InBounds(map) || newPos.Impassable(map))
                {
                    Messages.Message("Not enough room to unpack", MessageTypeDefOf.NegativeEvent);
                    return false;
                }
            }

            return true;
        }

        public void RemoveDestroyedThings()
        {
            storedThings.RemoveAll(st => st.Thing.Destroyed);
        }

        private CellRect GetCenteredRect()
        {
            return CellRect.CenteredOn(parent.Position, Props.size.x, Props.size.z);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (state == CapsuleState.Packed || state == CapsuleState.Unpacked)
            {
                if (state == CapsuleState.Unpacked)
                {
                    yield return new Command_Action
                    {
                        action = Pack,
                        defaultLabel = "Pack",
                        defaultDesc = "Pack the stored things and roofs into the capsule.",
                        icon = ContentFinder<Texture2D>.Get("UI/Designators/Deconstruct")
                    };
                }
                else if (CanUnpack())
                {
                    yield return new Command_Action
                    {
                        action = Unpack,
                        defaultLabel = "Unpack",
                        defaultDesc = "Unpack the stored things and rebuild roofs.",
                        icon = ContentFinder<Texture2D>.Get("UI/Designators/Deconstruct")
                    };
                }
            }
            else  // CapsuleState.Unconfigured
            {
                yield return new Command_Action
                {
                    action = Pack,
                    defaultLabel = "Configure and Pack",
                    defaultDesc = "Configure the capsule with nearby things and roofs, then pack them.",
                    icon = ContentFinder<Texture2D>.Get("UI/Designators/Deconstruct")
                };
            }
        }
    }

    public struct StoredThing
    {
        public Thing Thing;
        public IntVec3 RelativePosition;

        public StoredThing(Thing thing, IntVec3 relativePosition)
        {
            Thing = thing;
            RelativePosition = relativePosition;
        }
    }

    public struct StoredRoof
    {
        public RoofDef RoofDef;
        public IntVec3 RelativePosition;

        public StoredRoof(RoofDef roofDef, IntVec3 relativePosition)
        {
            RoofDef = roofDef;
            RelativePosition = relativePosition;
        }
    }
}