Shader "Custom/DomainEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MaskTex ("Mask", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _ColorTwo ("Color Two", Color) = (1,1,1,1)
        _ColorThree ("Color Three", Color) = (1,1,1,1)
        _CutoutTex ("Cutout", 2D) = "white" {}

        // Custom properties for the domain effect
        _CubemapTex ("Starfield Cubemap", Cube) = "" {}
        _ExpansionRate ("Expansion Rate", Float) = 1.0
    }

    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        LOD 100

        CGPROGRAM
        #pragma surface surf Lambert alpha

        sampler2D _MainTex;
        sampler2D _MaskTex;
        sampler2D _CutoutTex;
        samplerCUBE _CubemapTex;
        fixed4 _Color;
        fixed4 _ColorTwo;
        fixed4 _ColorThree;
        float _ExpansionRate;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldRefl;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            fixed4 mask = tex2D(_MaskTex, IN.uv_MainTex);
            
            // Apply color masking
            c.rgb = lerp(c.rgb, _ColorTwo.rgb, mask.r);
            c.rgb = lerp(c.rgb, _ColorThree.rgb, mask.g);

            // Apply starfield effect
            float3 starfield = texCUBE(_CubemapTex, IN.worldRefl * (_Time.y * _ExpansionRate)).rgb;
            c.rgb = lerp(c.rgb, starfield, mask.b); // Use blue channel of mask for starfield blend

            o.Albedo = c.rgb;
            o.Alpha = tex2D(_CutoutTex, IN.uv_MainTex).r * c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}