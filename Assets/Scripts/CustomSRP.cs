using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine;

public sealed class CustomSRP : ScriptableRenderPass
{
    private Material material;              // The pass's material for blitting to the screen
    private CustomPostShader postShader;    // The post processing shader our pass was attached to

    private RTHandle InputHandle;
    private RTHandle OutputHandle;

    public CustomSRP(CustomPostShader post)
    {
        if (post == null) { return; }   // This should never happen
        renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    
        this.postShader = post;
        this.material = new Material(postShader.shader);

        Debug.Log(material.name);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        // Ensure the material is valid
        if (material == null)
        {
            Debug.LogError("Material is null in CustomSRP!");
            return;
        }

        // Get the camera data
        var CameraData = renderingData.cameraData;

        if (CameraData.camera.cameraType != CameraType.Game ||  //we want our post processing to only affect the game view and not the scene editor view
            material == null)                                   //precautionary check in the event our material is not created
        {
            return;
        }

        CommandBuffer cmdBuffer = CommandBufferPool.Get();

        if (postShader.toProfile) // In the event we wish to profile the shader to check performance
        {
            using (new ProfilingScope(cmdBuffer, postShader.GetProfiler()))
            {
                postShader.SendDataToShader(material);
                //Blitter.BlitCameraTexture(cmdBuffer, InputHandle, OutputHandle, 0, true);
                cmdBuffer.Blit(InputHandle, OutputHandle);

                for (int i = 0; i < material.passCount; i++)
                {
                    if (OutputHandle != null)
                    {
                        ///verifies that the code is indeed setting the texture, however when trying to set the cameraColorTargetHandle, the image is pure black
                        cmdBuffer.SetGlobalTexture("_CamTexture", OutputHandle.nameID);
                    }

                    //Blitter.BlitCameraTexture(cmdBuffer, OutputHandle, InputHandle, material, i);
                    cmdBuffer.Blit(OutputHandle, InputHandle, material, i);
                }
            }
        }
        else
        {
            postShader.SendDataToShader(material);
            //Blitter.BlitCameraTexture(cmdBuffer, InputHandle, OutputHandle, 0, true);
            cmdBuffer.Blit(InputHandle, OutputHandle);

            for (int i = 0; i < material.passCount; i++)
            {
                if (OutputHandle != null)
                {
                    ///verifies that the code is indeed setting the texture, however when trying to set the cameraColorTargetHandle, the image is pure black
                    cmdBuffer.SetGlobalTexture("_CamTexture", OutputHandle.nameID);
                }

                //Blitter.BlitCameraTexture(cmdBuffer, OutputHandle, InputHandle, material, i);
                cmdBuffer.Blit(OutputHandle, InputHandle, material, i);
            }
        }

        context.ExecuteCommandBuffer(cmdBuffer);

        // Always make sure to clear and release the buffer to prevent memory leaks!!
        cmdBuffer.Clear();
        CommandBufferPool.Release(cmdBuffer);
    }

    public void SetTarget(RTHandle handle)
    {
        ConfigureInput(ScriptableRenderPassInput.Color); // Ensure we read from the camera color texture    
        //CameraColorTarget = handle;
        //ConfigureTarget(CameraColorTarget);

        InputHandle = handle;
    }

    public Material GetMaterial() { return material; }

    public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
    {
        var desc = cameraTextureDescriptor;
        desc.depthBufferBits = 0;
        desc.msaaSamples = 1;

        RenderingUtils.ReAllocateIfNeeded(ref OutputHandle, desc, FilterMode.Bilinear, TextureWrapMode.Clamp);
        ConfigureTarget(OutputHandle);
    }
}