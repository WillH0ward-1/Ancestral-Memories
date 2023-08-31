Shader "ProceduralModeling/ProceduralTreeAlbedo" {
    Properties {
        _EnableNoise ("Enable Noise", Range(0, 1)) = 1.0
        _T ("Growing", Range(0.0, 1.0)) = 1.0
        _MainTex ("Albedo", 2D) = "white" {}
        _LightColor ("Light Color", Color) = (1,1,1,1)
        _NoiseScaleX ("Noise Scale X", Range(0.01, 10)) = 1.0
        _NoiseScaleY ("Noise Scale Y", Range(0.01, 10)) = 1.0
        _NoiseScaleZ ("Noise Scale Z", Range(0.01, 10)) = 1.0
        _NoiseAmount ("Noise Amount", Range(0.0, 1.0)) = 0.1
        _TimeScale ("Time Scale", Range(0.0, 2.0)) = 1.0
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Cull Off 

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing // Enabling GPU Instancing

            #include "UnityCG.cginc"
            #include "./ProceduralTree.cginc"

            uniform float _EnableNoise;
            sampler2D _MainTex;
            float4 _LightColor;
            float _NoiseScaleX, _NoiseScaleY, _NoiseScaleZ, _NoiseAmount, _TimeScale;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID // Required for GPU Instancing
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO // Required for GPU Instancing
            };

            float perlinNoise(float3 pos) {
                return frac(sin(dot(pos ,float3(12.9898,78.233,37.719))) * 43758.5453);
            }

            v2f vert (appdata v) {
                UNITY_SETUP_INSTANCE_ID(v);  // Required for GPU Instancing
                v2f o;
                if (_EnableNoise > 0.5) { // If _EnableNoise is true
                    float3 noiseInput = v.vertex.xyz;
                    noiseInput.x *= _NoiseScaleX;
                    noiseInput.y *= _NoiseScaleY;
                    noiseInput.z *= _NoiseScaleZ;
                    float noise = _NoiseAmount * (2.0f * (0.5f - perlinNoise(noiseInput + _Time * _TimeScale)));
                    v.vertex.xyz += noise * v.normal;
                }
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                procedural_tree_clip(i.uv);
                fixed4 albedo_front = tex2D(_MainTex, i.uv);
                fixed4 albedo_back = tex2D(_MainTex, 1 - i.uv);
                fixed4 albedo = lerp(albedo_front, albedo_back, step(0.5, i.uv.y));
                
                fixed4 c;
                c.rgb = albedo.rgb * _LightColor.rgb;
                c.a = albedo.a;
                
                return c;
            }
            ENDCG
        }
    }
}
