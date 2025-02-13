using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace JJK
{
    public class CompProperties_TenShadowsToad : CompProperties_TenShadowsSummon
    {
        public float maxCarryMassCapacity = 90f;
        public float carryHeight = -1f;
        public float carryAngle = 0f;
        public Vector3 eastOffset = new Vector3(0.7f, 0, 0.3f);

        public CompProperties_TenShadowsToad()
        {
            compClass = typeof(Comp_TenShadowsToad);
        }
    }

    public class Comp_TenShadowsToad : Comp_TenShadowsSummon, IThingHolderWithDrawnPawn
    {
        private Pawn carriedPawn;
        private ThingOwner innerContainer;

        public bool IsCarrying => carriedPawn != null;
        public new CompProperties_TenShadowsToad Props => (CompProperties_TenShadowsToad)props;
        public Pawn CarriedPawn => carriedPawn;

        public float HeldPawnDrawPos_Y => parent.Rotation == Rot4.North ? parent.DrawPos.y - 1f : parent.DrawPos.y + 1f;
        public float HeldPawnBodyAngle => Props.carryAngle;
        public PawnPosture HeldPawnPosture => PawnPosture.LayingInBed;

        public override void PostDraw()
        {
            base.PostDraw();
            if (!IsCarrying || carriedPawn == null)
                return;

            Vector3 drawPos = parent.DrawPos;
            Vector3 offset = Vector3.zero;
            if (parent.Rotation == Rot4.East)
            {
                offset = Props.eastOffset;
            }
            else if (parent.Rotation == Rot4.West)
            {
                offset = Props.eastOffset;
                offset.x *= -1f;
            }
            else if (parent.Rotation == Rot4.North)
            {
                offset.z = 1f;
            }
            drawPos += offset;


            PawnRenderUtility.DrawCarriedThing(ParentPawn, drawPos, carriedPawn);   
        }


        public override void PostPostMake()
        {
            base.PostPostMake();
            innerContainer = new ThingOwner<Thing>(this, true, LookMode.Deep);
        }

        public bool CanCarry(Pawn target)
        {
            if (IsCarrying || target == null) return false;
            return target.Dead || target.Downed || target.Faction == this.ParentPawn.Faction;
        }

        public bool TryPickupPawn(Pawn target)
        {
            if (!CanCarry(target)) return false;

            carriedPawn = target;

            if (!carriedPawn.Dead)
            {
                carriedPawn.stances.stunner.StunFor(-1, this.parent, true, true, true);
            }

            if (carriedPawn.Spawned)
            {
                carriedPawn.DeSpawn();
            }

            innerContainer.TryAdd(carriedPawn, 1, false);

            return true;
        }

        public override void OnUnSummon()
        {
            base.OnUnSummon();

            if (IsCarrying)
            {
                Messages.Message($"{this.parent.Label} died and was holding {this.carriedPawn.Label} dropping them.", MessageTypeDefOf.NeutralEvent);
                TryPlacePawn(this.LastPosition, this.parent.Map);
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo gizmo in base.CompGetGizmosExtra())
            {
                yield return gizmo;
            }

            // Only show drop gizmo if we're carrying something
            if (IsCarrying && this.carriedPawn != null && this.parent.Spawned)
            {
                Gizmo carryGizmo = ContainingSelectionUtility.SelectCarriedThingGizmo(this.parent, this.carriedPawn);

                if (carryGizmo != null)
                {
                    yield return carryGizmo;
                }

                yield return new Command_Action
                {
                    defaultLabel = "Drop carried pawn",
                    defaultDesc = "Select a location to drop the carried pawn",
                    icon = TexButton.Drop,
                    action = delegate ()
                    {
                        Find.Targeter.BeginTargeting(new TargetingParameters
                        {
                            canTargetLocations = true,
                            canTargetBuildings = false,
                            canTargetPawns = false,
                            validator = (TargetInfo target) => target.Cell.InBounds(this.ParentPawn.Map)
                        },
                        delegate (LocalTargetInfo target)
                        {
                            TryPlacePawn(target.Cell, this.ParentPawn.Map);
                        }, this.ParentPawn, null, null);
                    }
                };
            }
        }

        public bool TryPlacePawn(IntVec3 dropLocation, Map map)
        {
            if (!IsCarrying) 
                return false;

            if (!dropLocation.InBounds(map))
                return false;

            if (innerContainer.Contains(carriedPawn) && innerContainer.TryDrop(carriedPawn, dropLocation, map, ThingPlaceMode.Near, out var resultingThing))
            {
                if (!carriedPawn.Spawned)
                {
                    GenSpawn.Spawn(carriedPawn, dropLocation, map);
                }

                carriedPawn.stances.stunner.StopStun();
                carriedPawn = null;
                return true;
            }

            return false;
        }
        



        // IThingHolder implementation
        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
        }

        public ThingOwner GetDirectlyHeldThings()
        {
            return innerContainer;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_References.Look(ref carriedPawn, "carriedPawn");
            Scribe_Deep.Look(ref this.innerContainer, "innerContainer", new object[]
            {
                this
            });
        }
    }
}