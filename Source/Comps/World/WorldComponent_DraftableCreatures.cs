using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using Verse;

namespace JJK
{
    public class WorldComponent_DraftableCreatures : WorldComponent
    {
        private HashSet<Pawn> draftableCreatures = new HashSet<Pawn>();

        public WorldComponent_DraftableCreatures(World world) : base(world)
        { 

        }

        public void RegisterDraftableCreature(Pawn pawn)
        {
            if (pawn != null)
            {
                if (draftableCreatures.Contains(pawn))
                {
                    draftableCreatures.Remove(pawn);
                }
                draftableCreatures.Add(pawn);
                EnsureDraftComponents(pawn);
            }
        }

        public void UnregisterDraftableCreature(Pawn pawn)
        {
            if (pawn != null)
            {
                draftableCreatures.Remove(pawn);
            }
        }

        public bool IsDraftableCreature(Pawn pawn)
        {
            return pawn != null && draftableCreatures.Contains(pawn);
        }

        private void EnsureDraftComponents(Pawn pawn)
        {
            if (pawn.drafter == null)
            {
                pawn.drafter = new Pawn_DraftController(pawn);
            }
            if (pawn.equipment == null)
            {
                pawn.equipment = new Pawn_EquipmentTracker(pawn);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref draftableCreatures, "draftableCreatures", LookMode.Reference);
        }
    }
}

    