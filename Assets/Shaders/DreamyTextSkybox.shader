Shader "Skybox/DreamyText"
{
    Properties
    {
        _TopColor ("Top Gradient Color", Color) = (0.05, 0.0, 0.15, 1)
        _BottomColor ("Bottom Gradient Color", Color) = (0.0, 0.0, 0.05, 1)
        _TextTex ("Text Texture (Alpha is Opacity)", 2D) = "black" {}
        _TextColor ("Text Color", Color) = (1, 1, 1, 0.5)
        _TextTiling ("Text Tiling (X, Y)", Vector) = (5, 5, 0, 0)
        _ScrollSpeed ("Scroll Speed (X, Y)", Vector) = (0.02, 0.01, 0, 0)
        _Emission ("Emission Multiplier", Range(0, 2)) = 1.0
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
                float3 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 viewDir : TEXCOORD0;
            };

            fixed4 _TopColor;
            fixed4 _BottomColor;
            sampler2D _TextTex;
            float4 _TextTex_ST;
            fixed4 _TextColor;
            float4 _TextTiling;
            float4 _ScrollSpeed;
            float _Emission;

            #define PI 3.14159265359

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.viewDir = v.vertex.xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 dir = normalize(i.viewDir);
                
                // 1. Background Gradient (Vertical based on Y)
                // Remap Y from -1..1 to 0..1 for Lerp
                float gradientFactor = smoothstep(-0.5, 0.8, dir.y);
                fixed4 skyColor = lerp(_BottomColor, _TopColor, gradientFactor);

                // 2. Text Texture Mapping (Equirectangular-ish)
                // Calculate UVs from direction
                float2 skyUV;
                skyUV.x = atan2(dir.x, dir.z) / (2.0 * PI) + 0.5;
                skyUV.y = asin(dir.y) / PI + 0.5;

                // Apply Tiling and Scrolling
                float2 tiledUV = skyUV * _TextTiling.xy;
                tiledUV += _Time.y * _ScrollSpeed.xy;

                // Sample Text
                fixed4 textSample = tex2D(_TextTex, tiledUV);
                
                // Apply Text Color and Opacity
                // Assuming text texture is white-on-black or uses alpha. 
                // We'll use the texture's brightness/alpha to blend.
                float textAlpha = textSample.a * textSample.r; // Combine A and R for robustness
                
                fixed4 finalTextColor = _TextColor * textAlpha * _Emission;

                // 3. Composite
                // Additive blend or alpha blend? Additive usually looks "dreamier" for glowing text.
                return skyColor + finalTextColor;
            }
            ENDCG
        }
    }
}
