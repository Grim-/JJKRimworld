using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace JJK
{
    public class CompProperties_Dismissable : CompProperties
    {
        public CompProperties_Dismissable()
        {
            compClass = typeof(CompDismissableEffect);
        }
    }


    public class CompDismissableEffect : ThingComp
    {
        public new CompProperties_Dismissable Props => (CompProperties_Dismissable)props;

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            yield return new Command_Action
            {
                defaultLabel = "Dismiss",
                icon = ContentFinder<Texture2D>.Get("UI/Designators/Deconstruct"),
                action = () =>
                {
                    parent.Destroy();
                }
            };
        }
    }


}