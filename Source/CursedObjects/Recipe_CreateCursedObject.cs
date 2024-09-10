using RimWorld;
using System.Collections.Generic;
using Verse;

namespace JJK
{
    public class Recipe_CreateCursedObject : RecipeWorker
    {
        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            for (int i = 0; i < 5; i++)
            {
                Thing piece = ThingMaker.MakeThing(JJKDefOf.JJK_CursedObjectPiece);

                CompCursedObjectPiece pieceComp = piece.TryGetComp<CompCursedObjectPiece>();
                if (pieceComp != null)
                {
                    pieceComp.originPawn = pawn;
                }

                GenSpawn.Spawn(piece, pawn.Position, pawn.Map);
            }

            if (EffecterDefOf.MeatExplosion != null)
            {
                EffecterDefOf.MeatExplosion.Spawn(pawn, pawn.MapHeld);
            }

            pawn.Destroy();
        }

        public override bool AvailableOnNow(Thing thing, BodyPartRecord part = null)
        {
            return thing is Pawn pawn && pawn.GetCursedEnergy() != null;
        }

        public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
        {
            return new List<BodyPartRecord>()
            {
                { null }
            };
        }
    }
}
