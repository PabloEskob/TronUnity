Shader "Malbers/DragonEyesURP"
{
    Properties
    {
        _Color0 ("Color 0", Color) = (1,0,0,0)
        _Color1 ("Color 1", Color) = (1,0.9310346,0,0)
        _Emission ("Emission", Range(0,1.5)) = 0
        [NoScaleOffset] _Iris ("Iris", 2D) = "white" {}
        [NoScaleOffset] _DragonPupil ("Dragon Pupil", 2D) = "white" {}
        [NoScaleOffset] _BloodPupil ("Blood Pupil", 2D) = "white" {}
        [NoScaleOffset] _CatPupil ("Cat Pupil", 2D) = "white" {}
        [Toggle] _ToggleSwitch0 ("Cat Eyes", Float) = 0
        [Toggle] _ToggleSwitch1 ("Blood Eyes", Float) = 0
        _PupilColor ("Pupil Color", Color) = (1,1,1,0)
        _Smooth ("Smooth", Range(0,1)) = 0
        _Metallic ("Metallic", Range(0,1)) = 0
        _Hue ("Hue", Range(0,1)) = 0
        _Saturation ("Saturation", Float) = 2
        _Lightness ("Lightness", Float) = 2
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 300

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma target 4.5

            // --------------------------------------------------
            // Universal RP keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_fog

            // Vertex / fragment
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            // --------------------------------------------------
            // Material uniforms
            CBUFFER_START(UnityPerMaterial)
                float4 _Color0;
                float4 _Color1;
                float4 _PupilColor;
                float   _Emission;
                float   _Hue;
                float   _Saturation;
                float   _Lightness;
                float   _ToggleSwitch0;
                float   _ToggleSwitch1;
                float   _Metallic;
                float   _Smooth;
            CBUFFER_END

            // Textures & samplers
            TEXTURE2D(_Iris);        SAMPLER(sampler_Iris);
            TEXTURE2D(_DragonPupil); SAMPLER(sampler_DragonPupil);
            TEXTURE2D(_BloodPupil);  SAMPLER(sampler_BloodPupil);
            TEXTURE2D(_CatPupil);    SAMPLER(sampler_CatPupil);

            // --------------------------------------------------
            // Utility: RGB <-> HSV
            float3 HSVToRGB(float3 c)
            {
                float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
                return c.z * lerp(K.xxx, saturate(p - K.xxx), c.y);
            }

            float3 RGBToHSV(float3 c)
            {
                float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
                float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));
                float d = q.x - min(q.w, q.y);
                float e = 1.0e-10;
                return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
            }

            // --------------------------------------------------
            // Vertex / fragment structs
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 normalWS   : TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            // --------------------------------------------------
            // Vertex
            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.normalWS   = TransformObjectToWorldNormal(IN.normalOS);

                OUT.positionCS = TransformWorldToHClip(OUT.positionWS);
                OUT.uv         = IN.uv;
                return OUT;
            }

            // --------------------------------------------------
            // Helpers to build SurfaceData
            void BuildSurfaceData(float2 uv, out SurfaceData surfaceData)
            {
                ZERO_INITIALIZE(SurfaceData, surfaceData);

                // -- Sample iris base
                float4 irisSample = SAMPLE_TEXTURE2D(_Iris, sampler_Iris, uv);

                // -- Build pupil mask
                float dragonA = SAMPLE_TEXTURE2D(_DragonPupil, sampler_DragonPupil, uv).a;
                float catA    = SAMPLE_TEXTURE2D(_CatPupil, sampler_CatPupil, uv).a;
                float bloodA  = SAMPLE_TEXTURE2D(_BloodPupil, sampler_BloodPupil, uv).a;

                float pupilMask = lerp(dragonA, catA, _ToggleSwitch0);
                pupilMask       = lerp(pupilMask, bloodA, _ToggleSwitch1);

                float4 pupilBlend = lerp(irisSample, _PupilColor, pupilMask);

                // -- Radial gradient (matches original surface shader)
                float2 uvStart  = float2(0.1, 0.1);
                float2 uvEnd    = float2(1.0, 1.0);
                float2 scaleMin = float2(-0.91, -0.96);
                float2 scaleMax = float2(1.27,  1.20);

                float2 scaled = scaleMin + (uv - uvStart) * (scaleMax - scaleMin) / (uvEnd - uvStart);
                float  r      = length(scaled);

                float4 radialColor  = lerp(_Color0, _Color1, r * -2.3 + 0.89);
                float4 radialValue  = (r * 10000.0 - 5598.0).xxxx;
                float4 radialCombine = saturate(max(radialColor, radialValue));

                float4 baseCol = saturate(pupilBlend * radialCombine);

                // -- HSV tweak
                float3 hsv = RGBToHSV(baseCol.rgb);
                hsv        = float3(_Hue, _Saturation * hsv.y, hsv.z * _Lightness);
                float3 albedoRGB = HSVToRGB(hsv);

                // -- Populate SurfaceData (PBR)
                surfaceData.albedo     = albedoRGB;
                surfaceData.metallic   = _Metallic;
                surfaceData.smoothness = _Smooth;
                surfaceData.normalTS   = float3(0,0,1);
                surfaceData.occlusion  = 1.0;
                surfaceData.alpha      = 1.0;
                surfaceData.specular   = 0;
                surfaceData.emission   = baseCol.rgb * _Emission;
                surfaceData.clearCoatMask        = 0;
                surfaceData.clearCoatSmoothness  = 0;
            }

            // --------------------------------------------------
            // Fragment
            half4 frag (Varyings IN) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN);

                // Build surface data
                SurfaceData surfaceData;
                BuildSurfaceData(IN.uv, surfaceData);

                // Prepare InputData for lighting
                InputData inputData;
                ZERO_INITIALIZE(InputData, inputData);
                inputData.positionWS      = IN.positionWS;
                inputData.normalWS        = normalize(IN.normalWS);
                inputData.viewDirectionWS = normalize(GetWorldSpaceViewDir(IN.positionWS));
                #if defined(_MAIN_LIGHT_SHADOWS) || defined(_MAIN_LIGHT_SHADOWS_CASCADE)
                    inputData.shadowCoord = TransformWorldToShadowCoord(IN.positionWS);
                #endif

                // Evaluate lighting via URP helper
                half4 color = UniversalFragmentPBR(inputData, surfaceData);
                return color;
            }
            ENDHLSL
        }
    }

    Fallback "Hidden/Universal Render Pipeline/FallbackError"
}
