using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.AI;

namespace JJK
{
    public class Ability_ExpandDomain : Ability
    {
        public bool IsDomainActive = false;
        public Thing DomainThing;
        public CompDomainEffect DomainComp;

        public Ability_ExpandDomain() : base() { }
        public Ability_ExpandDomain(Pawn pawn) : base(pawn) { }
        public Ability_ExpandDomain(Pawn pawn, Precept sourcePrecept) : base(pawn, sourcePrecept) { }
        public Ability_ExpandDomain(Pawn pawn, AbilityDef def) : base(pawn, def) { }
        public Ability_ExpandDomain(Pawn pawn, Precept sourcePrecept, AbilityDef def) : base(pawn, sourcePrecept, def) { }

        public override bool Activate(LocalTargetInfo target, LocalTargetInfo dest)
        {
            if (IsDomainActive)
            {
                DestroyActiveDomain();
            }
            else
            {
                ActivateDomain(target.Cell);           
            }
            return true;
        }

        private bool ActivateDomain(IntVec3 cell)
        {
            DomainExpansionDef domainDef = def as DomainExpansionDef;
            if (domainDef == null || string.IsNullOrEmpty(domainDef.DomainThingDefName))
            {
                Log.Error("Invalid DomainExpansionDef or missing DomainThingDefName");
                return false;
            }

            ThingDef domainThingDef = DefDatabase<ThingDef>.GetNamed(domainDef.DomainThingDefName);
            if (domainThingDef == null)
            {
                Log.Error($"No ThingDef found with name {domainDef.DomainThingDefName}");
                return false;
            }

            DomainThing = ThingMaker.MakeThing(domainThingDef);
            DomainThing = GenSpawn.Spawn(DomainThing, cell, pawn.Map);
            DomainComp = DomainThing.TryGetComp<CompDomainEffect>();

            if (DomainComp == null)
            {
                Log.Error($"Failed to find CompDomainEffect on DomainThing from {domainDef.DomainThingDefName}");
                DomainThing.Destroy();
                return false;
            }

            SetDomainComp(DomainComp);
            DomainComp.ActivateDomain();

            StartDomainChannel();
            IsDomainActive = true;
            Log.Message($"Domain activated. IsDomainActive: {IsDomainActive}");
            return true;
        }

        private void StartDomainChannel()
        {
            Job channelJob = JobMaker.MakeJob(JJKDefOf.JJK_ChannelDomain, DomainThing);
            channelJob.expiryInterval = 999999;

            if (pawn.jobs.TryTakeOrderedJob(channelJob))
            {
                JobDriver_ChannelDomain jobDriver = pawn.jobs.curDriver as JobDriver_ChannelDomain;
                if (jobDriver != null)
                {
                    jobDriver.SetAbilityReference(this);
                }
                else
                {
                    Log.Error("Failed to cast job driver to JobDriver_ChannelDomain");
                }
            }
        }
        public void DestroyActiveDomain()
        {
            if (DomainComp != null)
            {
                DomainComp.DestroyDomain();
            }

            IsDomainActive = false;
            DomainThing = null;

            // Force end the channeling job
            if (pawn.jobs != null && pawn.jobs.curDriver is JobDriver_ChannelDomain)
            {
                pawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
            }

            Log.Message($"Domain destroyed. IsDomainActive: {IsDomainActive}");
        }

        private void SetDomainComp(CompDomainEffect domainEffect)
        {
            DomainComp = domainEffect;
            DomainComp.SetCaster(pawn);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref IsDomainActive, "isChanneling");
            Scribe_References.Look(ref DomainThing, "domainThing");

            //if (Scribe.mode == LoadSaveMode.PostLoadInit && DomainThing != null)
            //{
            //    CompDomainEffect compDomainEffect = DomainThing.TryGetComp<CompDomainEffect>();
            //    if (compDomainEffect != null)
            //    {
            //        SetDomainComp(compDomainEffect);
            //    }

            //    if (IsDomainActive)
            //    {
            //        StartDomainChannel();
            //    }
            //}
        }
    }
}