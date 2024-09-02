using Verse;

namespace JJK
{
    public class Hediff_Summon : HediffWithComps
    {
        private Pawn referencedPawn;
        public Pawn Master => referencedPawn;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref referencedPawn, "referencedPawn");
        }

        public override void Tick()
        {
            base.Tick();


            if (pawn.IsHashIntervalTick(600))
            {
                if (Master.Dead || Master.Destroyed)
                {
                    if (!pawn.Destroyed)
                    {
                        pawn.Destroy();
                    }
                
                }
            }
        }

        public override string DebugString()
        {
            string baseString = base.DebugString();
            if (referencedPawn != null)
            {
                baseString += $"\nReferenced Pawn: {referencedPawn.Name}";
            }
            else
            {
                baseString += "\nNo Referenced Pawn";
            }
            return baseString;
        }

        public void SetMaster(Pawn pawn)
        {
            referencedPawn = pawn;
        }
    }

}


