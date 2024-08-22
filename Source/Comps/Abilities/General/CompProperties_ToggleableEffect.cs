using Verse;

namespace JJK
{
    public class CompProperties_ToggleableEffect : CompProperties_CursedAbilityProps
    {
        public int Ticks = 2500;
    }


    public abstract class ToggleableCompAbilityEffect : BaseCursedEnergyAbility, IToggleableComp
    {
        public new CompProperties_ToggleableEffect Props => (CompProperties_ToggleableEffect)props;

        public bool IsActive = false;


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

            if (parent.pawn.IsHashIntervalTick(Props.Ticks))
            {
                OnTickInterval();
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
    }
}