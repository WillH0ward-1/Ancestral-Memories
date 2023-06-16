Shader "ProceduralModeling/ProceduralTreeURP" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

        _T ("Growing", Range(0.0, 1.0)) = 1.0
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
            "Queue"="Geometry"
        }
        UniversalPipeline
        LOD 200

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "./ProceduralTree.cginc"

            sampler2D _MainTex;

            struct appdata {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float2 uv_MainTex : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            half _Glossiness;
            half _Metallic;
            fixed4 _Color;

            v2f vert(appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv_MainTex = v.texcoord;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                fixed2 uv_MainTex = i.uv_MainTex;
                procedural_tree_clip(uv_MainTex);
                fixed4 c = tex2D(_MainTex, uv_MainTex) * _Color;
                fixed4 col;
                col.rgb = c.rgb;
                col.a = c.a;
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
