Shader "ProceduralModeling/ProceduralTreeAlbedo" {
    Properties {
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

        Cull Off // Disable backface culling

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "./ProceduralTree.cginc"

            sampler2D _MainTex;
            float4 _LightColor;
            float _NoiseScaleX, _NoiseScaleY, _NoiseScaleZ, _NoiseAmount, _TimeScale;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };

            // Procedural Perlin noise function
            float perlinNoise(float3 pos) {
                return frac(sin(dot(pos ,float3(12.9898,78.233,37.719))) * 43758.5453);
            }

            v2f vert (appdata v) {
                v2f o;
                float3 noiseInput = v.vertex.xyz;
                noiseInput.x *= _NoiseScaleX;
                noiseInput.y *= _NoiseScaleY;
                noiseInput.z *= _NoiseScaleZ;
                float noise = _NoiseAmount * (2.0f * (0.5f - perlinNoise(noiseInput + _Time * _TimeScale)));
                v.vertex.xyz += noise * v.normal;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                procedural_tree_clip(i.uv);
                fixed4 albedo_front = tex2D(_MainTex, i.uv);
                fixed4 albedo_back = tex2D(_MainTex, 1 - i.uv); // Sample the texture with inverted UV for the back face
                fixed4 albedo = lerp(albedo_front, albedo_back, step(0.5, i.uv.y)); // Use step function to blend between front and back face
                
                fixed4 c;
                c.rgb = albedo.rgb * _LightColor.rgb;
                c.a = albedo.a;
                
                return c;
            }
            ENDCG
        }
    }
}
