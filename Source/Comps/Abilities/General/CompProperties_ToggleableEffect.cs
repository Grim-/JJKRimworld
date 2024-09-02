using RimWorld;
using UnityEngine;
using Verse;

namespace JJK
{
    public class CompProperties_ToggleableEffect : CompProperties_CursedAbilityProps
    {
        public int Ticks = 500;
    }


    public abstract class ToggleableCompAbilityEffect : BaseCursedEnergyAbility, IToggleableComp
    {
        public new CompProperties_ToggleableEffect Props => (CompProperties_ToggleableEffect)props;

        public bool IsActive = false;

        public virtual int Ticks => Props.Ticks;


        public virtual void Activate()
        {
            IsActive = true;
        }

        public virtual void DeActivate()
        {
            IsActive = false;
        }

        public override void CompTick()
        {
            base.CompTick();

            if (parent.pawn.IsHashIntervalTick(Ticks))
            {
                if (IsActive)
                {
                    OnTickInterval();
                }
              
            }
        }

        public virtual void OnTickInterval()
        {

        }

        public virtual void Toggle()
        {
            if (IsActive)
            {
                DeActivate();
            }
            else Activate();
        }

        public void DeactiveOnParentAbility()
        {
            // Notify the ability that it should deactivate
            if (parent is Ability_Toggleable ability)
            {
                ability.ForceDeactivate();
               // Messages.Message($"Reversed Curse Technique disabled due to lack of curse energy", MessageTypeDefOf.NegativeEvent);
            }
        }
    }
}