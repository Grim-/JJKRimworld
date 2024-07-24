using RimWorld;
using Verse;

namespace JJK
{
    public class CursedAbilityClass : Ability
    {
        public CursedAbilityClass() : base()
        {

        }

        public CursedAbilityClass(Pawn pawn, AbilityDef def) : base(pawn, def)
        {
            this.pawn = pawn;
            this.def = def;
            Initialize();
        }
    }
}
    

