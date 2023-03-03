using UnityEngine;
using Cinemachine;
using NaughtyAttributes;

namespace GameplayIngredients.Actions
{
    [Callable("Cinemachine", "Misc/ic-cinemachine.png")]
    [AddComponentMenu(ComponentMenu.cinemachinePath + "Cinemachine Camera Shake Action")]
    public class CinemachineCameraShakeAction : ActionBase
    {
        public enum ImpulseLocation
        {
            Self,
            ImpulseSource,
            OtherTransform,
            InstigatorPosition
        }

        [InfoBox("Remember to add Cinemachine Impulse listeners extensions on Virtual Cameras you want to recieve these camera shakes.")]
        [NonNullCheck]
        public CinemachineImpulseSource impulseSource;
        [Header("Position")]
        [Tooltip("Which transform to use for the source of the camera shake")]
        public ImpulseLocation impulseLocation = ImpulseLocation.Self;
        [ShowIf("useRemoteTransform")]
        [Tooltip("The alternate transform to use to generate impulses")]
        public Transform otherTransform;
        bool useRemoteTransform => impulseLocation == ImpulseLocation.OtherTransform;

        [Header("Impulse")]
        [Tooltip("Whether the Impulse Vector will be computed in local space (or world space)")]
        public bool localSpace = true;
        [Tooltip("The Impulse Vector to Use")]
        public Vector3 baseImpulse = Vector3.up;
        [Tooltip("Whether to apply randomness as well")]
        public bool randomImpulse = false;
        [Tooltip("The Random Variation scale of the Impulse")]
        [ShowIf("randomImpulse")]
        public Vector3 variation = Vector3.one;
        [Tooltip("Whether to normalize the Base+Random Impulse")]
        [ShowIf("randomImpulse")]
        public bool normalize = false;
        [Tooltip("A random rescale of the impulse vector, after normalization")]
        [ShowIf(EConditionOperator.And, "randomImpulse", "normalize")]
        public Vector2 postNormalizeRemap = Vector2.one;

        public override void Execute(GameObject instigator = null)
        {
            if(impulseSource == null)
            {
                Debug.LogWarning($"CinemachineCameraShakeAction : No Impulse source was provided");
                return;
            }

            Vector3 impulse = baseImpulse;
            if(randomImpulse)
            {
                impulse += new Vector3(Random.Range(-variation.x / 2, variation.x / 2), Random.Range(-variation.y / 2, variation.y / 2), Random.Range(-variation.z / 2, variation.z / 2));
                if(normalize)
                {
                    impulse.Normalize();
                    impulse *= Random.Range(postNormalizeRemap.x, postNormalizeRemap.y);
                }
            }

            switch (impulseLocation)
            {
                default:
                case ImpulseLocation.Self:
                    if (localSpace)
                        impulse = transform.localToWorldMatrix.MultiplyVector(impulse);

                    impulseSource.GenerateImpulseAt(transform.position, impulse);
                    break;
                case ImpulseLocation.ImpulseSource:
                    if (localSpace)
                        impulse = impulseSource.transform.localToWorldMatrix.MultiplyVector(impulse);

                    impulseSource.GenerateImpulseAt(impulseSource.transform.position, impulse);

                    break;
                case ImpulseLocation.OtherTransform:
                    if(otherTransform != null)
                    {
                        if (localSpace)
                            impulse = otherTransform.localToWorldMatrix.MultiplyVector(impulse);

                        impulseSource.GenerateImpulseAt(otherTransform.position, impulse);
                    }
                    else
                    {
                        Debug.LogWarning("CinemachineCameraShakeAction : No RemoteTransform found for setting position, using self transform instead");
                        if (localSpace)
                            impulse = transform.localToWorldMatrix.MultiplyVector(impulse);
                        impulseSource.GenerateImpulse(impulse);
                    }
                    break;
                case ImpulseLocation.InstigatorPosition:
                    if(instigator != null)
                    {
                        if (localSpace)
                            impulse = instigator.transform.localToWorldMatrix.MultiplyVector(impulse);

                        impulseSource.GenerateImpulseAt(instigator.transform.position, impulse);
                    }
                    else
                    {
                        Debug.LogWarning("CinemachineCameraShakeAction : No Instigator found for setting position, using self transform instead");
                        if (localSpace)
                            impulse = transform.localToWorldMatrix.MultiplyVector(impulse);
                        impulseSource.GenerateImpulse(impulse);
                    }
                    break;
            } 
        }

        public override string GetDefaultName() => $"CM CameraShake at {impulseLocation}";

    }

}

