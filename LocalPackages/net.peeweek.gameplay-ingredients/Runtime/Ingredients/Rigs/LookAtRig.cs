using UnityEngine;

namespace GameplayIngredients.Rigs
{
    public class LookAtRig : MonoBehaviour
    {
        public Transform LookAtTarget;
        public Space UpVectorSpace = Space.World;
        public Vector3 UpVector = Vector3.up;

        void Update()
        {
            if (LookAtTarget != null)
            {
                transform.LookAt(LookAtTarget, UpVectorSpace == Space.Self? transform.InverseTransformDirection(UpVector).normalized: UpVector.normalized);
            }
        }
    }
}
