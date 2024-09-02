using Verse;
using UnityEngine;

namespace JJK
{
    public class Graphic_Animated : Graphic_Indexed
    {
        private int currentFrameIndex = 0;
        private float timeSinceLastFrame = 0f;
        public float FrameDuration = 0.1f; // Time each frame is shown
        protected MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();

        public override Material MatSingle
        {
            get
            {
                return this.SubGraphicAtIndex(currentFrameIndex).MatSingle;
            }
        }

        public override void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
        {
            // Update the current frame
            timeSinceLastFrame += Time.deltaTime;

            if (timeSinceLastFrame >= FrameDuration)
            {
                currentFrameIndex = (currentFrameIndex + 1) % this.SubGraphicsCount;
                timeSinceLastFrame = 0f;
            }

            Vector3 s = new Vector3(1f, 1f, 1f);
            Vector3 exactScale = Vector3.one;
            s.x *= exactScale.x;
            s.z *= exactScale.z;
            Matrix4x4 matrix = default(Matrix4x4);
            if (!thingDef.rotatable)
            {
                rot = Rot4.North;
            }
            Quaternion q = rot.AsQuat;

            matrix.SetTRS(loc, q, s);
            Graphic graphic = this.subGraphics[currentFrameIndex];
            Material material = graphic.MatAt(rot, thing);
            Graphics.DrawMesh(graphic.MeshAt(rot), matrix, material, 0, null, 0, this.propertyBlock);
        }

        public override Graphic GetColoredVersion(Shader newShader, Color newColor, Color newColorTwo)
        {
            return GraphicDatabase.Get<Graphic_Animated>(this.path, newShader, this.drawSize, newColor, newColorTwo, this.data, null);
        }

        public override string ToString()
        {
            return string.Concat(new object[]
            {
            "Animated(path=",
            this.path,
            ", count=",
            this.SubGraphicsCount,
            ", currentFrame=",
            this.currentFrameIndex,
            ")"
            });
        }
    }
}
