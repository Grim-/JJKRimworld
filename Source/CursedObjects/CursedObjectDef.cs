using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace JJK
{
    public class CursedObjectDef : ThingDef
    {
        public int totalPieces = 20; // Default to 20 pieces like Sukuna's fingers
        public float abilityTransferThreshold = 0.5f; // 50%
    }
}
