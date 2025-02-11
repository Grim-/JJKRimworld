using RimWorld;
using Verse;

namespace JJK
{
    public class Hediff_Shikigami : HediffWithComps
    {
        private Pawn referencedPawn;
        public Pawn Master => referencedPawn;
        public override string Label => base.Label;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref referencedPawn, "referencedPawn");
            DraftingUtility.MakeDraftable(pawn);

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
                baseString += $"\nMaster Pawn: {referencedPawn.Name}";
            }
            else
            {
                baseString += "\nNo Master";
            }
            return baseString;
        }

        public void SetMaster(Pawn pawn)
        {
            referencedPawn = pawn;
        }

        public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
        {
            Log.Message($"Notify_PawnDied {Master}");

            if (Master != null && Master.IsCursedSpiritManipulator())
            {
                Messages.Message($"{this.pawn.Label} has died, {Master.Label} has lost the ability to summon {this.pawn.Label}.", MessageTypeDefOf.NegativeEvent);
                Master.GetCursedSpiritManipulator().RemoveSummon(this.pawn, true);
            }

            base.Notify_PawnDied(dinfo, culprit);
        }
    }

}


