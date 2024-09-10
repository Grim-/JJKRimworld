using RimWorld;
using Verse;

namespace JJK
{
    public class Hediff_CursedObjectConsumer : HediffWithComps
    {
        public Pawn currentOriginPawn;
        public int consumedPieces = 0;
        private int PiecesRequired = 4;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref currentOriginPawn, "currentOriginPawn");
            Scribe_Values.Look(ref consumedPieces, "consumedPieces");
            Scribe_Values.Look(ref PiecesRequired, "piecesRequired");
        }

        public void ProcessConsumedPiece(CompCursedObjectPiece compCursedObjectPiece)
        {
            Log.Message($"ProcessConsumedPiece");

            // Only generate a new pawn if both are null
            if (compCursedObjectPiece.originPawn == null && currentOriginPawn == null)
            {
                Pawn generatedPawn = PawnGenerator.GeneratePawn(PawnKindDefOf.Sanguophage, Faction.OfPlayer);
                compCursedObjectPiece.originPawn = generatedPawn;
                currentOriginPawn = generatedPawn;
            }
            else if (compCursedObjectPiece.originPawn == null)
            {
                compCursedObjectPiece.originPawn = currentOriginPawn;
            }
            else if (currentOriginPawn == null)
            {
                currentOriginPawn = compCursedObjectPiece.originPawn;
            }

            Log.Message($"target pawn : {compCursedObjectPiece.originPawn.Label} existing target if any {currentOriginPawn?.Label}");

            if (currentOriginPawn != compCursedObjectPiece.originPawn)
            {
                // Reset progress if consuming a piece from a different pawn
                ResetProgress();
                currentOriginPawn = compCursedObjectPiece.originPawn;
                PiecesRequired = compCursedObjectPiece.Props.TransformThreshold;
            }

            consumedPieces++;
            Log.Message($"consume count {consumedPieces} required {PiecesRequired}");

            if (consumedPieces >= PiecesRequired)
            {
                CompleteConsumption();
            }
        }

        private void ResetProgress()
        {
            currentOriginPawn = null;
            consumedPieces = 0;
        }

        private void TransferAbilities()
        {
            Messages.Message("Has fully absorbed the thingy, and as such the thingy has happened.", MessageTypeDefOf.PositiveEvent);
            JJKUtility.TryUpgradeSorcererGrade(pawn);
            JJKUtility.TransferAbilities(currentOriginPawn, pawn);

        }

        private void CompleteConsumption()
        {
            Log.Message($"CompleteConsumption called");
            TransferAbilities();
            RemoveSelf();
        }

        private void RemoveSelf()
        {
            pawn?.health?.RemoveHediff(this);
        }
    }
}
