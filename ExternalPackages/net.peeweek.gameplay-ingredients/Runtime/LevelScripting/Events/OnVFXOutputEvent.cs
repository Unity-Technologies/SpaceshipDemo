using UnityEngine;
#if PACKAGE_VFXGRAPH
using UnityEngine.VFX;
#endif

namespace GameplayIngredients.Events
{
#if !PACKAGE_VFXGRAPH
    [WarnDisabledModule("Visual Effect Graph")]
#else    
    [AddComponentMenu(ComponentMenu.vfxPath + "On VFX Output Event")]
    [RequireComponent(typeof(VisualEffect))]
#endif
    public class OnVFXOutputEvent : EventBase
    {
        public string vfxEventName { get => m_VFXEventName; set { m_VFXEventName = value; CacheEventName(); } }
        [SerializeField]
        string m_VFXEventName = "On Received Event";
        int m_VFXEventID;

        [SerializeField]
        protected Callable[] onEventReceived;

        private void OnEnable()
        {
            CacheEventName();
#if PACKAGE_VFXGRAPH
            GetComponent<VisualEffect>().outputEventReceived += OnVFXOutputEvent_Received;
#else
            Debug.LogWarning("OnVFXOutputEvent could not attach to VFX as VFX Graph package is not installed, if you're running HDRP or URP, please install it using package manager.");
#endif
        }

        private void OnDisable()
        {
#if PACKAGE_VFXGRAPH
            GetComponent<VisualEffect>().outputEventReceived -= OnVFXOutputEvent_Received;
#endif            
        }

        private void OnValidate()
        {
            CacheEventName();
        }

        void CacheEventName()
        {
            m_VFXEventID = Shader.PropertyToID(m_VFXEventName);
        }

#if PACKAGE_VFXGRAPH
        void OnVFXOutputEvent_Received(VFXOutputEventArgs args)
        {
            if(args.nameId == m_VFXEventID)
            {
                Callable.Call(onEventReceived, gameObject);
            }
        }
#endif
    }
}

