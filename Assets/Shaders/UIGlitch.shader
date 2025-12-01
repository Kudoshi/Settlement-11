Shader "UI/Glitch"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        [Header(Glitch Settings)]
        _GlitchIntensity ("Glitch Intensity", Range(0, 1)) = 0.5
        _GlitchSpeed ("Glitch Speed", Range(0, 10)) = 2.0
        _FlashSpeed ("Flash Speed", Range(0, 20)) = 5.0
        _FlashIntensity ("Flash Intensity", Range(0, 2)) = 1.0

        [Header(Color Shift)]
        _ColorShiftAmount ("Color Shift", Range(0, 0.1)) = 0.02

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
            float _GlitchIntensity;
            float _GlitchSpeed;
            float _FlashSpeed;
            float _FlashIntensity;
            float _ColorShiftAmount;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(v.vertex);
                OUT.texcoord = v.texcoord;
                OUT.color = v.color * _Color;
                return OUT;
            }

            // Random function
            float rand(float2 co)
            {
                return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                float2 uv = IN.texcoord;
                float time = _Time.y;

                // Glitch horizontal displacement
                float glitchLine = floor(uv.y * 10.0 + time * _GlitchSpeed);
                float glitchAmount = rand(float2(glitchLine, floor(time * _GlitchSpeed))) * _GlitchIntensity;

                // Random glitch displacement
                if (rand(float2(glitchLine, floor(time * _GlitchSpeed * 2))) > 0.9)
                {
                    uv.x += glitchAmount * 0.1 - 0.05;
                }

                // RGB color shift for chromatic aberration effect
                float shift = _ColorShiftAmount * _GlitchIntensity;
                fixed4 colR = tex2D(_MainTex, uv + float2(shift, 0));
                fixed4 colG = tex2D(_MainTex, uv);
                fixed4 colB = tex2D(_MainTex, uv - float2(shift, 0));

                fixed4 col = fixed4(colR.r, colG.g, colB.b, colG.a);

                // Flashing effect
                float flash = sin(time * _FlashSpeed) * 0.5 + 0.5;
                flash = pow(flash, 3.0); // Make flash punchier
                col.rgb += flash * _FlashIntensity * _GlitchIntensity;

                // Random brightness flicker
                float flicker = rand(float2(floor(time * 10.0), 0)) * 0.3 + 0.7;
                col.rgb *= flicker;

                // Apply tint and vertex color
                col *= IN.color;

                return col;
            }
            ENDCG
        }
    }
}
