    using UnityEngine.Rendering.Universal;
    using UnityEngine.Rendering;
    using UnityEngine;

    public sealed class CustomSRP : ScriptableRenderPass
    {
        private Material material;              // The pass's material for blitting to the screen
        private CustomPostShader postShader;    // The post processing shader our pass was attached to

        private RTHandle CameraColorTarget;

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
            if (renderingData.cameraData.renderer.cameraColorTargetHandle != null)
            {
                ///verifies that the code is indeed setting the texture, however when trying to set the cameraColorTargetHandle, the image is pure black
                cmdBuffer.SetGlobalTexture("_CamTexture", Shader.GetGlobalTexture("_CameraOpaqueTexture"));
            }


            if (postShader.toProfile) // In the event we wish to profile the shader to check performance
            {
                using (new ProfilingScope(cmdBuffer, postShader.GetProfiler()))
                {
                    BlitShader(cmdBuffer);
                Blitter.BlitCameraTexture(cmdBuffer, renderingData.cameraData.renderer.cameraColorTargetHandle, CameraColorTarget, material, 0);
            }
            }
            else
            {
                BlitShader(cmdBuffer);
            Blitter.BlitCameraTexture(cmdBuffer, renderingData.cameraData.renderer.cameraColorTargetHandle, CameraColorTarget, material, 0);
        }

            context.ExecuteCommandBuffer(cmdBuffer);

            // Always make sure to clear and release the buffer to prevent memory leaks!!
            cmdBuffer.Clear();
            CommandBufferPool.Release(cmdBuffer);
        }

        private void BlitShader(CommandBuffer cmdBuffer)
        {
            postShader.SendDataToShader(material);
            
            ///this code seems to incorrectly blit the texture, as the uv coordinates are 0 when displayed across the screen (seems to only sample bottom left pixel)

            //cmdBuffer.Blit(CameraColorTarget, rtTemp, material);
            //cmdBuffer.Blit(rtTemp, CameraColorTarget);
            

        }

        public void SetTarget(RTHandle handle)
        {
            ConfigureInput(ScriptableRenderPassInput.Color); // Ensure we read from the camera color texture    
            CameraColorTarget = handle;
            ConfigureTarget(CameraColorTarget);
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {

            ConfigureTarget(CameraColorTarget);
        }

        public Material GetMaterial() { return material; }
    }