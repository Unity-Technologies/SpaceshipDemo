using UnityEngine;

namespace GameplayIngredients.Rigs
{ 
    [AddComponentMenu(ComponentMenu.rigsPath + "Rotation Rig")]
    public class RotationRig : Rig
    {
        public override int defaultPriority => 0;
        public override UpdateMode defaultUpdateMode => UpdateMode.Update;
        public override bool canChangeUpdateMode => false;

        public Space Space = Space.World;
        public Vector3 RotationAxis = Vector3.up;
        public float RotationSpeed = 30.0f;

        public override void UpdateRig(float deltaTime)
        {
            transform.Rotate(RotationAxis.normalized, RotationSpeed * deltaTime, Space);
        }
    }
}
