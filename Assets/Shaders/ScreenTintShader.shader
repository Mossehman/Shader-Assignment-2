Shader "Unlit/ScreenTintShader"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZWrite Off Cull Off
        Pass
        {
            Name "ColorBlitPass"

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            // The Blit.hlsl file provides the vertex shader (Vert),
            // input structure (Attributes) and output strucutre (Varyings)
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            #pragma vertex Vert
            #pragma fragment frag



            float4 _Color;
            TEXTURE2D_X(_CamTexture);
            TEXTURE2D_X(_CameraDepthTexture);
            TEXTURE2D_X(_gBuffer1);

            SAMPLER(sampler_CamTexture);
            SAMPLER(sampler_CameraDepthTexture);
            SAMPLER(sampler_gBuffer1);

            half4 frag (Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                // Sample the GBuffer texture
                float depth = SAMPLE_TEXTURE2D_X(_CameraDepthTexture, sampler_CameraDepthTexture, input.texcoord);
                float isNotSky = -step(depth, 0) + 1;
                float4 gBuffer1 = SAMPLE_TEXTURE2D_X(_gBuffer1, sampler_gBuffer1, input.texcoord) * isNotSky;

                

                // Decode the GBuffer data (e.g., extract albedo)
                float3 albedo = gBuffer1.rgb; // Albedo is stored in RGB channels
                float occlusion = gBuffer1.a; // Occlusion is stored in the alpha channel

                // Return the albedo color
                return float4(gBuffer1.r, gBuffer1.g, gBuffer1.b, gBuffer1.a);
            }
            ENDHLSL
        }
    }
}
