Shader "Skybox/Psychedelic"
{
    Properties
    {
        _ColorA ("Color A", Color) = (1, 0, 0.5, 1)
        _ColorB ("Color B", Color) = (0, 0.5, 1, 1)
        _ColorC ("Color C", Color) = (1, 1, 0, 1)
        _Speed ("Speed", Range(0, 5)) = 1
        _Scale ("Scale", Range(0.1, 10)) = 1
        _StarDensity ("Star Density", Range(0, 1)) = 0.5
        _StarBrightness ("Star Brightness", Range(0, 2)) = 1
        _Twist ("Twist Amount", Range(0, 5)) = 1
        _Intensity ("Skybox Intensity", Range(0, 2)) = 1
        _Exposure ("Exposure", Range(0, 3)) = 1
    }

    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
        Cull Off ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 uv : TEXCOORD0;
            };

            float4 _ColorA;
            float4 _ColorB;
            float4 _ColorC;
            float _Speed;
            float _Scale;
            float _StarDensity;
            float _StarBrightness;
            float _Twist;
            float _Intensity;
            float _Exposure;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float hash(float3 p)
            {
                p = frac(p * 0.3183099 + 0.1);
                p *= 17.0;
                return frac(p.x * p.y * p.z * (p.x + p.y + p.z));
            }

            float noise(float3 x)
            {
                float3 p = floor(x);
                float3 f = frac(x);
                f = f * f * (3.0 - 2.0 * f);

                return lerp(lerp(lerp(hash(p + float3(0,0,0)), hash(p + float3(1,0,0)), f.x),
                                 lerp(hash(p + float3(0,1,0)), hash(p + float3(1,1,0)), f.x), f.y),
                            lerp(lerp(hash(p + float3(0,0,1)), hash(p + float3(1,0,1)), f.x),
                                 lerp(hash(p + float3(0,1,1)), hash(p + float3(1,1,1)), f.x), f.y), f.z);
            }

            float fbm(float3 p)
            {
                float value = 0.0;
                float amplitude = 0.5;
                for(int i = 0; i < 5; i++)
                {
                    value += amplitude * noise(p);
                    p *= 2.0;
                    amplitude *= 0.5;
                }
                return value;
            }

            float stars(float3 dir)
            {
                float3 p = dir * 100.0;
                float star = hash(floor(p));
                return step(1.0 - _StarDensity, star) * _StarBrightness * (0.5 + 0.5 * sin(_Time.y * 10.0 * star));
            }

            float4 frag(v2f i) : SV_Target
            {
                float3 dir = normalize(i.uv);

                float angle = atan2(dir.x, dir.z);
                float elevation = asin(dir.y);

                float time = _Time.y * _Speed;

                float2 uv = float2(angle, elevation) * _Scale;
                uv += float2(time * 0.1, time * 0.05);

                float twist = length(dir.xz) * _Twist;
                float s = sin(twist + time);
                float c = cos(twist + time);
                uv = float2(uv.x * c - uv.y * s, uv.x * s + uv.y * c);

                float n1 = fbm(float3(uv * 2.0, time * 0.5));
                float n2 = fbm(float3(uv * 3.0 + 100.0, time * 0.3));
                float n3 = fbm(float3(uv * 4.0 + 200.0, time * 0.7));

                float wave1 = sin(uv.x * 3.0 + time * 2.0 + n1 * 5.0) * 0.5 + 0.5;
                float wave2 = sin(uv.y * 4.0 + time * 3.0 + n2 * 5.0) * 0.5 + 0.5;
                float wave3 = sin((uv.x + uv.y) * 2.0 + time * 1.5 + n3 * 5.0) * 0.5 + 0.5;

                float4 color = _ColorA * wave1 + _ColorB * wave2 + _ColorC * wave3;
                color = normalize(color);

                color += lerp(_ColorA, _ColorB, n1);
                color += lerp(_ColorB, _ColorC, n2);
                color += lerp(_ColorC, _ColorA, n3);

                float pulse = sin(time * 2.0) * 0.5 + 0.5;
                color *= 0.5 + pulse * 0.5;

                float starField = stars(dir);
                color.rgb += starField;

                color.rgb *= _Intensity;
                color.rgb *= _Exposure;

                return color;
            }
            ENDCG
        }
    }
}
