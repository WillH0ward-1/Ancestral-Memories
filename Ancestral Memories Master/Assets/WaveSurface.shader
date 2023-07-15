Shader "Custom/SeamlessWaveSurfaceURP" {
    Properties {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _Albedo ("Albedo", 2D) = "white" {}
        _Amplitude ("Amplitude", Range(0, 1)) = 0.1
        _TimeMultiplier ("Time Multiplier", Range(0, 10)) = 1
        _WaveScale ("Wave Scale", Range(0, 10)) = 1
        _Tiling ("Tiling", Range(1, 10000)) = 1
        _NoiseScale ("Noise Scale", Range(0.1, 10)) = 1
        _NoiseSpeed ("Noise Speed", Range(0.1, 10)) = 1
        _FresnelPower ("Fresnel Power", Range(1, 5)) = 2
    }

    SubShader {
        Tags { "RenderType"="Opaque" "Queue"="Transparent" }
        LOD 100

        Pass {
            Stencil {
                Ref 1
                Comp always
                Pass keep
            }

            Cull Off
            ZWrite Off
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            fixed4 _Color;
            sampler2D _Albedo;
            float _Amplitude;
            float _TimeMultiplier;
            float _WaveScale;
            float _Tiling;
            float _NoiseScale;
            float _NoiseSpeed;
            float _FresnelPower;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
            };

            // Perlin Noise function helpers
            // These functions generate noise based on 2D input coordinates. 
            // We'll use these in the fragment shader to modify the color based on the noise.
            float2 random2(float2 p) {
                return frac(sin(float2(dot(p, float2(127.1, 311.7)), dot(p, float2(269.5, 183.3)))) * 43758.5453);
            }

            float noise(float2 p) {
                float2 i = floor(p);
                float2 f = frac(p);
                f = f*f*(3.0-2.0*f);
                float a = dot(random2(i), f);
                float b = dot(random2(i + float2(1.0, 0.0)), f - float2(1.0, 0.0));
                float c = dot(random2(i + float2(0.0, 1.0)), f - float2(0.0, 1.0));
                float d = dot(random2(i + float2(1.0, 1.0)), f - float2(1.0, 1.0));
                return lerp(a, b, f.x) + (c-a)*f.y*(1.0-f.x) + (d-b)*f.x*f.y;
            }

            v2f vert (appdata v) {
                v2f o;
                float time = _Time.y * _TimeMultiplier;
                float wave = sin(v.vertex.x * _WaveScale + time) * _Amplitude;
                v.vertex.y += wave;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = normalize(_WorldSpaceCameraPos.xyz - v.vertex.xyz);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // Use the Perlin Noise function for color modulation
                float2 offset1 = float2(_Time.y * _NoiseSpeed, 0.0);
                float2 offset2 = float2(0.0, _Time.y * _NoiseSpeed);
                float noise1 = noise(i.uv * _Tiling + offset1) * _NoiseScale;
                float noise2 = noise(i.uv * _Tiling + offset2) * _NoiseScale;
                float noise = (noise1 + noise2) * 0.5;
                
                fixed4 col = _Color;
                fixed4 albedo = tex2D(_Albedo, i.uv);

                col.rgb *= noise; // color modulation based on noise
                col.rgb *= albedo.rgb; // color modulation based on albedo

                float fresnel = pow(1.0 - max(dot(normalize(i.viewDir), i.normal), 0.0), _FresnelPower);
                col.rgb += fresnel;

                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
