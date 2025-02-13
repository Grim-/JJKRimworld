using RimWorld;
using System.Linq;
using UnityEngine;
using Verse;

namespace JJK
{

    public class CompProperties_AbilityPullTarget : CompProperties_AbilityEffect
    {
        public float flightDuration = 0.5f;
        public CompProperties_AbilityPullTarget()
        {
            compClass = typeof(CompAbilityEffect_PullTarget);
        }
    }

    public class CompAbilityEffect_PullTarget : CompAbilityEffect
    {
        public new CompProperties_AbilityPullTarget Props => (CompProperties_AbilityPullTarget)props;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            Pawn pawn = parent.pawn;
            Map map = pawn.MapHeld;
             
            IntVec3 targetCell = pawn.Position.RandomAdjacentCell8Way();

            if (!CanHitTargetSafely(pawn, map, targetCell))
            {
                Messages.Message("JJK_JumpFailed".Translate(), MessageTypeDefOf.RejectInput, false);
                return;
            }
            PawnFlyer pawnFlyer = PawnFlyer.MakeFlyer(JJKDefOf.JJK_GenericFlyer, target.Pawn, targetCell, null, null);
            GenSpawn.Spawn(pawnFlyer, target.Pawn.Position, map);
        }


        private bool CanHitTargetSafely(Pawn Caster, Map map, IntVec3 targetCell)
        {
            if (!targetCell.InBounds(map) || targetCell == Caster.Position || targetCell.Roofed(map) || !targetCell.WalkableByAny(map))
                return false;

            return true;
        }
    }

    public class CompProperties_AbilityJumper : CompProperties_AbilityEffect
    {
        public float flightDuration = 0.5f;
        public CompProperties_AbilityJumper()
        {
            compClass = typeof(CompAbilityEffect_Jump);
        }
    }

    public class CompAbilityEffect_Jump : CompAbilityEffect
    {
        public new CompProperties_AbilityJumper Props => (CompProperties_AbilityJumper)props;


        private DelegateFlyer pawnFlyer = null;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            Pawn pawn = parent.pawn;
            Map map = pawn.MapHeld;
            IntVec3 position = pawn.Position;
            IntVec3 targetCell = target.Cell;

            if (!CanHitTargetSafely(pawn, map, targetCell))
            {
                Messages.Message("JJK_JumpFailed".Translate(), MessageTypeDefOf.RejectInput, false);
                return;
            }

            if (pawnFlyer != null)
            {
                pawnFlyer.OnBeforeRespawnPawn -= PawnFlyer_OnBeforeRespawnPawn;
                pawnFlyer.OnRespawnPawn -= PawnFlyer_OnRespawnPawn;
            }

            pawnFlyer = (DelegateFlyer)PawnFlyer.MakeFlyer(JJKDefOf.JJK_GenericFlyer, pawn, targetCell, null, null);
            GenSpawn.Spawn(pawnFlyer, targetCell, map);
            pawnFlyer.OnBeforeRespawnPawn += PawnFlyer_OnBeforeRespawnPawn;
            pawnFlyer.OnRespawnPawn += PawnFlyer_OnRespawnPawn;
        }

        private void PawnFlyer_OnBeforeRespawnPawn(Pawn arg1, PawnFlyer arg2, Map map)
        {
            pawnFlyer.OnBeforeRespawnPawn -= PawnFlyer_OnBeforeRespawnPawn;
            foreach (var item in parent.comps.OfType<CompAbilityEffect_JumpLanding>())
            {
                item.OnLand(arg1, arg2, map);
            }
        }

        private void PawnFlyer_OnRespawnPawn(Pawn arg1, PawnFlyer arg2, Map map)
        {
            pawnFlyer.OnRespawnPawn -= PawnFlyer_OnRespawnPawn;
        }


        private bool CanHitTargetSafely(Pawn Caster, Map map, IntVec3 targetCell)
        {
            if (!targetCell.InBounds(map) || targetCell == Caster.Position || targetCell.Roofed(map) || !targetCell.WalkableByAny(map))
                return false;

            return true;
        }
    }
}