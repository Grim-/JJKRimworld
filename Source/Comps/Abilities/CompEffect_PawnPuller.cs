using RimWorld;
using System.Collections.Generic;
using Verse;

namespace JJK
{
    internal class CompEffect_PawnPuller : ThingComp
    {
        private CompProperties_PawnPuller Props => (CompProperties_PawnPuller)props;

        // Timer for periodic pulling
        private int ticksUntilPull = 0;
        private const int PullInterval = 600; // Adjust this value as needed for desired frequency
        private const int PullRadius = 10; // Adjust this value to set the radius of pulling effect

        public override void CompTick()
        {
            base.CompTick();
            if (ticksUntilPull <= 0)
            {
                PullPawnsTowardsCenter();
                ticksUntilPull = PullInterval;
            }
            else
            {
                ticksUntilPull--;
            }
        }

        private void PullPawnsTowardsCenter()
        {
            // Retrieve all pawns in the map
            Map map = parent.Map;
            List<Pawn> pawns = (List<Pawn>)map.mapPawns.AllPawnsSpawned;

            // Create a list to store pawns to be pulled
            List<Pawn> pawnsToPull = new List<Pawn>();

            // Iterate through pawns
            foreach (Pawn pawn in pawns)
            {
                // Check if the pawn has the specific gene
                bool isBlueUser = pawn.IsLimitlessUser();
                // Skip pulling pawns that are "blue users"
                if (isBlueUser)
                {
                    continue;
                }

                // Check if the pawn is within the pull radius
                if ((pawn.Position - parent.Position).LengthHorizontalSquared <= PullRadius * PullRadius)
                {
                    // Add pawn to the list of pawns to be pulled
                    pawnsToPull.Add(pawn);
                }
            }

            // Iterate through pawns to be pulled and apply pulling effect
            foreach (Pawn pawn in pawnsToPull)
            {
                // Calculate direction towards the center (position of the thing)
                IntVec3 direction = parent.Position - pawn.Position;

                // Apply pulling effect (move pawn towards the center)
                IntVec3 destination = pawn.Position + direction;

                // Spawn the flyer
                PawnFlyer pawnFlyer = PawnFlyer.MakeFlyer(JJKDefOf.JJK_Flyer, pawn, destination, null, null);
                GenSpawn.Spawn(pawnFlyer, destination, map);
            }
        }
    }

    public class CompProperties_PawnPuller : CompProperties
    {
        public CompProperties_PawnPuller()
        {
            compClass = typeof(CompEffect_PawnPuller);
        }
    }
}