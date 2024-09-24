using System.Collections.Generic;
using Verse;

namespace JJK
{
    public class TagExtension : DefModExtension
    {
        public List<string> Tags;

        public bool HasTag(string tag)
        {
            return Tags != null && Tags.Contains(tag);
        }
    }
}

