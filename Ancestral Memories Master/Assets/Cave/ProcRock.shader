Shader "Custom/ProcRock"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseAmount ("Noise Amount", float) = 0.1
        _LightColor ("Light Color", Color) = (1, 1, 1, 1)
        _SkyColour ("Sky Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // Add these lines to declare your new properties.
            uniform float4 _LightColor;
            uniform float4 _SkyColour;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float _NoiseAmount;

            float noise(float2 uv)
            {
                return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
            }

            v2f vert(appdata v)
            {
                v2f o;
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float2 uniqueOffset = worldPos.xz;
                float n = noise(v.uv + uniqueOffset);
                float3 displaced = v.vertex.xyz + v.normal * n * _NoiseAmount;

                o.vertex = UnityObjectToClipPos(float4(displaced, 1.0));
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;

            half4 frag(v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv);
                col *= _LightColor;  // Utilize the light color
                // Use _SkyColour somewhere if needed
                
                return col;
            }
            ENDCG
        }
    }
}
