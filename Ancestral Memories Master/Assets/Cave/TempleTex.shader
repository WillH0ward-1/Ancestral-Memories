Shader "Custom/TempleTex"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MainTexTiling ("Tiling", Vector) = (1,1,0,0)
        _MainTexOffset ("Offset", Vector) = (0,0,0,0)
        _NoiseAmount ("Noise Amount", float) = 0.1
        _LightColor ("Light Color", Color) = (1, 1, 1, 1)
        _SkyColour ("Sky Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Cull Off // Disable face culling
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

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

            float noise(float3 position)
            {
                return frac(sin(dot(position, float3(12.9898, 78.233, 96.574))) * 43758.5453);
            }

            v2f vert(appdata v)
            {
                v2f o;
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float n = noise(worldPos);
                float3 displaced = v.vertex.xyz + v.normal * n * _NoiseAmount;

                o.vertex = UnityObjectToClipPos(float4(displaced, 1.0));
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float4 _MainTexTiling;
            float4 _MainTexOffset;

            half4 frag(v2f i) : SV_Target
            {
                float2 tiledUV = i.uv * _MainTexTiling.xy + _MainTexOffset.xy;
                half4 col = tex2D(_MainTex, tiledUV);
                col *= _LightColor;  // Utilize the light color
                
                return col;
            }
            ENDCG
        }
    }
}
