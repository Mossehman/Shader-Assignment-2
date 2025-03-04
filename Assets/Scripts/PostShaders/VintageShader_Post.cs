using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Vintage Shader", menuName = "Post Processing/Vintage Shader")]
public class VintageShader_Post : CustomPostShader
{
    [Range(0f, 2f)]
    public float vintageScale;

    public override void SendDataToShader(Material mat)
    {
        mat.SetFloat("_vintageScale", vintageScale);
    }
}
