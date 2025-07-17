using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using Verse;

namespace JJK
{
    public class DomainManagerWorldComp : WorldComponent
    {
        public List<ActiveDomainData> ActiveDomains = new List<ActiveDomainData>();

        public DomainManagerWorldComp(World world) : base(world)
        {

        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref ActiveDomains, "activeDomains", LookMode.Reference);
        }

        public bool TryExpandDomain(Pawn domainUser, IntVec3 newDomainOrigin, CompDomainEffect domainComp)
        {
            if (HasActiveDomain(domainUser))
            {
                // Cannot have two domains active
                return false;
            }

            float newDomainStrength = GetDomainStrength(domainUser);

            foreach (var activeDomain in ActiveDomains)
            {
                if (DomainsOverlap(newDomainOrigin, domainComp.Props.AreaRadius,
                                   activeDomain.DomainThing.Position, activeDomain.DomainComp.Props.AreaRadius))
                {
                    float existingDomainStrength = GetDomainStrength(activeDomain.DomainUser);

                    if (newDomainStrength <= existingDomainStrength)
                    {
                        // New domain is not strong enough to overtake the existing one
                        return false;
                    }
                    else
                    {
                        // New domain is stronger, remove the weaker one
                       // RemoveActiveDomain(activeDomain.DomainUser);
                    }
                }
            }

            // If we've made it here, the new domain can be added
            AddActiveDomain(domainUser, domainComp);
            return true;
        }

        private float GetDomainStrength(Pawn pawn)
        {
            return pawn.GetStatValue(JJKDefOf.JJK_CursedEnergy);
        }

        private bool DomainsOverlap(IntVec3 origin1, float radius1, IntVec3 origin2, float radius2)
        {
            float distanceBetweenCenters = (origin1 - origin2).LengthHorizontal;
            return distanceBetweenCenters < (radius1 + radius2);
        }

        public void AddActiveDomain(Pawn domainUser, CompDomainEffect domainComp)
        {
            ActiveDomains.Add(new ActiveDomainData()
            {
                DomainUser = domainUser,
                DomainThing = domainComp.parent,
            });
        }

        public void RemoveActiveDomain(Pawn domainUser)
        {
            ActiveDomainData activeDomainData = GetActiveDomain(domainUser);
            if (activeDomainData != null)
            {
                ActiveDomains.Remove(activeDomainData);
            }
        }

        public bool HasActiveDomain(Pawn domainUser)
        {
            return GetActiveDomain(domainUser) != null;
        }

        public ActiveDomainData GetActiveDomain(Pawn domainUser)
        {
            return ActiveDomains.Find(x => x.DomainUser == domainUser);
        }


        public static CompDomainEffect CreateDomain(ThingDef DomainDef, IntVec3 Origin, Map Map)
        {
            if (DomainDef == null)
            {
                return null;
            }

            if (Map == null)
            {
                return null;
            }

            ThingWithComps DomainThing = (ThingWithComps)ThingMaker.MakeThing(DomainDef);
            DomainThing = (ThingWithComps)GenSpawn.Spawn(DomainThing, Origin, Map);

            if (DomainThing.TryGetComp(out CompDomainEffect DomainComp))
            {
                return DomainComp;
            }
            else
            {
                Log.Error($"Failed to find CompDomainEffect on DomainThing from Def : [{DomainDef.defName}]");
                DomainThing.Destroy();
                return null;
            }
        }

    }


    public class ActiveDomainData : IExposable
    {
        public Pawn DomainUser;
        public Thing DomainThing;
        public CompDomainEffect DomainComp => DomainThing.TryGetComp<CompDomainEffect>();

        public void ExposeData()
        {
            Scribe_References.Look(ref DomainUser, "domainUser");
            Scribe_References.Look(ref DomainThing, "domainThing");
        }
    }
}