using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
public class BackupFrameBufferPass : CustomPass
{
    [Header("Output")]
    public RenderTexture outputRenderTexture;
    protected override bool executeInSceneView => true;

    protected override void Execute(CustomPassContext ctx)
    {
        if(outputRenderTexture != null)
        {
            var scale = RTHandles.rtHandleProperties.rtHandleScale;
            ctx.cmd.Blit(ctx.cameraColorBuffer, outputRenderTexture, new Vector2(scale.x, scale.y), Vector2.zero, 0, 0);
        }
    }

    protected override void Cleanup()
    {
        base.Cleanup();
    }
}
