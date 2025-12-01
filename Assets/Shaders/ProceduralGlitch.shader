Shader "UI/ProceduralGlitch"
{
    Properties
    {
        _Color ("Base Color", Color) = (1,1,1,1)

        [Header(Glitch Lines)]
        _LineCount ("Line Count", Range(5, 100)) = 30
        _LineSpeed ("Line Speed", Range(0, 20)) = 8.0
        _LineThickness ("Line Thickness", Range(0.001, 0.1)) = 0.02
        _LineIntensity ("Line Intensity", Range(0, 2)) = 1.0

        [Header(Wave Distortion)]
        _WaveAmount ("Wave Amount", Range(0, 0.5)) = 0.1
        _WaveSpeed ("Wave Speed", Range(0, 20)) = 5.0

        [Header(RGB Colors)]
        _ColorSpeed ("Color Speed", Range(0, 10)) = 3.0
        _ColorIntensity ("Color Intensity", Range(0, 2)) = 1.0

        [Header(Noise)]
        _NoiseScale ("Noise Scale", Range(1, 50)) = 10.0
        _NoiseAmount ("Noise Amount", Range(0, 1)) = 0.3

        [Header(UI)]
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            fixed4 _Color;
            float _LineCount;
            float _LineSpeed;
            float _LineThickness;
            float _LineIntensity;
            float _WaveAmount;
            float _WaveSpeed;
            float _ColorSpeed;
            float _ColorIntensity;
            float _NoiseScale;
            float _NoiseAmount;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(v.vertex);
                OUT.texcoord = v.texcoord;
                OUT.color = v.color * _Color;
                return OUT;
            }

            float rand(float2 co)
            {
                return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
            }

            float noise(float2 uv)
            {
                float2 i = floor(uv);
                float2 f = frac(uv);
                f = f * f * (3.0 - 2.0 * f);

                float a = rand(i);
                float b = rand(i + float2(1.0, 0.0));
                float c = rand(i + float2(0.0, 1.0));
                float d = rand(i + float2(1.0, 1.0));

                return lerp(lerp(a, b, f.x), lerp(c, d, f.x), f.y);
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                float2 uv = IN.texcoord;
                float time = _Time.y;

                // Wave distortion
                float wave = sin(uv.y * 10.0 + time * _WaveSpeed) * _WaveAmount;
                wave += cos(uv.x * 8.0 - time * _WaveSpeed * 0.7) * _WaveAmount * 0.5;
                uv.x += wave;
                uv.y += wave * 0.5;

                // Horizontal glitch lines
                float lineIndex = floor(uv.y * _LineCount + time * _LineSpeed);
                float lineRandom = rand(float2(lineIndex, floor(time * _LineSpeed)));

                float glitchLine = 0.0;
                if (lineRandom > 0.8)
                {
                    float lineY = frac(uv.y * _LineCount + time * _LineSpeed);
                    if (lineY < _LineThickness)
                    {
                        glitchLine = _LineIntensity;
                    }
                }

                // Vertical glitch lines
                float vertLineIndex = floor(uv.x * _LineCount * 0.5 + time * _LineSpeed * 1.3);
                float vertRandom = rand(float2(vertLineIndex, floor(time * _LineSpeed * 1.5)));

                if (vertRandom > 0.9)
                {
                    float lineX = frac(uv.x * _LineCount * 0.5 + time * _LineSpeed * 1.3);
                    if (lineX < _LineThickness * 2.0)
                    {
                        glitchLine += _LineIntensity * 0.5;
                    }
                }

                // Procedural noise background
                float n = noise(uv * _NoiseScale + time * 2.0);
                n += noise(uv * _NoiseScale * 2.0 - time * 3.0) * 0.5;
                n *= _NoiseAmount;

                // Psychedelic RGB colors
                float r = sin(uv.x * 5.0 + time * _ColorSpeed) * 0.5 + 0.5;
                float g = sin(uv.y * 5.0 + time * _ColorSpeed * 1.3) * 0.5 + 0.5;
                float b = sin((uv.x + uv.y) * 5.0 - time * _ColorSpeed * 0.8) * 0.5 + 0.5;

                fixed4 col = fixed4(r, g, b, 1.0) * _ColorIntensity;

                // Add glitch lines
                col.rgb += glitchLine;

                // Add noise
                col.rgb += n;

                // Random flicker
                float flicker = rand(float2(floor(time * 30.0), 0)) * 0.2 + 0.8;
                col.rgb *= flicker;

                // Apply vertex color
                col *= IN.color;

                return col;
            }
            ENDCG
        }
    }
}
