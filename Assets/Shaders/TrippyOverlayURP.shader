Shader "URP/TrippyOverlay"
{
    Properties
    {
        [MainTexture] _BaseMap ("Base Texture", 2D) = "white" {}
        [MainColor] _BaseColor ("Base Color", Color) = (1,1,1,1)
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _BumpScale ("Normal Scale", Float) = 1.0
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Smoothness ("Smoothness", Range(0,1)) = 0.5

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
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
        }
        LOD 300

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
                float4 tangentWS : TEXCOORD3;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            TEXTURE2D(_BumpMap);
            SAMPLER(sampler_BumpMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                float4 _BaseColor;
                float _BumpScale;
                float _Metallic;
                float _Smoothness;
                float _EffectIntensity;
                float _FlowSpeed;
                float _FlowScale;
                float _ColorShift;
                float _RainbowIntensity;
                float _PulseSpeed;
                float _WarpAmount;
            CBUFFER_END

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

            Varyings vert(Attributes input)
            {
                Varyings output;

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);

                output.positionCS = vertexInput.positionCS;
                output.positionWS = vertexInput.positionWS;
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                output.normalWS = normalInput.normalWS;
                output.tangentWS = float4(normalInput.tangentWS, input.tangentOS.w);

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float time = _Time.y;

                float2 uv = input.uv;
                float2 flowUV = uv * _FlowScale;

                float n1 = fbm(flowUV + time * _FlowSpeed * 0.1);
                float n2 = fbm(flowUV * 1.3 - time * _FlowSpeed * 0.15);

                float2 warp = float2(n1, n2) * _WarpAmount;
                float2 warpedUV = uv + warp;

                half4 baseColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, warpedUV) * _BaseColor;

                float flow = fbm(flowUV + time * _FlowSpeed * 0.2);
                flow = flow * 2.0 - 1.0;

                float pulse = sin(time * _PulseSpeed + n1 * 3.14159) * 0.5 + 0.5;

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

                half3 normalTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, warpedUV), _BumpScale);
                float3 bitangentWS = input.tangentWS.w * cross(input.normalWS, input.tangentWS.xyz);
                float3 normalWS = TransformTangentToWorld(normalTS, half3x3(input.tangentWS.xyz, bitangentWS, input.normalWS));

                InputData lightingInput = (InputData)0;
                lightingInput.positionWS = input.positionWS;
                lightingInput.normalWS = normalize(normalWS);
                lightingInput.viewDirectionWS = GetWorldSpaceNormalizeViewDir(input.positionWS);
                lightingInput.shadowCoord = TransformWorldToShadowCoord(input.positionWS);

                SurfaceData surfaceData = (SurfaceData)0;
                surfaceData.albedo = finalColor;
                surfaceData.metallic = _Metallic;
                surfaceData.smoothness = _Smoothness + pulse * _EffectIntensity * 0.1;
                surfaceData.normalTS = normalTS;
                surfaceData.alpha = 1.0;

                return UniversalFragmentPBR(lightingInput, surfaceData);
            }
            ENDHLSL
        }

        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

            ZWrite On
            ZTest LEqual

            HLSLPROGRAM
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            ENDHLSL
        }

        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode" = "DepthOnly" }

            ZWrite On
            ColorMask 0

            HLSLPROGRAM
            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
            ENDHLSL
        }
    }
    FallBack "Universal Render Pipeline/Lit"
}
