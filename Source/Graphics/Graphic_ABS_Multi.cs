using UnityEngine;
using Verse;

namespace JJK
{
    public class Graphic_ABS_Multi : Graphic_Multi
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
            Material material = base.MatAt(rot, thing);
            ApplyCustomShader(material);
            return material;
        }

        public override Material MatSingle
        {
            get
            {
                Material material = base.MatSingle;
                ApplyCustomShader(material);
                return material;
            }
        }

        public override Material MatSingleFor(Thing thing)
        {
            Material material = base.MatSingleFor(thing);
            ApplyCustomShader(material);
            return material;
        }

        public void ApplyCustomShader(Material material)
        {
            material.shader = Shader;
            foreach (var item in Data.shaderParameters)
            {
                item.Apply(material);
            }

            ListMaterialProperties(material);
        }

        public void ListMaterialProperties(Material material)
        {
            if (material == null)
            {
                Debug.LogError("Material is null");
                return;
            }

            Log.Message($"Shader name: {material.shader.name}");
            Log.Message("Texture properties:");

            // Get all texture property names
            string[] texturePropertyNames = material.GetTexturePropertyNames();

            foreach (string propertyName in texturePropertyNames)
            {
                Texture texture = material.GetTexture(propertyName);
                if (texture != null)
                {
                    Log.Message($"- {propertyName}: {texture.name}");
                }
                else
                {
                    Log.Message($"- {propertyName}: Not assigned");
                }
            }
        }
    }
}
