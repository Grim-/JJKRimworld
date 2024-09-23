using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace JJK
{
    public class CompProperties_RiikaEat: CompProperties_CursedAbilityProps
    {
        public EffecterDef effecterDef;

        public CompProperties_RiikaEat()
        {
            compClass = typeof(Comp_RiikaEat);
        }
    }

    public class Comp_RiikaEat : BaseCursedEnergyAbility
    {
        public override void ApplyAbility(LocalTargetInfo target, LocalTargetInfo dest)
        {
            if (!(target.Thing is Corpse corpse))
                return;

            Pawn caster = parent.pawn;

            List<Ability> abilities = JJKUtility.GetCursedEnergyAbilities(corpse.InnerPawn);;

            if (abilities != null)
            {
                foreach (var item in abilities)
                {
                    if (!this.parent.pawn.HasAbility(item.def))
                    {
                        this.parent.pawn.abilities.GainAbility(item.def);
                    }
                }

            }
        }
    }
}

