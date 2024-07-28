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

        public SummonedCreatureManager(World world) : base(world) { }

        public Action<Pawn> OnSummonDied;

        public void RegisterSummon(Pawn summoned, Pawn master)
        {
            summonPairs.Add(new SummonPair(summoned, master));
        }

        public void UnregisterSummon(Pawn summoned)
        {
            summonPairs.RemoveAll(pair => pair.Summoned == summoned);
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
    }

    public class SummonPair : IExposable
    {
        public Pawn Summoned;
        public Pawn Master;

        public SummonPair() 
        {

        } 
        public SummonPair(Pawn summoned, Pawn master)
        {
            Summoned = summoned;
            Master = master;
        }

        public void ExposeData()
        {
            Scribe_References.Look(ref Summoned, "Summoned");
            Scribe_References.Look(ref Master, "Master");
        }
    }
}

    