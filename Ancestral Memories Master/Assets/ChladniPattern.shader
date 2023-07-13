Shader "Custom/ChladniPattern"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Frequency ("Frequency", Range(1, 10)) = 1
        _Amplitude ("Amplitude", Range(0.1, 1)) = 1
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
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _Frequency;
            float _Amplitude;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                uv = uv * 2 - 1; // remap uv to (-1, 1)
                
                float d = length(uv);
                float pattern = sin(d * _Frequency - _Time.y) * _Amplitude;
                
                pattern = pattern * 0.5 + 0.5; // remap pattern to (0, 1)
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                
                return lerp(col, _Color, pattern);
            }
            ENDCG
        }
    }
}
