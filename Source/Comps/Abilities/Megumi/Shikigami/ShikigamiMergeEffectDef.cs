using System;
using Verse;

namespace JJK
{
    public class ShikigamiMergeEffectDef : Def
    {
        public Type workerClass;
        public string mergeDescription;

        public ShikigamiMergeWorker CreateWorker()
        {
            return (ShikigamiMergeWorker)Activator.CreateInstance(workerClass);
        }
    }
}