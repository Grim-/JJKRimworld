using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace JJK
{
    public class SummonedCreatureManager : WorldComponent
    {
        private List<SummonPair> summonPairs = new List<SummonPair>();
        public Action<Pawn> OnSummonDied;

        public SummonedCreatureManager(World world) : base(world) { }

        public void RegisterSummon(Pawn summoned, Pawn master)
        {
            SummonPair existingPair = summonPairs.FirstOrDefault(x => x.Summoned == summoned);
            if (existingPair == null)
            {
                summonPairs.Add(new SummonPair(summoned, master));
            }
            else existingPair.Master = master;
        }

        public void UnregisterSummon(Pawn summoned)
        {
            SummonPair summonPair = summonPairs.Find(x => x.Summoned == summoned);
            if (summonPair != null)
            {
                summonPairs.Remove(summonPair);
            }
        }

        public void Notify_SummonDeath(Pawn pawnThatDied)
        {
            if (IsSummonedCreature(pawnThatDied))
            {
                OnSummonDied?.Invoke(pawnThatDied);
                UnregisterSummon(pawnThatDied);
            }
        }

        public Pawn GetMasterFor(Pawn summoned)
        {
            return summonPairs.FirstOrDefault(pair => pair.Summoned == summoned)?.Master;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref summonPairs, "SummonPairs", LookMode.Deep);
        }

        public bool IsSummonedCreature(Pawn pawn)
        {
            return summonPairs.Any(pair => pair.Summoned == pawn);
        }

        public List<Pawn> GetSummonsFor(Pawn master)
        {
            return summonPairs.Where(pair => pair.Master == master).Select(pair => pair.Summoned).ToList();
        }
    }
}

    