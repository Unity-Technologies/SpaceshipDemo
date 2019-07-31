using UnityEngine;

namespace GameplayIngredients.Rigs
{ 
    public class RotationRig : MonoBehaviour
    {
        public Space Space = Space.World;
        public Vector3 RotationAxis = Vector3.up;
        public float RotationSpeed = 30.0f;

        void Update()
        {
            transform.Rotate(RotationAxis.normalized, RotationSpeed * Time.deltaTime, Space);
        }
    }
}
