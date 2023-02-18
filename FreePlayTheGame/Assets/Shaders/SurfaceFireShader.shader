Shader "Custom/SurfaceFireShader"
{
    Properties
    {
        [Header(Color)]
        _BaseColor ("Base Color", Color) = (0,0,0,1)
        _HighColor ("Highlight Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}

        [Header(Texture)]
        _Scale ("Scale", Range(0,32)) = 1
        _Smoothness ("Smoothness", Range(0.1,50)) = 1
        _HighAmount("Highlight Coverage", Range(0,1)) = 0
        _Sharpness("Sharpness", Range(0,1)) = 1
        _Height ("Height", Range(0,1)) = 0

        [Header(Effect)]
        _EffectColor ("Color", Color) = (1,1,1,1)
        _EffectAlpha ("Alpha", Range(0,1)) = 0.1
        _EffectSize ("Size", Range(0,1)) = 0.1
        _EffectScale ("Scale", Range(0,50)) = 1
        _EffectSpeed ("Animation Speed", Range(0,10)) = 0

        [Header(Rendering)]
        _Speed ("Animation Speed", Range(0,10)) = 0
        _Tessellation ("Tessellation", Range(1,256)) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows tessellate:tess vertex:vert
        #pragma target 3.0

        #include "ShaderNoise.cginc"


        struct Input
        {
            float2 uv_MainTex;
            float4 color : COLOR;
        };

        fixed4 _BaseColor;
        fixed4 _HighColor;
        sampler2D _MainTex;
        float _HighAmount;
        float _Sharpness;
        float _Scale;
        float _Smoothness;
        float _Height;
        float _Speed;
        float _Tessellation;


        float tess()
        {
            return _Tessellation;
        }

        void vert(inout appdata_full v)
        {
            float noise = voronoiNoise(v.vertex.xyz * _Scale + float3(0,1,0) * _Speed * _Time.y);

            v.color.x = pow(noise, _Smoothness);
            v.color.y = 1-smoothstep(_HighAmount - _Sharpness, _HighAmount + _Sharpness, 1-v.color.x);
            v.vertex.xyz -= _Height * v.color.y * v.normal;
        }

        void surf (Input i, inout SurfaceOutputStandard o)
        {
            fixed4 col = lerp(_BaseColor, _HighColor, i.color.y);
            fixed4 c = tex2D (_MainTex, i.uv_MainTex) * col;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG

 
        Blend OneMinusDstColor One
        ZWrite Off

        CGPROGRAM
        #pragma vertex vert
        #pragma surface surf Lambert alpha:fade
        #pragma target 3.0

        #include "ShaderNoise.cginc"

        struct Input
        {
            float2 uv_MainTex : TEXCOORD0;
            float noise : TEXCOORD1;
        };


        fixed4 _EffectColor;
        float _EffectAlpha;
        float _EffectSize;
        float _EffectScale;
        float _EffectSpeed;

        float sum(float3 p)
        {
            return p.x + p.y + p.z;
        }

        void vert(inout appdata_base v, out Input o)
        {
            float3 noise, noise2;
            Unity_SimpleNoise_float(v.vertex.yz, _EffectScale + _Time.y * _EffectSpeed, noise.x);
            Unity_SimpleNoise_float(v.vertex.xz, _EffectScale + _Time.y * _EffectSpeed, noise.y);
            Unity_SimpleNoise_float(v.vertex.xy, _EffectScale + _Time.y * _EffectSpeed, noise.z);
            Unity_SimpleNoise_float(v.vertex.yz, _EffectScale - _Time.y * _EffectSpeed, noise2.x);
            Unity_SimpleNoise_float(v.vertex.xz, _EffectScale - _Time.y * _EffectSpeed, noise2.y);
            Unity_SimpleNoise_float(v.vertex.xy, _EffectScale - _Time.y * _EffectSpeed, noise2.z);

            v.vertex.xyz += _EffectSize * v.normal * noise;

            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.noise = sum(noise) + sum(noise2);
        }

        void surf (Input i, inout SurfaceOutput o)
        {
            o.Albedo = _EffectColor.rgb;
            o.Alpha = smoothstep(1-_EffectAlpha, 1, _EffectColor.a * i.noise);
        }
        ENDCG
    }
    FallBack "Diffuse"
}
