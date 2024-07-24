using RimWorld.Planet;
using System;
using System.Collections.Generic;
using Verse;

namespace JJK
{
    public class SummonedCreatureManager : WorldComponent
    {
        private Dictionary<Pawn, Pawn> summonedCreatures = new Dictionary<Pawn, Pawn>();

        public SummonedCreatureManager(World world) : base(world) { }


        public Action<Pawn> OnSummonDied;

        public void RegisterSummon(Pawn summoned, Pawn master)
        {
            summonedCreatures[summoned] = master;
        }

        public void UnregisterSummon(Pawn summoned)
        {
            summonedCreatures.Remove(summoned);
        }

        public void Notify_SummonDeath(Pawn PawnThatDied)
        {

        }

        public Pawn GetMaster(Pawn summoned)
        {
            if (summonedCreatures.TryGetValue(summoned, out Pawn masterID))
            {
                return masterID;
            }
            return null;
        }

        public override void ExposeData()
        {
            Scribe_Collections.Look(ref summonedCreatures, "summonedCreatures", LookMode.Value, LookMode.Value);
        }

        internal bool IsSummonedCreature(Pawn pawn)
        {
            return summonedCreatures.ContainsKey(pawn);
        }
    }
}

    