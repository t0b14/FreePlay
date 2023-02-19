Shader "Unlit/SurfaceDisplacementShader"
{
    Properties
    {
        _TessellationUniform ("Tessellation", Range(1, 256)) = 1
        _Center ("Center", Vector) = (0,0,0,1)
        _Height ("Height", Range(0,1)) = 0
        _Scale ("Scale", Range(0,50)) = 0
        _Sharpness ("Sharpness", Range(0,12)) = 1
        _Lower ("Sharpness", Range(0,1)) = 0
        _Upper ("Sharpness", Range(0,1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 300

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma hull hull
            #pragma domain domain
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "ShaderNoise.cginc"


            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
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
            };

            struct fragdata
            {
                float4 vertex : SV_POSITION;
                float noise : TEXCOORD1;
            };


            float _TessellationUniform;
            float4 _Center;
            float _Height;
            float _Scale;
            float _Sharpness;
            float _Lower;
            float _Upper;

            vertdata vert (appdata v)
            {
                vertdata o;
                o.vertex = v.vertex;
                o.normal = normalize(v.vertex - _Center);
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

            float disp(float3 p)
            {
                return voronoiNoise(p * _Scale + float3(0,1,0) * 2 * _Time.y);
            }

            [UNITY_domain("tri")]
            fragdata domain(tessfactor factors, OutputPatch<vertdata, 3> patch, float3 barycentricCoordinates : SV_DomainLocation)
            {
                vertdata v;

                #define DOMAIN_INTERPOLATE(fieldName) v.fieldName = \
                    patch[0].fieldName * barycentricCoordinates.x + \
                    patch[1].fieldName * barycentricCoordinates.y + \
                    patch[2].fieldName * barycentricCoordinates.z;

                DOMAIN_INTERPOLATE(vertex)
                DOMAIN_INTERPOLATE(normal)

                float noise = smoothstep(_Lower, _Upper, pow(disp(v.vertex.xyz), _Sharpness));
                v.vertex.xyz -= _Height * noise * v.normal;

                fragdata o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.noise = noise;
                return o;
            }

            fixed4 frag (fragdata i) : SV_Target
            {
                fixed4 col = fixed4(i.noise, i.noise, 0, 1);
                return col;
            }
            ENDCG
        }
    }
}
