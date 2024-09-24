using System.Collections.Generic;
using Verse;

namespace JJK
{
    public class HediffCompProperties_KenjakuPossession : HediffCompProperties
    {
        public HediffCompProperties_KenjakuPossession()
        {
            compClass = typeof(HediffComp_KenjakuPossession);
        }
    }

    public class HediffComp_KenjakuPossession : HediffComp
    {
        private Pawn originalPawn;
        private List<Pawn> possessedPawns = new List<Pawn>();

        public Pawn OriginalPawn => originalPawn;
        public List<Pawn> PossessedPawns => possessedPawns;

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_References.Look(ref originalPawn, "originalPawn");
            Scribe_Collections.Look(ref possessedPawns, "possessedPawns", LookMode.Reference);
        }

        public void InitializeOriginalPawn(Pawn pawn)
        {
            if (originalPawn == null)
            {
                originalPawn = pawn;
            }
        }

        public void AddPossessedPawn(Pawn pawn)
        {
            if (!possessedPawns.Contains(pawn))
            {
                possessedPawns.Add(pawn);
            }
        }

        public override string CompTipStringExtra
        {
            get
            {
                string tip = $"Original form: {originalPawn?.LabelMouseover ?? "Unknown"}\n";
                tip += "Possessed forms:\n";
                foreach (var pawn in possessedPawns)
                {
                    tip += $"- {pawn.LabelShort}\n";
                }
                return tip;
            }
        }
    }
}


