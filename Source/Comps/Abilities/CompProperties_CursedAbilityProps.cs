﻿using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace JJK
{
    public abstract class CompProperties_CursedAbilityProps : CompProperties_AbilityEffect
    {
        public float cursedEnergyCost = 0.1f;
        public int cooldownTicks = 2500;

        public override IEnumerable<string> ExtraStatSummary()
        {
            yield return (string)("AbilitycursedEnergyCost".Translate() + ": ") + Mathf.RoundToInt(cursedEnergyCost);
        }
    }
}


