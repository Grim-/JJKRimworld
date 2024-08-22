using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace JJK
{
    //public class DomainClashManager : GameComponent
    //{
    //    private List<CompDomainEffect> activeDomains = new List<CompDomainEffect>();

    //    public void RegisterDomain(CompDomainEffect domain)
    //    {
    //        activeDomains.Add(domain);
    //    }

    //    public void DeregisterDomain(CompDomainEffect domain)
    //    {
    //        activeDomains.Remove(domain);
    //    }

    //    public void SimulateDomainClashes()
    //    {
    //        for (int i = 0; i < activeDomains.Count; i++)
    //        {
    //            for (int j = i + 1; j < activeDomains.Count; j++)
    //            {
    //                SimulateClash(activeDomains[i], activeDomains[j]);
    //            }
    //        }
    //    }

    //    private void SimulateClash(CompDomainEffect domain1, CompDomainEffect domain2)
    //    {
    //        var overlapCells = GetOverlappingCells(domain1, domain2);
    //        if (overlapCells.Any())
    //        {
    //            float strength1 = CalculateDomainStrength(domain1);
    //            float strength2 = CalculateDomainStrength(domain2);

    //            CompDomainEffect dominantDomain = strength1 > strength2 ? domain1 : domain2;
    //            CompDomainEffect suppressedDomain = strength1 > strength2 ? domain2 : domain1;

    //            ApplyDomainSuppressionEffects(dominantDomain, suppressedDomain, overlapCells);
    //        }
    //    }

    //    private IEnumerable<IntVec3> GetOverlappingCells(CompDomainEffect domain1, CompDomainEffect domain2)
    //    {
    //        IntVec3 center1 = domain1.parent.Position;
    //        IntVec3 center2 = domain2.parent.Position;
    //        float radius1 = domain1.Props.AreaRadius;
    //        float radius2 = domain2.Props.AreaRadius;

    //        float distanceBetweenCenters = center1.DistanceTo(center2);

    //        // If the domains don't overlap at all, return an empty list
    //        if (distanceBetweenCenters > radius1 + radius2)
    //        {
    //            return Enumerable.Empty<IntVec3>();
    //        }

    //        // Calculate the center and radius of the overlapping area
    //        float overlapCenterX = ((center2.x * radius1) + (center1.x * radius2)) / (radius1 + radius2);
    //        float overlapCenterZ = ((center2.z * radius1) + (center1.z * radius2)) / (radius1 + radius2);
    //        IntVec3 overlapCenter = new IntVec3((int)overlapCenterX, 0, (int)overlapCenterZ);

    //        float overlapRadius = (radius1 + radius2 - distanceBetweenCenters) / 2;

    //        // Generate cells within the overlapping area
    //        return GenRadial.RadialCellsAround(overlapCenter, overlapRadius, true)
    //            .Where(cell => IsInDomain(cell, center1, radius1) && IsInDomain(cell, center2, radius2));
    //    }

    //    private bool IsInDomain(IntVec3 cell, IntVec3 center, float radius)
    //    {
    //        return cell.DistanceTo(center) <= radius;
    //    }

    //    private float CalculateDomainStrength(CompDomainEffect domain)
    //    {
    //        return domain.DomainCaster.GetStatValue(JJKDefOf.JJK_DomainStrength);
    //    }

    //    private void ApplyDomainSuppressionEffects(CompDomainEffect dominant, CompDomainEffect suppressed, IEnumerable<IntVec3> cells)
    //    {
    //        // Implement the effects of domain suppression
    //        // This could reduce the suppressed domain's effectiveness in the overlapping cells
    //    }
    //}
}