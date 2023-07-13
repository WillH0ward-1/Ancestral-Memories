Shader "Custom/SeamlessWaveSurface" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1, 1, 1, 1)
        _Amplitude ("Amplitude", Range(0, 1)) = 0.1
        _TimeMultiplier ("Time Multiplier", Range(0, 10)) = 1
        _WaveScale ("Wave Scale", Range(0, 10)) = 1
        _RefractionStrength ("Refraction Strength", Range(0, 1)) = 0.1
        _Tiling ("Tiling", Range(1, 10000)) = 1
        _Offset ("Offset", Range(0, 1)) = 0
        _NoiseScale ("Noise Scale", Range(0.1, 10)) = 1
        _NoiseSpeed ("Noise Speed", Range(0.1, 10)) = 1
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            uniform sampler2D _MainTex;
            uniform fixed4 _Color;
            uniform float _Amplitude;
            uniform float _TimeMultiplier;
            uniform float _WaveScale;
            uniform float _RefractionStrength;
            uniform float _Tiling;
            uniform float _Offset;
            uniform float _NoiseScale;
            uniform float _NoiseSpeed;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata v) {
                v2f o;
                float time = _Time.y * _TimeMultiplier;
                float wave = sin(v.vertex.x * _WaveScale + time) * _Amplitude;
                o.vertex = UnityObjectToClipPos(v.vertex + float4(0, wave, 0, 0));
                o.uv = v.uv;
                return o;
            }

            float Random(float2 p) {
                float2 uv = (frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453) * 2.0) - 1.0;
                return clamp(uv.x + uv.y, 0.0, 1.0);
            }

            float2 PerlinNoise(float2 uv, float time) {
                float x = (uv.x + time) * _NoiseScale;
                float y = (uv.y + time) * _NoiseScale;
                float2 p = floor(float2(x, y));
                float2 f = frac(float2(x, y));

                f = f * f * (3.0 - 2.0 * f);
                float n = p.x + p.y * 57.0;
                return lerp(lerp(Random(n), Random(n + 1.0), f.x),
                            lerp(Random(n + 57.0), Random(n + 58.0), f.x), f.y);
            }

            fixed4 frag (v2f i) : SV_Target {
                float2 refractionOffset = float2(0.0, 0.0);
                float2 center = float2(0.5, 0.5);
                float2 delta = i.uv - center;
                float distance = length(delta);
                float distortion = PerlinNoise(i.uv, _Time.y * _NoiseSpeed);
                float2 direction = delta / distance;

                refractionOffset = direction * (distortion * _RefractionStrength * distance);
                float2 uv = (i.uv * _Tiling + _Offset) + refractionOffset;

                return _Color * tex2D(_MainTex, uv);
            }

            ENDCG
        }
    }

    FallBack "Diffuse"
}
