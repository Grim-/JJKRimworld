using Verse;

namespace JJK
{
    public class CompProperties_BoundPawn : CompProperties
    {
        public PawnKindDef pawnKindDef;
    }


    public class BoundPawnComp : BaseTraitComp
    {
        public new CompProperties_BoundPawn Props => (CompProperties_BoundPawn)props;

        public override string TraitName => $"Bound {Props.pawnKindDef.label}";

        public override string Description => $"This equipment has a {Props.pawnKindDef.label} soul bound to it, upon equipping the bound pawn will spawn, if it dies there is a 3-day cooldown before it can appear again.";

        private Pawn spawnedPawn;
        private int cooldownTicks = -1;
        private const int CooldownDurationTicks = 180000; // 3 days * 60000 ticks per day

        public override void Notify_Equipped(Pawn pawn)
        {
            base.Notify_Equipped(pawn);

            if (cooldownTicks > 0)
            {
                return;
            }

            SpawnPawn(pawn);
        }

        public override void Notify_Unequipped(Pawn pawn)
        {
            base.Notify_Unequipped(pawn);

            DestroyBoundPawn();
        }

        private void DestroyBoundPawn()
        {
            if (spawnedPawn != null && !spawnedPawn.Dead)
            {
                spawnedPawn.Destroy();
                spawnedPawn = null;
            }
        }

        public override void CompTick()
        {
            base.CompTick();

            if (cooldownTicks > 0)
            {
                cooldownTicks--;
            }

            if (spawnedPawn != null && spawnedPawn.Dead)
            {
                StartCooldown();
                spawnedPawn.Destroy();
                spawnedPawn = null;
            }
        }

        private void SpawnPawn(Pawn equipper)
        {
            spawnedPawn = PawnGenerator.GeneratePawn(Props.pawnKindDef, equipper.Faction);
            spawnedPawn.health.AddHediff(JJKDefOf.JJK_ZombieWorkSlaveHediff);
            GenSpawn.Spawn(spawnedPawn, equipper.Position, equipper.Map);
        }

        public override string CompInspectStringExtra()
        {

            string Descrip = cooldownTicks > 0 ? $"The {Props.pawnKindDef.label} bound to this equipment has recently died and needs time to regenerate." : string.Empty;

            return base.CompInspectStringExtra() + Descrip;
        }

        private void StartCooldown()
        {
            cooldownTicks = CooldownDurationTicks;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_References.Look(ref spawnedPawn, "spawnedPawn");
            Scribe_Values.Look(ref cooldownTicks, "cooldownTicks");
        }
    }
}

    