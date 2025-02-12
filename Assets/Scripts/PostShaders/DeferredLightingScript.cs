using UnityEngine;

[CreateAssetMenu(fileName = "Deferred Lighting Shader", menuName = "Post Processing/Deferred Lighting Shader")]
public class DeferredLightingScript : CustomPostShader
{
    public Vector3 lightDirection;

    public Color lightColor;
    public float colorBanding;

    public Texture2D shadingTexture;
    public Vector2 shadingTiling;

    
    public float shadowBrightness = 0.2f;

    public override void SendDataToShader(Material mat)
    {
        mat.SetTexture("_gBuffer0", Shader.GetGlobalTexture("_GBuffer0"));
        mat.SetTexture("_gBuffer2", Shader.GetGlobalTexture("_GBuffer2"));

        mat.SetColor("_lightColor", lightColor);
        mat.SetVector("_lightDir", lightDirection);
        mat.SetFloat("_colorBanding", colorBanding);

        mat.SetTexture("_shadingTexture", shadingTexture);

        mat.SetVector("_shadingTiling", shadingTiling);
        mat.SetFloat("_shadowBrightness", shadowBrightness);

    }
}
