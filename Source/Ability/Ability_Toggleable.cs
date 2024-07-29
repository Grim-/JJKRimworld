using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace JJK
{
    public class Ability_Toggleable : Ability
    {
        public bool IsActive = false;

        public event Action OnToggle;

        public Ability_Toggleable() : base ()
        {

        }

        public Ability_Toggleable(Pawn pawn) : base(pawn)
        {
        }

        public Ability_Toggleable(Pawn pawn, AbilityDef def) : base(pawn, def)
        {

        }
        public Ability_Toggleable(Pawn pawn, Precept sourcePrecept, AbilityDef def) : base(pawn, sourcePrecept, def)
        {

        }

       

        public override IEnumerable<Command> GetGizmos()
        {
            if (!this.pawn.Drafted || this.def.showWhenDrafted)
            {
                Command_ToggleAbility gizmo = new Command_ToggleAbility(pawn, this, IsActive, Toggle);
                gizmo.Order = this.def.uiOrder;
                yield return gizmo;
            }
        }

        public virtual void Toggle()
        {
            IsActive = !IsActive;
            OnToggle?.Invoke();
        }
    }
}