using UnityEngine;

[CreateAssetMenu(fileName = "Screen Color Shader", menuName = "Post Processing/Screen Color Shader")]
public class ScreenColor_Post : CustomPostShader
{ 
    public Color color = Color.white;
    public Texture2D tex;

    public override void SendDataToShader(Material mat)
    {
        mat.SetTexture("_gBuffer1", Shader.GetGlobalTexture("_GBuffer0"));
        mat.SetColor("_Color", color);
        //mat.SetTexture("_gBuffer1", tex);
    }
}
