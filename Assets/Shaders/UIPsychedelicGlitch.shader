Shader "UI/PsychedelicGlitch"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        [Header(Glitch Lines)]
        _LineCount ("Line Count", Range(5, 50)) = 20
        _LineSpeed ("Line Speed", Range(0, 20)) = 5.0
        _LineOffset ("Line Offset", Range(0, 0.2)) = 0.05

        [Header(Wave Distortion)]
        _WaveAmount ("Wave Amount", Range(0, 0.1)) = 0.03
        _WaveSpeed ("Wave Speed", Range(0, 10)) = 3.0
        _WaveFrequency ("Wave Frequency", Range(1, 20)) = 5.0

        [Header(RGB Split)]
        _RGBSplitAmount ("RGB Split", Range(0, 0.05)) = 0.01
        _SplitSpeed ("Split Speed", Range(0, 10)) = 2.0

        [Header(Scanlines)]
        _ScanlineIntensity ("Scanline Intensity", Range(0, 1)) = 0.3
        _ScanlineCount ("Scanline Count", Range(10, 200)) = 100

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
            "CanUseSpriteAtlas"="True"
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

            sampler2D _MainTex;
            fixed4 _Color;
            float _LineCount;
            float _LineSpeed;
            float _LineOffset;
            float _WaveAmount;
            float _WaveSpeed;
            float _WaveFrequency;
            float _RGBSplitAmount;
            float _SplitSpeed;
            float _ScanlineIntensity;
            float _ScanlineCount;

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

            fixed4 frag(v2f IN) : SV_Target
            {
                float2 uv = IN.texcoord;
                float time = _Time.y;

                // Random glitch lines
                float lineIndex = floor(uv.y * _LineCount);
                float lineRandom = rand(float2(lineIndex, floor(time * _LineSpeed)));

                // Only glitch some lines randomly
                if (lineRandom > 0.7)
                {
                    float offset = (rand(float2(lineIndex, floor(time * _LineSpeed * 2))) - 0.5) * _LineOffset;
                    uv.x += offset;
                }

                // Wave distortion
                float wave = sin(uv.y * _WaveFrequency + time * _WaveSpeed) * _WaveAmount;
                wave += sin(uv.y * _WaveFrequency * 2.3 - time * _WaveSpeed * 1.5) * _WaveAmount * 0.5;
                uv.x += wave;

                // Psychedelic RGB split
                float splitOffset = sin(time * _SplitSpeed) * _RGBSplitAmount;
                float splitY = sin(uv.y * 10.0 + time * _SplitSpeed * 2.0) * _RGBSplitAmount * 0.5;

                float2 uvR = uv + float2(splitOffset, splitY);
                float2 uvG = uv;
                float2 uvB = uv - float2(splitOffset, splitY);

                fixed4 colR = tex2D(_MainTex, uvR);
                fixed4 colG = tex2D(_MainTex, uvG);
                fixed4 colB = tex2D(_MainTex, uvB);

                fixed4 col = fixed4(colR.r, colG.g, colB.b, colG.a);

                // Scanlines
                float scanline = sin(uv.y * _ScanlineCount + time * 2.0) * 0.5 + 0.5;
                scanline = lerp(1.0, scanline, _ScanlineIntensity);
                col.rgb *= scanline;

                // Random vertical line glitches
                float verticalGlitch = step(0.98, rand(float2(floor(uv.x * 100.0), floor(time * 20.0))));
                col.rgb += verticalGlitch * 0.5;

                // Apply tint and vertex color
                col *= IN.color;

                return col;
            }
            ENDCG
        }
    }
}
