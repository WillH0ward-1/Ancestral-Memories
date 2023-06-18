Shader "ProceduralModeling/ProceduralTreeAlbedo" {
    Properties {
        _T ("Growing", Range(0.0, 1.0)) = 1.0
        _MainTex ("Albedo", 2D) = "white" {}
        _LightColor ("Light Color", Color) = (1,1,1,1)
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

            v2f vert (appdata v) {
                v2f o;
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
