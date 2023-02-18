Shader "Custom/WaterShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _HighColor ("Hightlight Color", Color) = (1,1,1,1)
        _Scale ("Scale", Range(0, 10)) = 1
        _Speed ("Speed", Range(0, 10)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma vertex vert
        #pragma surface surf Lambert fullforwardshadows
        #pragma target 3.0

        #include "ShaderNoise.cginc"


        struct Input
        {
            float2 uv_MainTex : TEXCOORD0;
            float4 pos : SV_POSITION;
        };

        sampler2D _MainTex;
        fixed4 _BaseColor;
        fixed4 _HighColor;
        float _Scale;
        float _Speed;

        float prod(float3 p)
        {
            return p.x * p.y *p.z;
        }

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.pos = v.vertex;
        }

        void surf (Input i, inout SurfaceOutput o)
        {
            float3 noise_up, noise_down;
            Unity_GradientNoise_float(i.pos.yz + _Time.x * _Speed, _Scale, noise_up.x);
            Unity_GradientNoise_float(i.pos.xz + _Time.x * _Speed, _Scale, noise_up.y);
            Unity_GradientNoise_float(i.pos.xy + _Time.x * _Speed, _Scale, noise_up.z);
            Unity_GradientNoise_float(i.pos.yz - _Time.x * _Speed, _Scale, noise_down.x);
            Unity_GradientNoise_float(i.pos.xz - _Time.x * _Speed, _Scale, noise_down.y);
            Unity_GradientNoise_float(i.pos.xy - _Time.x * _Speed, _Scale, noise_down.z);

            fixed4 c = lerp(_BaseColor, _HighColor, prod(noise_up) + prod(noise_down));
            o.Albedo = c.rgb;
            o.Gloss = 0.8;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
