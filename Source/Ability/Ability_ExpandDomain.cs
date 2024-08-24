using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.AI;

namespace JJK
{
    public class Ability_ExpandDomain : Ability
    {
        protected bool IsChanneling = false;
        protected Thing DomainThing;
        protected CompDomainEffect DomainComp;
        public Ability_ExpandDomain() : base() { }
        public Ability_ExpandDomain(Pawn pawn) : base(pawn) { }
        public Ability_ExpandDomain(Pawn pawn, Precept sourcePrecept) : base(pawn, sourcePrecept) { }
        public Ability_ExpandDomain(Pawn pawn, AbilityDef def) : base(pawn, def) { }
        public Ability_ExpandDomain(Pawn pawn, Precept sourcePrecept, AbilityDef def) : base(pawn, sourcePrecept, def) { }

        public override bool Activate(LocalTargetInfo target, LocalTargetInfo dest)
        {
            if (IsChanneling)
            {
                DestroyActiveDomain();
                return true;
            }
            else
            {
                return ActivateDomain(target.Cell);
            }
        }

        private bool ActivateDomain(IntVec3 cell)
        {
            Log.Message($"Activating ExpandDomain ability for {pawn} at {cell}");
            DomainExpansionDef domainDef = def as DomainExpansionDef;

            if (domainDef == null)
            {
                Log.Error("Invalid DomainExpansionDef");
                return false;
            }

            if (string.IsNullOrEmpty(domainDef.DomainThingDefName))
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
            GenSpawn.Spawn(DomainThing, cell, pawn.Map);

            CompDomainEffect CompDomainEffect = DomainThing.TryGetComp<CompDomainEffect>();

            if (CompDomainEffect == null)
            {
                Log.Error($"Failed to find CompDomainEffect on DomainThing from {domainDef.DomainThingDefName}");
                DomainThing.Destroy();
                return false;
            }

            SetDomainComp(CompDomainEffect);
            DomainComp.ActivateDomain();
            IsChanneling = true;
            return true;
        }

        public void DestroyActiveDomain()
        {
            if (DomainComp == null)
            {
                Log.Error("DeactivateDomain DomainComp is null");
                return;
            }

            if (DomainThing == null)
            {
                Log.Error("DeactivateDomain DomainThing is null");
                return;
            }

            DomainComp.DestroyDomain();
            IsChanneling = false;
        }

        private void SetDomainComp(CompDomainEffect domainEffect)
        {
            DomainComp = domainEffect;
            DomainComp.SetCaster(pawn);
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref IsChanneling, "isChanneling");
            Scribe_References.Look(ref DomainThing, "domainThing");

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (DomainThing != null)
                {
                    CompDomainEffect CompDomainEffect = DomainThing.TryGetComp<CompDomainEffect>();

                    if (CompDomainEffect != null)
                    {
                        SetDomainComp(CompDomainEffect);
                    }
                }             
            }
        }
    }
}