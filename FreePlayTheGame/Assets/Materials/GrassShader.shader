Shader "Unlit/GrassShader"
{
    Properties
    {
        _GroundColor ("Ground Color", Color) = (0, 1, 0, 1)
        _GrassColor ("Grass Color", Color) = (0, 1, 0, 1)
        _HighColor ("High Color", Color) = (0.5, 1, 0.5, 1)
        _GrassScale ("Grass Scale", Range(0, 1)) = 1
        _GrassHeight ("Grass Height", Range(0, 2)) = 1
        _GrassBend ("Grass Bend", Range(0, 1)) = 0.1
        _WindScale ("Wind Scale", Range(0, 5)) = 0
        _WindStrength ("Wind Strength", Range(0, 1)) = 0
        _WindSpeed ("Wind Speed", Range(0, 10)) = 1
        _TessellationUniform ("Tesselation", Range(0, 16)) = 1
        _RandPos ("Randomize Position", Range(0, 1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };     

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };       

            fixed4 _GroundColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = _GroundColor;
                return col;
            }
            ENDCG
        }

        Pass
        {
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma hull hull
            #pragma domain domain
            #pragma geometry geo
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "ShaderNoise.cginc"


            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
            };

            struct tessfactor
            {
                float edge[3] : SV_TessFactor;
                float inside : SV_InsideTessFactor;
            };

            struct vertdata
            {
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
            };

            struct fragdata
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };


            fixed4 _GrassColor;
            fixed4 _HighColor;
            float _GrassScale;
            float _GrassHeight;
            float _GrassBend;
            float _WindScale;
            float _WindStrength;
            float _WindSpeed;
            float _TessellationUniform;
            float _RandPos;


            vertdata vert (appdata v)
            {
                vertdata o;
                o.vertex = v.vertex;
                o.normal = v.normal;
                o.tangent = v.tangent;
                return o;
            }

            tessfactor patchConstantFunction (InputPatch<vertdata, 3> patch)
            {
                tessfactor f;
                f.edge[0] = _TessellationUniform;
                f.edge[1] = _TessellationUniform;
                f.edge[2] = _TessellationUniform;
                f.inside = _TessellationUniform;
                return f;
            }

            [UNITY_domain("tri")]
            [UNITY_outputcontrolpoints(3)]
            [UNITY_outputtopology("triangle_cw")]
            [UNITY_partitioning("integer")]
            [UNITY_patchconstantfunc("patchConstantFunction")]
            vertdata hull (InputPatch<vertdata, 3> patch, uint id : SV_OutputControlPointID)
            {
                return patch[id];
            }

            [UNITY_domain("tri")]
            vertdata domain(tessfactor factors, OutputPatch<vertdata, 3> patch, float3 barycentricCoordinates : SV_DomainLocation)
            {
                vertdata v;

                #define MY_DOMAIN_PROGRAM_INTERPOLATE(fieldName) v.fieldName = \
                    patch[0].fieldName * barycentricCoordinates.x + \
                    patch[1].fieldName * barycentricCoordinates.y + \
                    patch[2].fieldName * barycentricCoordinates.z;

                MY_DOMAIN_PROGRAM_INTERPOLATE(vertex)
                MY_DOMAIN_PROGRAM_INTERPOLATE(normal)
                MY_DOMAIN_PROGRAM_INTERPOLATE(tangent)

                vertdata o;
                o.vertex = v.vertex;
                o.normal = v.normal;
                o.tangent = v.tangent;
                return o;
            }

            [maxvertexcount(3)]
            void geo(triangle vertdata IN[3] : SV_POSITION, inout TriangleStream<fragdata> triStream)
            {
                fragdata o;

                float3 pos = IN[0].vertex;
                float3 normal = IN[0].normal;
                float4 tangent = IN[0].tangent;
                float3 binormal = cross(normal, tangent) * tangent.w;
                float3x3 m_local = float3x3(
                    tangent.x, binormal.x, normal.x,
                    tangent.y, binormal.y, normal.y,
                    tangent.z, binormal.z, normal.z
                );

                float noise = 0;
                Unity_SimpleNoise_float(pos.xz + _Time.y * _WindSpeed, _WindScale, noise);

                float3 random = _RandPos * (randvec(pos) - 0.5);
                float3 localPos = pos + float3(random.x, 0, random.z);
                float3x3 rot = AngleAxis3x3(random.y * UNITY_TWO_PI, float3(0, 0, 1));
                float3x3 b_local = AngleAxis3x3(rand(pos) * _GrassBend * UNITY_PI * 0.5, float3(-1, 0, 0));
                float3x3 w_local = AngleAxis3x3(noise * _WindStrength * UNITY_PI, float3(0, 1, 0));
                float3x3 t_local = mul(mul(mul(m_local, w_local), rot), b_local);

                o.vertex = UnityObjectToClipPos(localPos + mul(t_local, float3(-0.5*_GrassScale, 0, 0)));
                o.uv = float2(0, 0);
                triStream.Append(o);

                o.vertex = UnityObjectToClipPos(localPos + mul(t_local, float3(0.5*_GrassScale, 0, 0)));
                o.uv = float2(1, 0);
                triStream.Append(o);

                o.vertex = UnityObjectToClipPos(localPos + mul(t_local, float3(0, 0, _GrassHeight)));
                o.uv = float2(0.5, 1);
                triStream.Append(o);
            }

            fixed4 frag (fragdata i) : SV_Target
            {
                fixed4 col = lerp(_GrassColor, _HighColor, i.uv.y);
                return col;
            }
            ENDCG
        }
    }
}
