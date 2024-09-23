using Verse;

namespace JJK
{
    public class Hediff_Shikigami : HediffWithComps
    {
        private Pawn referencedPawn;
        public Pawn Master => referencedPawn;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref referencedPawn, "referencedPawn");
            JJKUtility.MakeDraftable(pawn);

        }

        public override void PostMake()
        {
            base.PostMake();
        }


        public override void PostTick()
        {
            base.PostTick();

            if (referencedPawn != null && referencedPawn.Dead)
            {
                this.pawn.Kill(null);
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


