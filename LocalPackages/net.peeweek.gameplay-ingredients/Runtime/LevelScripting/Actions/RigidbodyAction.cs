using NaughtyAttributes;
using UnityEngine;

namespace GameplayIngredients.Actions
{
    public enum RigidbodyActionType { Force, Torque, ExplosionForce, Sleep };
    public enum ActionSpace { Local, World };

    [ExecuteAlways]
    public class RigidbodyAction : ActionBase
    {
        public bool ApplyOnInstigator = false;
        [HideIf("ApplyOnInstigator")]
        public Rigidbody m_Rigidbody;
        [OnValueChanged("OnParameterTypeChanged")]
        public RigidbodyActionType actionType;

        bool force = true;
        bool explosion;

        [ShowIf("force")]
        public ActionSpace actionSpace;
        [ShowIf("force")]
        public ForceMode forceMode;
        [ShowIf("force")]
        public Vector3 direction;
        [ShowIf("explosion")]
        public float explosionForce;
        [ShowIf("explosion")]
        public Vector3 explositonPosition;
        [ShowIf("explosion")]
        public float explosionRadius;

        public override void Execute(GameObject instigator = null)
        {
            Rigidbody target = m_Rigidbody;

            if (ApplyOnInstigator)
                target = instigator.GetComponent<Rigidbody>();

            if (target == null)
            {
                Debug.LogWarning("Could not apply RigidbodyAction to null Rigidbody");
                return;
            }

            switch (actionType)
            {
                case RigidbodyActionType.Force:
                    if(actionSpace == ActionSpace.World)
                    {
                        target.AddForce(direction, forceMode);
                    }
                    if(actionSpace == ActionSpace.Local)
                    {
                        target.AddRelativeForce(direction, forceMode);
                    }
                    break;
                case RigidbodyActionType.Torque:
                    if (actionSpace == ActionSpace.World)
                    {
                        target.AddTorque(direction, forceMode);
                    }
                    if (actionSpace == ActionSpace.Local)
                    {
                        target.AddRelativeTorque(direction, forceMode);
                    }
                    break;
                case RigidbodyActionType.ExplosionForce:
                    target.AddExplosionForce(explosionForce, explositonPosition, explosionRadius, 0, forceMode);
                    break;
                case RigidbodyActionType.Sleep:
                    target.Sleep();
                    break;
            }
        }

        private void OnParameterTypeChanged()
        {
            force = (actionType == RigidbodyActionType.Force || actionType == RigidbodyActionType.Torque);
            explosion = (actionType == RigidbodyActionType.ExplosionForce);
        }
    }
}
