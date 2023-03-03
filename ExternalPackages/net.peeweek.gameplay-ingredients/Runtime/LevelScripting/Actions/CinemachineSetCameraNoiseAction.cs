using UnityEngine;
using Cinemachine;
using NaughtyAttributes;

namespace GameplayIngredients.Actions
{
    [Callable("Cinemachine", "Misc/ic-cinemachine.png")]
    [AddComponentMenu(ComponentMenu.cinemachinePath + "Cinemachine Set Camera Noise Action")]

    public class CinemachineSetCameraNoiseAction : ActionBase
    {
        [SerializeField]
        bool useLiveCamera;
        [SerializeField, HideIf("useLiveCamera")]
        CinemachineVirtualCamera targetCamera;

        [SerializeField]
        NoiseSettings settings;

        public override void Execute(GameObject instigator = null)
        {
            CinemachineVirtualCamera cam = useLiveCamera ?
                Manager.Get<VirtualCameraManager>().GetComponent<CinemachineBrain>().ActiveVirtualCamera as CinemachineVirtualCamera
                : targetCamera;

            if(cam == null)
            {
                Debug.Log("CinemachineSetCameraNoiseAction : Cannot find a suitable CinemachineVirtualCamera to set Noise to");
                return;
            }

            var noise = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            if(noise == null && settings != null)
                noise = cam.AddCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            noise.m_NoiseProfile = settings;
        }

        public override string GetDefaultName() => $"CM Set Noise ({settings.name}) for {(useLiveCamera? "Live Camera" : targetCamera?.gameObject.name)}";

    }

}

