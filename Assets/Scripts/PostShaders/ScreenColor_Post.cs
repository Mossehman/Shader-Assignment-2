
using System.Drawing;
using UnityEngine;

[CreateAssetMenu(fileName = "Screen Color Shader", menuName = "Post Processing/Screen Color Shader")]
public class ScreenColor_Post : CustomPostShader
{ 
    public UnityEngine.Color color = UnityEngine.Color.white;   

    public override void SendDataToShader(Material mat)
    {
        mat.SetColor("_Color", color);
    }
}
