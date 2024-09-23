using UnityEngine;
using Verse;

namespace JJK
{
    public class Graphic_ABS : Graphic_Single
    {

        private Shader _Shader;
        public new Shader Shader
        {
            get
            {
                if (_Shader == null)
                {
                    _Shader = Data.GetShaderFromAssets();
                }
                return _Shader;
            }
        }


        public ABSGraphicData Data => (ABSGraphicData)data;


        public override Material MatAt(Rot4 rot, Thing thing = null)
        {
            Material material =  base.MatAt(rot, thing);
            material.shader = Shader;
            foreach (var item in Data.shaderParameters)
            {
                item.Apply(material);
            }

            material.renderQueue = 3001;
            return material;
        }
    }
}
