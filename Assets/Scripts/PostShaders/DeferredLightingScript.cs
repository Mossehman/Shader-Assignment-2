using UnityEngine;

[CreateAssetMenu(fileName = "Deferred Lighting Shader", menuName = "Post Processing/Deferred Lighting Shader")]
public class DeferredLightingScript : CustomPostShader
{
    public bool doHatchShading = true;

    public int colorBanding;

    [Range(0f, 1f)]
    public float actualLightInfluence;

    public Texture2D shadingTexture;
    public Vector2 shadingTiling;

    
    public float shadowBrightness = 0.2f;

    public override void SendDataToShader(Material mat)
    {
        mat.SetTexture("_gBuffer0", Shader.GetGlobalTexture("_GBuffer0"));
        mat.SetTexture("_gBuffer2", Shader.GetGlobalTexture("_GBuffer2"));

        mat.SetInt("_colorBanding", colorBanding);
        mat.SetInt("_colorBanding", colorBanding);

        if (doHatchShading )
        {
            mat.SetInt("_doHatchShading", 1);
        }
        else
        {
            mat.SetInt("_doHatchShading", 0);
        }


        mat.SetTexture("_shadingTexture", shadingTexture);

        mat.SetVector("_shadingTiling", shadingTiling);
        mat.SetFloat("_shadowBrightness", shadowBrightness);
        mat.SetFloat("_actualLightInfluence", actualLightInfluence);

        if (LightManagerScript.instance != null)
        {
            ComputeBuffer lightBuffer = LightManagerScript.instance.GetLightBufferData();
            if (lightBuffer != null)
            {
                mat.SetInt("_numLights", LightManagerScript.instance.GetNumLights());
                mat.SetBuffer("_lightBuffer", lightBuffer);
            }

        }

    }
}
