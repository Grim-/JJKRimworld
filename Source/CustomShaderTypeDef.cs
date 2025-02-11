using UnityEngine;
using Verse;

namespace JJK
{
    public class CustomShaderTypeDef : ShaderTypeDef
    {
        public new Shader Shader
        {
            get
            {
                if (JJKMod.CustomShaders.TryGetValue(shaderPath, out Shader customShader))
                {
                    return customShader;
                }
                Log.Error($"Custom shader not found: {shaderPath}");
                return ShaderDatabase.DefaultShader;
            }
            set 
            { 
            
            }
        }

        public string NewShaderPath;
    }
}
    

