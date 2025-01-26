using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

internal class ColorBlitRendererFeature : ScriptableRendererFeature
{
    public Shader m_Shader;
    public Color m_FogColor;
    public float m_fogDensity, m_fogOffset;
    Material m_Material;

    ColorBlitPass m_RenderPass;

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType == CameraType.Game)
        {
            // Calling ConfigureInput with the ScriptableRenderPassInput.Color argument
            // ensures that the opaque texture is available to the Render Pass.
            m_RenderPass.ConfigureInput(ScriptableRenderPassInput.Color);
            m_RenderPass.ConfigureDepthStoreAction(RenderBufferStoreAction.Store);
            m_RenderPass.ConfigureColorStoreAction(RenderBufferStoreAction.Store);
            m_RenderPass.SetFogColor(m_FogColor);
            m_RenderPass.SetFogDensity(m_fogDensity);
            m_RenderPass.SetFogOffset(m_fogOffset);
            renderer.EnqueuePass(m_RenderPass);
        }
    }

    public override void Create()
    {
        m_Material = CoreUtils.CreateEngineMaterial(m_Shader);
        m_RenderPass = new ColorBlitPass(m_Material);
    }

    protected override void Dispose(bool disposing)
    {
        CoreUtils.Destroy(m_Material);
    }
}

internal class ColorBlitPass : ScriptableRenderPass
{
    ProfilingSampler m_ProfilingSampler = new ProfilingSampler("ColorBlit");
    Material m_Material;
    Color m_fogColor;
    float m_fogDensity, m_fogOffset;

    public ColorBlitPass(Material material)
    {
        m_Material = material;
        renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public void SetFogColor(Color fogColor)
    {
        m_fogColor = fogColor;
    }
    public void SetFogDensity(float density)
    {
        m_fogDensity = density;
    }

    public void SetFogOffset(float offset)
    {
        m_fogOffset = offset;
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        var camera = renderingData.cameraData.camera;
        if (camera.cameraType != CameraType.Game)
            return;

        if (m_Material == null)
            return;

        CommandBuffer cmd = CommandBufferPool.Get();
        using (new ProfilingScope(cmd, m_ProfilingSampler))
        {
            m_Material.SetColor("_FogColor", m_fogColor);
            m_Material.SetFloat("_FogDensity", m_fogDensity);
            m_Material.SetFloat("_FogDensity", m_fogOffset);

            //The RenderingUtils.fullscreenMesh argument specifies that the mesh to draw is a quad.
            //cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, m_Material);
        }
        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();

        CommandBufferPool.Release(cmd);
    }
}