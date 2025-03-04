Shader "Unlit/VintageShader"
{
        SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZWrite Off Cull Off
        Pass
        {
            Name "DistanceFog"

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            // The Blit.hlsl file provides the vertex shader (Vert),
            // input structure (Attributes) and output strucutre (Varyings)

            #pragma vertex vert
            #pragma fragment frag


            
            TEXTURE2D(_CamTexture);
            SAMPLER(sampler_CamTexture);

            float _vintageScale;

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

            half4 frag (VertToFrag input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                
                float4 camColor = SAMPLE_TEXTURE2D(_CamTexture, sampler_CamTexture, input.uv);

                //if (_vintageStart >= _vintageEnd)
                //{
                //    return camColor;
                //}

                float distanceFromScreenCenter = distance(float2(0.5, 0.5), input.uv);
                float vintageScaling = _vintageScale - 2 * (distanceFromScreenCenter);


                return camColor * vintageScaling;
            }
            ENDHLSL
        }
    }
}
