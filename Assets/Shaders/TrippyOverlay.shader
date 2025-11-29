Shader "Custom/TrippyOverlay"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Glossiness ("Smoothness", Range(0,1)) = 0.5

        [Header(Trippy Effects)]
        _EffectIntensity ("Effect Intensity", Range(0, 1)) = 0.15
        _FlowSpeed ("Flow Speed", Range(0, 5)) = 1
        _FlowScale ("Flow Scale", Range(0.1, 20)) = 5
        _ColorShift ("Color Shift Amount", Range(0, 0.5)) = 0.1
        _RainbowIntensity ("Rainbow Intensity", Range(0, 1)) = 0.2
        _PulseSpeed ("Pulse Speed", Range(0, 10)) = 2
        _WarpAmount ("Warp Amount", Range(0, 0.1)) = 0.02
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _BumpMap;
        float _Metallic;
        float _Glossiness;
        float _EffectIntensity;
        float _FlowSpeed;
        float _FlowScale;
        float _ColorShift;
        float _RainbowIntensity;
        float _PulseSpeed;
        float _WarpAmount;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        float hash21(float2 p)
        {
            p = frac(p * float2(234.34, 435.345));
            p += dot(p, p + 34.23);
            return frac(p.x * p.y);
        }

        float noise(float2 p)
        {
            float2 i = floor(p);
            float2 f = frac(p);
            f = f * f * (3.0 - 2.0 * f);

            return lerp(
                lerp(hash21(i), hash21(i + float2(1,0)), f.x),
                lerp(hash21(i + float2(0,1)), hash21(i + float2(1,1)), f.x),
                f.y
            );
        }

        float fbm(float2 p)
        {
            float v = 0.0;
            float a = 0.5;
            for(int i = 0; i < 3; i++)
            {
                v += a * noise(p);
                p *= 2.0;
                a *= 0.5;
            }
            return v;
        }

        float3 rainbow(float t)
        {
            float3 c = 0.5 + 0.5 * cos(6.28318 * (t + float3(0.0, 0.33, 0.67)));
            return c;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float time = _Time.y;

            float2 uv = IN.uv_MainTex;
            float2 flowUV = uv * _FlowScale;

            float n1 = fbm(flowUV + time * _FlowSpeed * 0.1);
            float n2 = fbm(flowUV * 1.3 - time * _FlowSpeed * 0.15);

            float2 warp = float2(n1, n2) * _WarpAmount;
            float2 warpedUV = uv + warp;

            float4 baseColor = tex2D(_MainTex, warpedUV);

            float flow = fbm(flowUV + time * _FlowSpeed * 0.2);
            flow = flow * 2.0 - 1.0;

            float pulse = sin(time * _PulseSpeed + n1 * 3.14) * 0.5 + 0.5;

            float rainbowPhase = frac(flow * 0.5 + time * 0.1);
            float3 rainbowColor = rainbow(rainbowPhase);

            float hueShift = (n1 - 0.5) * _ColorShift;
            float3 shiftedColor = baseColor.rgb;
            shiftedColor.r = baseColor.r + hueShift;
            shiftedColor.g = baseColor.g + hueShift * 0.5;
            shiftedColor.b = baseColor.b - hueShift;

            float3 finalColor = lerp(baseColor.rgb, shiftedColor, _EffectIntensity);

            finalColor = lerp(finalColor, finalColor * rainbowColor, _RainbowIntensity * _EffectIntensity);

            float brightnessBoost = pulse * _EffectIntensity * 0.1;
            finalColor += brightnessBoost;

            o.Albedo = finalColor;
            o.Normal = UnpackNormal(tex2D(_BumpMap, warpedUV));
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness + pulse * _EffectIntensity * 0.1;
            o.Alpha = 1.0;
        }
        ENDCG
    }
    FallBack "Standard"
}
