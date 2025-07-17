using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace JJK
{
    public class CompProperties_CursedSpeechCrush : CompProperties_CursedAbilityProps
    {
        public float radius = 10f;
        public int maxTargets = 10;
        public float baseDamage = 10f;

        public CompProperties_CursedSpeechCrush()
        {
            compClass = typeof(CompAbilityEffect_CursedSpeechCrush);
        }
    }

    public class CompAbilityEffect_CursedSpeechCrush : BaseCursedEnergyAbility
    {
        public new CompProperties_CursedSpeechCrush Props => (CompProperties_CursedSpeechCrush)props;
        public override void ApplyAbility(LocalTargetInfo target, LocalTargetInfo dest)
        {
            if (target.Cell.IsValid)
            {
                IEnumerable<Pawn> targets = GetEnemyPawnsInRange(target.Cell, parent.pawn.Map, Props.radius);

                foreach (var item in targets)
                {
                    DamageInfo dinfo = new DamageInfo(JJKDefOf.JJK_TwistDamage, Props.baseDamage, 1f, -1f, parent.pawn, JJKUtility.GetRandomLimb(item));
                    item.TakeDamage(dinfo);
                }
            }
        }

        public override void DrawEffectPreview(LocalTargetInfo target)
        {
            base.DrawEffectPreview(target);

            if (target.IsValid)
            {
                GenDraw.DrawRadiusRing(target.Cell, Props.radius);
            }
        }

        private IEnumerable<Pawn> GetEnemyPawnsInRange(IntVec3 center, Map map, float radius)
        {
            return GenRadial.RadialCellsAround(center, radius, true)
                .SelectMany(c => c.GetThingList(map))
                .OfType<Pawn>()
                .Where(p => p.Faction != null && p.Faction != Faction.OfPlayer).ToList();
        }
    }
}