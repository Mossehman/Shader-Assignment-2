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
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            #pragma vertex vert
            #pragma fragment frag

            // stores our color data
            TEXTURE2D(_gBuffer0);
            SAMPLER(sampler_gBuffer0);

            // stores our normal data
            TEXTURE2D(_gBuffer2);
            SAMPLER(sampler_gBuffer2);

            // stores camera depth
            //TEXTURE2D(_CameraDepthTexture);
            //SAMPLER(sampler_CameraDepthTexture);

            TEXTURE2D(_shadingTexture);
            SAMPLER(sampler_shadingTexture);

            struct LightObj
			{
				float4 lightCol;
				float3 Attenuation;
				float Smoothness;
				float3 Direction;
				float Intensity;
				float3 Position;
				float SpotCutoff;
				float SpotInnerCutoff;
				float SpecularStrength;
				int LightType;
			};
            StructuredBuffer<LightObj> _lightBuffer;
            int _numLights;

            int _doHatchShading;
           
           float2 _shadingTiling = float2(5, 5);
           float _shadowBrightness;
           float _actualLightInfluence;

            float3 _lightDir;
            float4 _lightColor;
            int _colorBanding = 0;
            
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

            float Luminance(float3 c )
            {
            	return dot( c, float3(0.22, 0.707, 0.071) );
            }

            half4 frag (VertToFrag input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                //check if pixel is sky or object (if sky, color pixel black, else color pixel to output color)
                //we need to do this because gBuffer data doesn't clear after the frame
                float depth = SAMPLE_TEXTURE2D_X(_CameraDepthTexture, sampler_CameraDepthTexture, input.uv).r;
                float3 worldPos = ComputeWorldSpacePosition(input.uv, depth, UNITY_MATRIX_I_VP);
                float isNotSky = -step(depth, 0) + 1;

        
                float4 colorData = SAMPLE_TEXTURE2D(_gBuffer0, sampler_gBuffer0, input.uv);

                float4 normalData = SAMPLE_TEXTURE2D(_gBuffer2, sampler_gBuffer2, input.uv);

                float2 tiledUV = input.uv * _shadingTiling;
                float2 normalDistortion = normalData.xy * 0.5f;
                normalDistortion *= (1.0 + normalData.z);
                float2 warpedUV = tiledUV + normalDistortion;


                float4 shadingCol = SAMPLE_TEXTURE2D(_shadingTexture, sampler_shadingTexture, warpedUV);
                float4 lighting = float4(0, 0, 0, 1);

                for (int i = 0; i < _numLights; i++)
                {
                    LightObj light = _lightBuffer[i];
                    float3 finalLightDir;

                    if (light.LightType == 0)
                    {
                        finalLightDir = normalize(light.Direction);
                        light.Attenuation = 1.0;
                    }
                    else 
                    {
                        finalLightDir = normalize(worldPos - light.Position);
                        float distance = length(light.Position - worldPos);
					    light.Attenuation = 1.0 / (light.Attenuation.x + light.Attenuation.y * distance + light.Attenuation.z * distance * distance);

					    if (light.LightType == 2)
					    {
                            float theta = dot(finalLightDir, light.Direction);
                            float angle = cos(radians(light.SpotCutoff));
					    	if (theta <= angle)
					    	{
					    		light.Attenuation = 0.0;
					    	}
					    	else
					    	{
                                float epsilon = cos(radians(light.SpotInnerCutoff)) - angle;
                                float intensity = clamp((theta - angle) / epsilon, 0.0, 1.0);
                                light.Attenuation *= intensity;
					    	}
					    }
                    }

                    float3 viewDir = normalize(light.Position - worldPos);
                    float3 halfVector = normalize(viewDir - finalLightDir);

				    float specular = pow((saturate(dot(normalData.rgb, halfVector))), light.Smoothness * 100);
                    specular = max(floor(max(specular, 0) * _colorBanding) / _colorBanding, 0.2);

				    float3 specularCol = specular * light.lightCol.rgb * light.SpecularStrength;
				    float3 diffuse = colorData.rgb * light.lightCol.rgb * saturate(dot(normalData.rgb, -finalLightDir));
				    float3 result = (diffuse + specularCol) * light.Intensity * light.Attenuation;

                    lighting.rgb += saturate(result);
                }

                //return lighting;

                ///Red -> 0.7, Green -> 0.2, Blue -> 0.1

                float alignFactor = Luminance(lighting.rgb);

                //float alignFactor = dot(_lightDir, -normalData.xyz);
                //
                float4 shadingFactor = float4(1, 1, 1, 1);
                
                if (alignFactor < 0.17)
                {
                    shadingFactor *= 1 - float4(shadingCol.x, shadingCol.x, shadingCol.x, 1.0f);
                }
                
                if (alignFactor < 0.1)
                {
                    shadingFactor *= 1 - float4(shadingCol.y, shadingCol.y, shadingCol.y, 1.0f);
                }
                
                if (alignFactor < 0.05)
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
                
                //lighting += _lightColor * shadingFactor;
                if (_doHatchShading == 0)
                {
                    return colorData * lighting * isNotSky;
                }

                return colorData * shadingFactor * (lerp(1.0f, lighting, _actualLightInfluence)) * isNotSky;

                
            }
            ENDHLSL
        }
    }
}
