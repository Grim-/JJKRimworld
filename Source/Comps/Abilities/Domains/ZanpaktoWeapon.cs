using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace JJK
{
    public class ZanpaktoWeapon : ThingWithComps, IZanpaktoStateHolder
    {      
        public ZanpaktoWeapon()
        {
        
        }

        private ZanpaktoWeaponDef Def => (ZanpaktoWeaponDef)def;

        private ZanpaktoState currentState = ZanpaktoState.Sealed;

        public event Action<ZanpaktoState> StateChanged;

        public ZanpaktoState CurrentState => currentState;

        public override Graphic Graphic
        {
            get
            {
                Graphic stateGraphic = GetGraphicForState(currentState);
                if (stateGraphic != null)
                {
                    return stateGraphic;
                }

                return base.Graphic;
            }
        }


        private Graphic GetGraphicForState(ZanpaktoState state)
        {
            switch (state)
            {
                case ZanpaktoState.Sealed:
                    return Def.sealedForm.graphicData.Graphic;
                case ZanpaktoState.Shikai:
                    return Def.shikaiForm.graphicData.Graphic;
                case ZanpaktoState.Bankai:
                    return Def.bankaiForm.graphicData.Graphic;
            }
            return null;
        }

        public SwordFormDef GetSwordFormForState(ZanpaktoState state)
        {
            switch (state)
            {
                case ZanpaktoState.Sealed:
                    return Def.sealedForm;
                case ZanpaktoState.Shikai:
                    return Def.shikaiForm;
                case ZanpaktoState.Bankai:
                    return Def.bankaiForm;
            }
            return null;
        }

        public void SetState(ZanpaktoState state)
        {
            currentState = state;
            EffecterDefOf.ApocrionAoeResolve.SpawnAttached(this, this.MapHeld, 3f);
            StateChanged?.Invoke(state);
        }
        public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
        {
            foreach (StatDrawEntry entry in base.SpecialDisplayStats())
            {
                yield return entry;
            }

            yield return new StatDrawEntry(StatCategoryDefOf.Weapon,
                StatDefOf.MeleeDamageFactor, GetSwordFormForState(currentState).MeleeDamage, StatRequest.For(this));
        }

        public override string GetInspectString()
        {
            return base.GetInspectString() + $"\r\n State: {currentState}";
        }


        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref currentState, "currentState", ZanpaktoState.Sealed);
        }
    }
    public class ZanpaktoWeaponDef : ThingDef
    {
        public SwordFormDef sealedForm;
        public SwordFormDef shikaiForm;
        public SwordFormDef bankaiForm;

        public ZanpaktoWeaponDef()
        {
            thingClass = typeof(ZanpaktoWeapon);
        }
    }


    public class SwordFormDef
    {
        public GraphicData graphicData;
        public float MeleeDamage = 1f;
        public float Cooldown = 2f;
    }
}