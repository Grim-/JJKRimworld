﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace JJK
{
    public class CEResourceDrainUtility
    {
        public static void TickResourceDrain(IGeneResourceDrain drain)
        {
            if (drain.CanOffset && drain.Resource != null)
            {
                OffsetResource(drain, (0f - drain.ResourceLossPerDay) / 60000f);
            }
        }

        public static void PostResourceOffset(IGeneResourceDrain drain, float oldValue)
        {
            if (oldValue > 0f && drain.Resource.Value <= 0f)
            {
                Pawn pawn = drain.Pawn;
            }
        }

        public static void OffsetResource(IGeneResourceDrain drain, float amnt)
        {
            float value = drain.Resource.Value;
            drain.Resource.Value += amnt;
            PostResourceOffset(drain, value);
        }

        public static IEnumerable<Gizmo> GetResourceDrainGizmos(IGeneResourceDrain drain)
        {
            if (DebugSettings.ShowDevGizmos && drain.Resource != null)
            {
                Gene_Resource resource = drain.Resource;
                Command_Action command_Action = new Command_Action();
                command_Action.defaultLabel = "DEV: " + resource.ResourceLabel + " -10";
                command_Action.action = delegate
                {
                    OffsetResource(drain, -0.1f);
                };
                yield return command_Action;
                Command_Action command_Action2 = new Command_Action();
                command_Action2.defaultLabel = "DEV: " + resource.ResourceLabel + " +10";
                command_Action2.action = delegate
                {
                    OffsetResource(drain, 0.1f);
                };
                yield return command_Action2;
            }
        }
    }
}
    
