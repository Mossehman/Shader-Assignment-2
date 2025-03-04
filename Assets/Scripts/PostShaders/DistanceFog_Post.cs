using UnityEngine;

[CreateAssetMenu(fileName = "Distance Fog Shader", menuName = "Post Processing/Distance fog Shader")]
public class DistanceFog_Post : CustomPostShader
{
    [Range(0.0f, 1.0f)]
    public float fogStartingDepth;
    [Range(0.0f, 1.0f)]
    public float fogEndDepth;

    public Color fogColor;

    public override void SendDataToShader(Material mat)
    {
        mat.SetFloat("_fogMinDepth", fogStartingDepth);
        mat.SetFloat("_fogMaxDepth", fogEndDepth);

        mat.SetColor("_fogColor", fogColor);
    }
}
