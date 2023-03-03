using UnityEngine;
#if PACKAGE_VFXGRAPH
using UnityEngine.VFX;
#endif

namespace GameplayIngredients.Actions
{
#if !PACKAGE_VFXGRAPH
    [WarnDisabledModule("Visual Effect Graph")]
#endif
    [AddComponentMenu(ComponentMenu.vfxPath + "VFX Send Event Action")]
    [Callable("Visual Effects", "Misc/ic-vfx.png")]
    public class VFXSendEventAction : ActionBase
    {
#if PACKAGE_VFXGRAPH
        [NonNullCheck]
        public VisualEffect visualEffect;
#endif

        public string eventName = "Event";

        public override void Execute(GameObject instigator = null)
        {
#if PACKAGE_VFXGRAPH
            int id = Shader.PropertyToID(eventName);
            visualEffect?.SendEvent(eventName);
#else
            Debug.LogWarning("VFXSendEventAction could not attach to VFX as VFX Graph package is not installed, if you're running HDRP or URP, please install it using package manager.");
#endif
        }
#if PACKAGE_VFXGRAPH

        public override string GetDefaultName()
        {
            return $"Send VFX Event '{eventName}' to {visualEffect?.gameObject.name}";
        }
#endif
    }

}
