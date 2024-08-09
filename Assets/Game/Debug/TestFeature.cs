using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Game
{
    public class TestFeature : FullScreenPassRendererFeature
    {
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            TestVolume volume = VolumeManager.instance.stack.GetComponent<TestVolume>();
            passMaterial.SetInt("_Value", volume.parameter.value);
            base.AddRenderPasses(renderer, ref renderingData);
        }
    }
}