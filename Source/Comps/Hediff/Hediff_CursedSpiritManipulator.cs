using System.Collections.Generic;
using Verse;

namespace JJK
{
    public class Hediff_CursedSpiritManipulator : HediffWithComps
    {
        private const int CursedSpiritTypeLimit = 5;
        private Dictionary<PawnKindDef, Pawn> storedCursedSpirits = new Dictionary<PawnKindDef, Pawn>();
        private List<Pawn> activeCursedSpirits = new List<Pawn>();

        public List<PawnKindDef> GetAbsorbedCreatures()
        {
            return new List<PawnKindDef>(storedCursedSpirits.Keys);
        }

        public bool IsCreatureActive(PawnKindDef def)
        {
            return activeCursedSpirits.Any(p => p.kindDef == def);
        }

        public Pawn GetActiveSummonOfKind(PawnKindDef def)
        {
            return activeCursedSpirits.FirstOrDefault(p => p.kindDef == def);
        }

        public bool UnsummonCreature(Pawn pawn)
        {
            if (activeCursedSpirits.Remove(pawn))
            {
                pawn.Destroy();
                return true;
            }
            return false;
        }

        public bool SummonCreature(PawnKindDef def)
        {
            if (storedCursedSpirits.TryGetValue(def, out Pawn storedPawn))
            {
                Pawn summonedPawn = PawnGenerator.GeneratePawn(def, pawn.Faction);
                GenSpawn.Spawn(summonedPawn, pawn.Position, pawn.Map);
                Hediff_Shikigami shikigami = (Hediff_Shikigami)summonedPawn.health.GetOrAddHediff(JJKDefOf.JJK_Shikigami);
                shikigami.SetMaster(pawn);

                summonedPawn.abilities.GainAbility(JJKDefOf.JJK_CastLightningStrike);
                activeCursedSpirits.Add(summonedPawn);
                return true;
            }
            return false;
        }

        public bool HasAbsorbedCreatureKind(PawnKindDef def)
        {
            return storedCursedSpirits.ContainsKey(def);
        }

        public void DeleteAbsorbedCreature(PawnKindDef def)
        {
            if (storedCursedSpirits.Remove(def))
            {
                Pawn activeSummon = GetActiveSummonOfKind(def);
                if (activeSummon != null)
                {
                    UnsummonCreature(activeSummon);
                }
            }
        }

        public bool CanAbsorbNewSummon(PawnKindDef def)
        {
            return storedCursedSpirits.Count < CursedSpiritTypeLimit || storedCursedSpirits.ContainsKey(def);
        }

        public void AbsorbCreature(PawnKindDef def, Pawn pawn)
        {
            if (CanAbsorbNewSummon(def))
            {
                storedCursedSpirits[def] = pawn;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref storedCursedSpirits, "storedCursedSpirits", LookMode.Def, LookMode.Deep);
            Scribe_Collections.Look(ref activeCursedSpirits, "activeCursedSpirits", LookMode.Reference);
        }
    }
}


