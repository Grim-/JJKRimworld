using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using Verse;

namespace JJK
{
    public class Ability_Toggleable : Ability
    {
        public bool IsActive = false;
        //I have no god damn idea which constructors are even used.

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

        public override bool Activate(LocalTargetInfo target, LocalTargetInfo dest)
        {
            Toggle();
            return base.Activate(target, dest);
        }

        public override IEnumerable<Command> GetGizmos()
        {
            if (!this.pawn.Drafted || this.def.showWhenDrafted)
            {
                yield return new Command_ToggleAbility(pawn, this, Toggle);
            }
        }

        public virtual void Toggle()
        {
            //Log.Message("Toggle pressed");
            IsActive = !IsActive;

            if (EffectComps != null && EffectComps.Count > 0)
            {
                foreach (var item in EffectComps)
                {
                    if (item is IToggleableComp toggleableComp)
                    {
                        if (IsActive)
                        {
                            //Log.Message("Activating");
                            toggleableComp.Activate();
                        }
                        else
                        {
                            //Log.Message("Deactivating");
                            toggleableComp.DeActivate();
                        }
                           
                    }
                }
            }
        }

        public void ForceDeactivate()
        {
            if (IsActive)
            {
                Toggle();
            }
        }
    }
}