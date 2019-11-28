using NaughtyAttributes;
using UnityEngine;

namespace GameplayIngredients.Rigs
{
    public class ReachPositionRig : MonoBehaviour
    {
        public Transform target => m_Target;

        [Header("Target")]
        [SerializeField]
        protected Transform m_Target;
        [Header("Motion")]
        public float Dampen = 1.0f;
        public float MaximumVelocity = 1.0f;

        [Header("On Reach Position")]
        [ReorderableList]
        public Callable[] OnReachPosition;
        public float ReachSnapDistance = 0.01f;

        bool m_PositionReached = false;

        void LateUpdate()
        {
            if(m_Target != null)
            {
                var transform = gameObject.transform;
                if(Vector3.Distance(transform.position , m_Target.position) < ReachSnapDistance)
                {
                    transform.position = m_Target.position;
                    if(!m_PositionReached)
                    {
                        Callable.Call(OnReachPosition, this.gameObject);
                        m_PositionReached = true;
                    }
                }
                else
                {
                    var delta = m_Target.position - transform.position;
                    var speed = Time.deltaTime * Mathf.Min((Dampen * delta.magnitude), MaximumVelocity);
                    gameObject.transform.position += delta.normalized * speed;
                    m_PositionReached = false;
                }
            }
        }

        public void SetTarget(Transform target)
        {
            m_Target = target;
        }

    }
}
