//test shader to check if the camera's render texture is being passed into shaders
Shader "Unlit/GreenShader"  
{
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        LOD 100
        ZWrite Off Cull Off
        Pass
        {
            Name "ColorBlitPass"

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            #pragma vertex Vert // Ensure you're using the standard Blit vertex shader
            #pragma fragment frag

            TEXTURE2D_X(_CamTexture);
            SAMPLER(sampler_CamTexture);

            float _Intensity;

            half4 frag(Varyings input) : SV_Target
            {
                // Sample the texture using the screen-space UVs
                float4 color = SAMPLE_TEXTURE2D_X(_CamTexture, sampler_CamTexture, input.texcoord);
                    return color; //* float4(_Intensity, 0, _Intensity, 1);
                //return half4(input.texcoord.x, input.texcoord.y, 0.0, 1.0);
            }
            ENDHLSL
        }
    }
}
