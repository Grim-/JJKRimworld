using UnityEngine;
using Verse;

namespace JJK
{
    public class MoteBeam : Mote
    {
        private Vector3 origin;
        private Vector3 destination;
        private Material beamMat;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref origin, "origin");
            Scribe_Values.Look(ref destination, "destination");
        }

        public void UpdateTargets(Vector3 newOrigin, Vector3 newDestination)
        {
            origin = newOrigin;
            destination = newDestination;
        }

        protected override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            base.DrawAt(drawLoc, flip);

            if (beamMat == null)
            {
                beamMat = MaterialPool.MatFrom(def.graphicData.texPath, ShaderDatabase.MoteGlow);
            }

            Vector3 vec = destination - origin;
            float length = vec.magnitude;
            Quaternion rotation = Quaternion.LookRotation(vec);
            Vector3 pos = origin + vec / 2f + Vector3.up * 0.1f;

            Matrix4x4 matrix = Matrix4x4.TRS(pos, rotation, new Vector3(1f, 1f, length));
            Graphics.DrawMesh(MeshPool.plane10, matrix, beamMat, 0);
        }
    }
    //public class MoteBeam : Mote
    //{
    //    private Vector3 origin;
    //    private Vector3 destination;
    //    private Material beamMat;

    //    public override void ExposeData()
    //    {
    //        base.ExposeData();
    //        Scribe_Values.Look(ref origin, "origin");
    //        Scribe_Values.Look(ref destination, "destination");
    //    }

    //    public void UpdateTargets(TargetInfo originTarget, TargetInfo destinationTarget)
    //    {
    //        origin = originTarget.CenterVector3;
    //        destination = destinationTarget.CenterVector3;
    //    }

    //    protected override void DrawAt(Vector3 drawLoc, bool flip = false)
    //    {
    //        //base.DrawAt(drawLoc, flip);

    //        if (beamMat == null)
    //        {
    //            beamMat = MaterialPool.MatFrom(def.graphicData.texPath, ShaderDatabase.MoteGlow);
    //        }

    //        Vector3 vec = destination - origin;
    //        float length = vec.magnitude;
    //        Quaternion rotation = Quaternion.LookRotation(vec);
    //        Vector3 pos = origin + vec / 2f + Vector3.up * 0.1f;

    //        Matrix4x4 matrix = default(Matrix4x4);
    //        matrix.SetTRS(pos, rotation, new Vector3(1f, 1f, length));

    //        Graphics.DrawMesh(MeshPool.plane10, matrix, beamMat, 0);
    //    }
    //}
}
    

