using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CustomRenderFeature : ScriptableRendererFeature
{
    public CustomPostShader[] shaders;

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        foreach (var s in shaders)
        {
            if (s == null || s.GetPass() == null) {  continue; }
            renderer.EnqueuePass(s.GetPass());
        }
    }

    public override void Create()
    {
        foreach (var s in shaders)
        {
            s.Init();
            s.CreatePass();
        }
    }

    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        foreach (var s in shaders)
        {
            if (s == null) { continue; }
            s.GetPass().SetTarget(renderer.cameraColorTargetHandle);
        }
    }

    //protected override void Dispose(bool disposing)
    //{
    //    Debug.Log("Destroyed!");
    //    foreach (var s in shaders)
    //    {
    //        if (s == null) { continue; }
    //        CoreUtils.Destroy(s.material);
    //
    //    }
    //}
}
