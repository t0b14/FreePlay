Shader "Unlit/FireShader"
{
    Properties
    {
        _Color ("Color", Color) = (1, 0, 0, 1)
        _AlphaThreshold ("Alpha Threshold", Range(0,1)) = 0.5
        _FadeStrength ("Fade Strength", Range(0,1)) = 1
        _NoiseScale ("Noise Scale", float) = 1
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
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 localPos : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.localPos = v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = WorldSpaceViewDir(v.vertex);
                return o;
            }

            fixed4 _Color;
            float _NoiseScale;
            float _AlphaThreshold;
            float _FadeStrength;

            fixed4 frag (v2f i) : SV_Target
            {
                float3 normal = normalize(i.normal);
                float3 viewDir = normalize(i.viewDir);
                float radius = pow(dot(normal, viewDir), 1/_FadeStrength);

                float3 strength;
                float3 raster = i.localPos.xyz - float3(0, _Time.z, 0);
                Unity_GradientNoise_float(raster.xy, _NoiseScale, strength.x);
                Unity_GradientNoise_float(raster.xz, _NoiseScale, strength.y);
                Unity_GradientNoise_float(raster.yz, _NoiseScale, strength.z);

                float fade = smoothstep(_AlphaThreshold, 1, (radius * length(strength)));
                fixed4 col = _Color;
                col.a *= fade;
                return col;
            }
            ENDCG
        }
    }
}
