using UnityEngine;

namespace GameplayIngredients.Rigs
{
    [AddComponentMenu(ComponentMenu.rigsPath + "LookAt Rig")]
    public class LookAtRig : Rig
    {
        public override int defaultPriority => 0;
        public override UpdateMode defaultUpdateMode => UpdateMode.LateUpdate;

        public Transform LookAtTarget;
        public Space UpVectorSpace = Space.World;
        public Vector3 UpVector = Vector3.up;

        public override void UpdateRig(float deltaTime)
        {
            if (LookAtTarget != null)
            {
                transform.LookAt(LookAtTarget, UpVectorSpace == Space.Self? transform.InverseTransformDirection(UpVector).normalized: UpVector.normalized);
            }
        }
    }
}
