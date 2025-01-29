using UnityEngine;

[CreateAssetMenu(fileName = "Green Tint Shader", menuName = "Post Processing/Green Shader")]
public class ScreenTint_Post : CustomPostShader
{
    public float Intensity = 1.0f;

    public override void SendDataToShader(Material mat)
    {
        mat.SetFloat("_Intensity", Intensity);
    }
}
