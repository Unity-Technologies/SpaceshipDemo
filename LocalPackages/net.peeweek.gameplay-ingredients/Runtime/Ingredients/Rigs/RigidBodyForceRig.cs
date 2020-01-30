using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayIngredients.Rigs
{
    [RequireComponent(typeof(Rigidbody))]
    public class RigidBodyForceRig : MonoBehaviour
    {
        private Rigidbody m_RigidBody;

        public enum EffectorType
        {
            Force,
            ForceAtPosition,
            RelativeForce,
            Torque,
            RelativeTorque,
            Explosion
        }

        [BoxGroup("Configuration"), Tooltip("The kind of force to apply")]
        public EffectorType type;
        [BoxGroup("Configuration"), Tooltip("How to compute the force applied every frame")]
        public ForceMode forceMode;

        [BoxGroup("Force Properties"),Tooltip("The force or torque vector"), ShowIf("isNotExplosion")]
        public Vector3 vector;
        [BoxGroup("Force Properties"), Tooltip("The force or explosion position"), ShowIf("needPosition")]
        public Vector3 position;
        [BoxGroup("Force Properties"), Tooltip("The force scale over time")]
        public AnimationCurve ForceOverTime = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));

        [BoxGroup("Force Properties"), ShowIf("isExplosion")]
        public float explosionForce;
        [BoxGroup("Force Properties"), ShowIf("isExplosion")]
        public float explosionRadius;
        [BoxGroup("Force Properties"), ShowIf("isExplosion")]
        public float upwardsModifier;

        [BoxGroup("Noise")]
        public bool AddNoise = false;
        [BoxGroup("Noise")]
        public Vector3 NoiseScale = Vector3.one; 

        private float m_Time;
        [SerializeField]
        private float m_RandomSeed;

        private void Start()
        {
            m_RandomSeed = Random.Range(-100, +100);
        }

        bool needPosition()
        {
            return type == EffectorType.ForceAtPosition || type == EffectorType.Explosion;
        }

        bool isExplosion()
        {
            return type == EffectorType.Explosion;
        }

        bool isNotExplosion()
        {
            return !isExplosion();
        }

        private void OnEnable()
        {
            m_RigidBody = GetComponent<Rigidbody>();
            m_Time = 0.0f;
        }

        Vector3 GetNoise()
        {
            var noise = new Vector3(
                Mathf.PerlinNoise(m_RandomSeed - 11.42f, m_Time + m_RandomSeed + 24.71f),
                Mathf.PerlinNoise(m_RandomSeed + 17.3f,  m_Time + m_RandomSeed - 1.08f),
                Mathf.PerlinNoise(m_RandomSeed - 0.07f,  m_Time + m_RandomSeed + 43.12f)
                ) * 2.0f-Vector3.one;
            noise.Scale(NoiseScale);
            return noise;
        }

        public void Update()
        {
            if (m_RigidBody == null)
                return;

            m_Time += Time.deltaTime;

            Vector3 force = vector;
            float attenuation = ForceOverTime.Evaluate(m_Time);
            if(AddNoise)
            {
                force += GetNoise();
            }
            force *= attenuation;

            switch (type)
            {
                default:
                case EffectorType.Force:
                    m_RigidBody.AddForce(force, forceMode);
                    break;
                case EffectorType.ForceAtPosition:
                    m_RigidBody.AddForceAtPosition(force, position, forceMode);
                    break;
                case EffectorType.RelativeForce:
                    m_RigidBody.AddRelativeForce(force, forceMode);
                    break;
                case EffectorType.Torque:
                    m_RigidBody.AddTorque(force, forceMode);
                    break;
                case EffectorType.RelativeTorque:
                    m_RigidBody.AddRelativeTorque(force, forceMode);
                    break;
                case EffectorType.Explosion:
                    m_RigidBody.AddExplosionForce(explosionForce * attenuation, position, explosionRadius, upwardsModifier, forceMode);
                    break;
            }
        }

    }
}
