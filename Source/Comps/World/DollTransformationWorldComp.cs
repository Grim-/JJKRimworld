using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace JJK
{

    public class DollTransformationWorldComp : WorldComponent
    {
        public List<Pawn> transformedPawns = new List<Pawn>();

        public DollTransformationWorldComp(World world) : base(world)
        {

        }

        public override void ExposeData()
        {
            base.ExposeData();
            //Scribe_Collections.Look(ref transformedPawns, "transformedPawns", LookMode.Deep);
        }

        public void StorePawn(Pawn pawn)
        {
            transformedPawns.Add(pawn);
        }

        public void RemovePawn(Pawn pawn)
        {
            transformedPawns.Remove(pawn);
        }

        public Pawn RetrievePawn(int index)
        {
            if (index >= 0 && index < transformedPawns.Count)
            {
                Pawn pawn = transformedPawns[index];
                transformedPawns.RemoveAt(index);
                return pawn;
            }
            return null;
        }
    }

}