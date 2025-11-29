Shader "Custom/DrippyOverlay"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _TintColor ("Tint Color", Color) = (1, 1, 1, 1)
        _TintStrength ("Tint Strength", Range(0, 1)) = 0.1
        _DripSpeed ("Drip Speed", Range(0, 5)) = 1
        _DripScale ("Drip Scale", Range(0.1, 10)) = 2
        _DripIntensity ("Drip Intensity", Range(0, 2)) = 0.5
        _OverlayColor1 ("Overlay Color 1", Color) = (1, 0.3, 0.5, 1)
        _OverlayColor2 ("Overlay Color 2", Color) = (0.3, 0.5, 1, 1)
        _OverlayStrength ("Overlay Strength", Range(0, 1)) = 0.2
        _WaveSpeed ("Wave Speed", Range(0, 10)) = 2
        _WaveAmount ("Wave Amount", Range(0, 1)) = 0.1
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _MainTex;
        float4 _TintColor;
        float _TintStrength;
        float _DripSpeed;
        float _DripScale;
        float _DripIntensity;
        float4 _OverlayColor1;
        float4 _OverlayColor2;
        float _OverlayStrength;
        float _WaveSpeed;
        float _WaveAmount;
        float _Glossiness;
        float _Metallic;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        float hash(float2 p)
        {
            p = frac(p * float2(123.45, 678.90));
            p += dot(p, p + 45.32);
            return frac(p.x * p.y);
        }

        float noise(float2 p)
        {
            float2 i = floor(p);
            float2 f = frac(p);
            f = f * f * (3.0 - 2.0 * f);

            float a = hash(i);
            float b = hash(i + float2(1.0, 0.0));
            float c = hash(i + float2(0.0, 1.0));
            float d = hash(i + float2(1.0, 1.0));

            return lerp(lerp(a, b, f.x), lerp(c, d, f.x), f.y);
        }

        float fbm(float2 p)
        {
            float value = 0.0;
            float amplitude = 0.5;
            for(int i = 0; i < 4; i++)
            {
                value += amplitude * noise(p);
                p *= 2.0;
                amplitude *= 0.5;
            }
            return value;
        }

        float drip(float2 uv, float time)
        {
            float2 p = uv * _DripScale;

            float drips = 0.0;
            for(int i = 0; i < 3; i++)
            {
                float offset = float(i) * 123.456;
                float x = p.x + offset;
                float speed = time * _DripSpeed + offset;

                float dripNoise = noise(float2(x * 2.0, 0.0));
                float dripY = frac(p.y - speed * (0.5 + dripNoise * 0.5));

                float dripWidth = 0.1 + dripNoise * 0.05;
                float dripX = frac(x) - 0.5;

                float dripMask = smoothstep(dripWidth, 0.0, abs(dripX));
                dripMask *= smoothstep(1.0, 0.8, dripY);
                dripMask *= (1.0 - dripY);

                drips += dripMask;
            }

            return saturate(drips * _DripIntensity);
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float4 baseColor = tex2D(_MainTex, IN.uv_MainTex);

            float time = _Time.y;

            float dripEffect = drip(IN.uv_MainTex, time);

            float2 waveUV = IN.uv_MainTex;
            waveUV.x += sin(IN.uv_MainTex.y * 10.0 + time * _WaveSpeed) * 0.02 * _WaveAmount;
            waveUV.y += cos(IN.uv_MainTex.x * 10.0 + time * _WaveSpeed * 0.7) * 0.02 * _WaveAmount;

            float fbmValue = fbm(waveUV * 5.0 + time * 0.1);

            float wave1 = sin(waveUV.x * 8.0 + time * _WaveSpeed + fbmValue * 3.0) * 0.5 + 0.5;
            float wave2 = sin(waveUV.y * 6.0 + time * _WaveSpeed * 1.3 + fbmValue * 2.0) * 0.5 + 0.5;

            float4 overlayColor = lerp(_OverlayColor1, _OverlayColor2, wave1 * wave2);

            float overlayMask = (wave1 * wave2 * 0.1) + (fbmValue * 0.1) + (dripEffect * 0.2);
            overlayMask *= _OverlayStrength;

            float4 tintedColor = lerp(baseColor, baseColor * _TintColor, _TintStrength);

            float4 finalColor = lerp(tintedColor, overlayColor, overlayMask);

            float pulse = sin(time * 1.5) * 0.5 + 0.5;
            finalColor.rgb += dripEffect * _OverlayColor2.rgb * pulse * 0.1 * _OverlayStrength;

            o.Albedo = finalColor.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness + dripEffect * 0.3;
            o.Alpha = 1.0;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
