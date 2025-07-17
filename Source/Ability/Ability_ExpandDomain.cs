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
                pawn.jobs.StopAll();
                pawn.jobs.ClearQueuedJobs();
                //DestroyActiveDomain();
                return false;
            }
            else
            {
                ActivateDomain(target.Cell);
            }

            return false;
        }

        public bool ActivateDomain(IntVec3 cell)
        {          
            IsDomainActive = true;
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


            DomainManagerWorldComp domainManagerComp = Find.World.GetComponent<DomainManagerWorldComp>();

            if (domainManagerComp != null)
            { 
                var DomainComp = DomainManagerWorldComp.CreateDomain(domainThingDef, cell, pawn.Map);
                if (DomainComp == null)
                {
                    Log.Error($"Failed to find CompDomainEffect on DomainThing from {domainDef.DomainThingDefName}");
                    return false;
                }

                DomainThing = DomainComp.parent;

                if (domainManagerComp.TryExpandDomain(pawn, cell, DomainComp))
                {
                    SetDomainComp(DomainComp);
                    DomainComp.ActivateDomain();
                    StartDomainChannel();
                    Log.Message($"Domain activated. IsDomainActive: {IsDomainActive}");
                }
            }
            else
            {
                //domain main manger not found
            }
            return true;
        }

       
        private void StartDomainChannel()
        {
            //channelJob.expiryInterval = -1;
            if (pawn.CurJobDef != null && pawn.CurJobDef == JJKDefOf.JJK_ChannelDomain)
            {
                return;
            }

            Job channelJob = JobMaker.MakeJob(JJKDefOf.JJK_ChannelDomain, DomainThing);
            pawn.jobs.StopAll();
            pawn.jobs.ClearQueuedJobs();
            pawn.jobs.StartJob(channelJob, JobCondition.InterruptForced);
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

        //public override Job GetJob(LocalTargetInfo target, LocalTargetInfo destination)
        //{
        //    Job channelJob = JobMaker.MakeJob(JJKDefOf.JJK_ChannelDomain, DomainThing);

        //    JobDriver_ChannelDomain jobDriver = pawn.jobs.curDriver as JobDriver_ChannelDomain;
        //    if (jobDriver != null)
        //    {
        //        jobDriver.SetAbilityReference(this);
        //    }
        //    else
        //    {
        //        Log.Error("Failed to cast job driver to JobDriver_ChannelDomain");
        //    }

        //    return channelJob;
        //}

        public void DestroyActiveDomain()
        {
            IsDomainActive = false;
            if (DomainComp != null)
            {
                DomainComp.DestroyDomain();
            }

            DomainThing = null;
            DomainComp = null;



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

            if (Scribe.mode == LoadSaveMode.PostLoadInit && DomainThing != null)
            {
                CompDomainEffect compDomainEffect = DomainThing.TryGetComp<CompDomainEffect>();
                if (compDomainEffect != null)
                {
                    SetDomainComp(compDomainEffect);
                }

                if (IsDomainActive)
                {
                    //StartDomainChannel();
                }
            }
        }
    }
}