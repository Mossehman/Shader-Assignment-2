using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Abstract class such that we can use a generic SRP and Render Feature, and only need to call SendDataToShader()
/// </summary>
public abstract class CustomPostShader : ScriptableObject
{
    [Header("Shader config")]
    public Shader shader;
    public ScriptableRenderPassInput renderInput = ScriptableRenderPassInput.Color;


    [Header("Debugging")]
    public bool toProfile = false;

    ProfilingSampler profiler = null;

    private CustomSRP customPass;

    public CustomSRP GetPass() { return customPass; }
    public ProfilingSampler GetProfiler() { return profiler; }

    public void Init()
    {
        //ensures we have a fallback shader for the event that we do not define the shader variable
        if (shader == null) { Debug.LogError("Shader was null!"); return; }
        if (toProfile) { profiler = new ProfilingSampler(this.name); }
    }

    public void CreatePass()
    {
        customPass = new CustomSRP(this);
    }

    /// <summary>
    /// Sends relevant data to the shader, made abstract as different shaders will need different data types
    /// </summary>
    public abstract void SendDataToShader(Material mat);
}
