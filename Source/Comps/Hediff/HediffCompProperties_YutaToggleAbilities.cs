using Verse;

namespace JJK
{
    public class HediffCompProperties_YutaToggleAbilities : HediffCompProperties
    {
        public HediffCompProperties_YutaToggleAbilities()
        {
            compClass = typeof(HediffComp_YutaToggleAbilities);
        }
    }

    public class HediffComp_YutaToggleAbilities : HediffComp
    {
        private HediffComp_YutaAbilities Storage => this.parent.pawn.GetYutaAbilityStorage();

        public override void CompPostMake()
        {
            base.CompPostMake();

            //if (Storage != null)
            //{
            //    Storage.GrantRiikaAbilities(Pawn);
            //}
        }

        public override void CompPostPostRemoved()
        {
            base.CompPostPostRemoved();

            //if (Storage != null)
            //{
            //    Storage.RemoveRiikaAbilities(Pawn);
            //}
        }
    }

}

