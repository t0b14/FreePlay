Shader "Unlit/FireShader"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1, 0, 0, 1)
        _HighColor ("Highlight Color", Color) = (1, 1, 0, 1)
        _WidthScale ("Width Scale", float) = 1
        _HeightScale ("Height Scale", float) = 1
        _ShapeScale ("Shape Scale", float) = 1
        _Alpha ("Alpha", Range(0,1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector"="True" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Back Lighting Off ZWrite Off ZTest Less

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "ShaderNoise.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : POSITION;
                float4 localPos : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.localPos = v.vertex;
                return o;
            }

            fixed4 _BaseColor;
            fixed4 _HighColor;
            float _WidthScale;
            float _HeightScale;
            float _ShapeScale;
            float _Alpha;

            fixed4 frag (v2f i) : SV_Target
            {
                float3 strength;
                float3 raster = i.localPos.xyz - float3(0, _Time.y, 0);
                Unity_GradientNoise_float(raster.xy, _WidthScale, strength.x);
                Unity_GradientNoise_float(raster.xz, _WidthScale, strength.y);
                Unity_GradientNoise_float(raster.yz, _WidthScale, strength.z);
                fixed4 col = lerp(_BaseColor, _HighColor, dot(strength, strength));

                float fade;
                Unity_GradientNoise_float(i.localPos.xz, _ShapeScale, fade);
                fade = saturate(1 - i.localPos.y / _HeightScale * (1 - 0.5*fade));

                col.w = fade * _Alpha;

                return col;
            }
            ENDCG
        }
    }
}
