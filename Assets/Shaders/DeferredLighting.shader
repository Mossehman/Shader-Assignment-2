Shader "Unlit/DeferredLighting"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZWrite Off Cull Off
        Pass
        {
            Name "LightingPass"

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            // The Blit.hlsl file provides the vertex shader (Vert),
            // input structure (Attributes) and output strucutre (Varyings)

            #pragma vertex vert
            #pragma fragment frag

            // stores our color data
            TEXTURE2D(_gBuffer0);
            SAMPLER(sampler_gBuffer0);

            // stores our normal data
            TEXTURE2D(_gBuffer2);
            SAMPLER(sampler_gBuffer2);

            // stores camera depth
            TEXTURE2D(_CameraDepthTexture);
            SAMPLER(sampler_CameraDepthTexture);

            TEXTURE2D(_shadingTexture);
            SAMPLER(sampler_shadingTexture);
           
           float2 _shadingTiling = float2(5, 5);
           float _shadowBrightness;

            float3 _lightDir;
            float4 _lightColor;
            float _colorBanding = 0;
            
            struct VertexData{
                float4 position : POSITION;
                half2 uv : TEXCOORD0;
            };

            struct VertToFrag {

                float4 position : SV_POSITION;
                half2 uv : TEXCOORD0;
            };

            VertToFrag vert(VertexData vd)
            {
                VertToFrag v2f;
                v2f.position = TransformObjectToHClip(vd.position);
                v2f.uv = vd.uv;
                return v2f;
            }

            float roundToStep(float val, float step)
			{
                if (_colorBanding <= 0) return val;
                float stepSize = 1.0 / _colorBanding;
                return floor(val * _colorBanding) * stepSize;
			}

            half4 frag (VertToFrag input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                //check if pixel is sky or object (if sky, color pixel black, else color pixel to output color)
                //we need to do this because gBuffer data doesn't clear after the frame
                float depth = SAMPLE_TEXTURE2D_X(_CameraDepthTexture, sampler_CameraDepthTexture, input.uv).r;
                float isNotSky = -step(depth, 0) + 1;

                // Sample the GBuffer texture
                float4 colorData = SAMPLE_TEXTURE2D(_gBuffer0, sampler_gBuffer0, input.uv);
                float4 normalData = SAMPLE_TEXTURE2D(_gBuffer2, sampler_gBuffer2, input.uv);

                float2 tiledUV = input.uv * _shadingTiling;
                float2 normalDistortion = normalData.xy * 0.5f; // X and Y components for horizontal/vertical distortion
                normalDistortion *= (1.0 + normalData.z); // Scale distortion intensity based on the Z component
                float2 warpedUV = tiledUV + normalDistortion;


                float4 shadingCol = SAMPLE_TEXTURE2D(_shadingTexture, sampler_shadingTexture, warpedUV);
                float4 lighting = float4(0, 0, 0, 0);

                float alignFactor = dot(_lightDir, -normalData.xyz);

                float4 shadingFactor = float4(1, 1, 1, 1);

                if (alignFactor < 1.0)
                {
                    shadingFactor *= 1 - float4(shadingCol.x, shadingCol.x, shadingCol.x, 1.0f);
                }

                if (alignFactor < 0.5)
                {
                    shadingFactor *= 1 - float4(shadingCol.y, shadingCol.y, shadingCol.y, 1.0f);
                }

                if (alignFactor < 0.2)
                {
                    shadingFactor *= 1 - float4(shadingCol.z, shadingCol.z, shadingCol.z, 1.0f);
                }

                if (shadingFactor.x > 0.96)
                {
                    shadingFactor = float4(1, 1, 1, 1);
                }
                else
                {
                    shadingFactor = float4(_shadowBrightness, _shadowBrightness, _shadowBrightness, 1);
                }

                lighting += _lightColor * shadingFactor;
                
                return colorData * lighting * isNotSky;
            }
            ENDHLSL
        }
    }
}
