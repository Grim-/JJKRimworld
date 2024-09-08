using RimWorld;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace JJK
{
    public class Hediff_CursedSpiritManipulator : HediffWithComps
    {
        private const int CursedSpiritTypeLimit = 5;
        private List<PawnKindDef> storedCursedSpirits = new List<PawnKindDef>();
        private List<Pawn> activeCursedSpirits = new List<Pawn>();

        public List<PawnKindDef> GetAbsorbedCreatures()
        {
            return storedCursedSpirits;
        }

        public bool IsCreatureActive(PawnKindDef def)
        {
            return activeCursedSpirits.Any(p => p.kindDef == def);
        }

        public void RemoveSummon(Pawn Pawn, bool RemoveStoredKind = true)
        {
            if (activeCursedSpirits.Contains(Pawn))
            {
                activeCursedSpirits.Remove(Pawn);
            }

            if (RemoveStoredKind && storedCursedSpirits.Contains(Pawn.kindDef))
            {
                storedCursedSpirits.Remove(Pawn.kindDef);
            }
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
            if (storedCursedSpirits.Contains(def))
            {
                Pawn summonedPawn = PawnGenerator.GeneratePawn(def, pawn.Faction);
                GenSpawn.Spawn(summonedPawn, pawn.Position, pawn.Map);
                Hediff_Shikigami shikigami = (Hediff_Shikigami)summonedPawn.health.GetOrAddHediff(JJKDefOf.JJK_Shikigami);
                shikigami.SetMaster(pawn);
                if (summonedPawn.abilities == null)
                {
                    summonedPawn.abilities = new Pawn_AbilityTracker(pawn);
                }

                summonedPawn.abilities.GainAbility(JJKDefOf.JJK_CastLightningStrike);
                activeCursedSpirits.Add(summonedPawn);
                return true;
            }
            return false;
        }

        public bool HasAbsorbedCreatureKind(PawnKindDef def)
        {
            return storedCursedSpirits.Contains(def);
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
            return storedCursedSpirits.Count < CursedSpiritTypeLimit || storedCursedSpirits.Contains(def);
        }

        public void AbsorbCreature(PawnKindDef def, Pawn pawn)
        {
            if (CanAbsorbNewSummon(def))
            {
                storedCursedSpirits.Add(def);
            }
        }


        public override string Description => base.Description + $"\r\n This pawn has absorbed {storedCursedSpirits.Count} types of cursed spirits.";

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref storedCursedSpirits, "storedCursedSpirits", LookMode.Def, LookMode.Deep);
            Scribe_Collections.Look(ref activeCursedSpirits, "activeCursedSpirits", LookMode.Reference);
        }
    }
}


