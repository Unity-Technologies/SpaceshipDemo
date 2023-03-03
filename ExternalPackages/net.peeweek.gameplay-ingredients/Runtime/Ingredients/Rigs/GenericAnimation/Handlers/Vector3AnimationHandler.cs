using System;
using UnityEngine;

namespace GameplayIngredients.Rigs
{
    [Serializable]
    public abstract class Vector3AnimationHandler : AnimationHandler<Vector3> { }

    [Serializable, AnimationHandler("Vector3 Continuous", typeof(Vector3))]
    public class Vector3ContinuousAnimationHandler : Vector3AnimationHandler
    {
        [SerializeField]
        Vector3 Rate = Vector3.one;

        Vector3 m_Base;
        float m_Time;

        public override void OnStart(Vector3 defaultValue)
        {
            m_Base = defaultValue;
            m_Time = 0;
        }

        public override Vector3 OnUpdate(float deltaTime)
        {
            m_Time += deltaTime;
            return m_Base + m_Time * Rate;
        }
    }

    [Serializable, AnimationHandler("Vector3 Sine Wave", typeof(Vector3))]
    public class Vector3SineAnimationHandler : Vector3AnimationHandler
    {
        [SerializeField]
        Vector3 frequency = Vector3.one;
        [SerializeField]
        Vector3 amplitude = Vector3.one;

        Vector3 m_Base;
        float m_Time;

        public override void OnStart(Vector3 defaultValue)
        {
            m_Base = defaultValue;
            m_Time = 0;
        }

        public override Vector3 OnUpdate(float deltaTime)
        {
            m_Time += deltaTime;
            return m_Base + new Vector3(
                Mathf.Sin(m_Time * frequency.x * Mathf.PI) * amplitude.x,
                Mathf.Sin(m_Time * frequency.y * Mathf.PI) * amplitude.y,
                Mathf.Sin(m_Time * frequency.z * Mathf.PI) * amplitude.z
                );
        }
    }

    [Serializable, AnimationHandler("Vector3 Noise", typeof(Vector3))]
    public class Vector3NoiseAnimationHandler : Vector3AnimationHandler
    {
        [SerializeField]
        Vector3 frequency = Vector3.one;
        [SerializeField]
        Vector3 amplitude = Vector3.one;
        [SerializeField, Range(0f, 5f)]
        float lacunarity = 0.3f;
        [SerializeField, Range(1,5)]
        int octaves = 1;
        [SerializeField]
        int seed = -1485472;

        Vector3 m_Base;
        float m_Time;

        public override void OnStart(Vector3 defaultValue)
        {
            m_Base = defaultValue;
            m_Time = 0;
        }

        public override Vector3 OnUpdate(float deltaTime)
        {
            m_Time += deltaTime;
            return m_Base + GetRandom();
        }

        Vector3 GetRandom()
        {
            Vector3 v = Vector3.zero;
            for(int i = 0; i < octaves; i++)
            {
                v += new Vector3(
                    (Mathf.PerlinNoise(seed, m_Time * frequency.x * ((i+1) * 1.7153f)) - .5f) * (amplitude.x / (i * lacunarity + 1)),
                    (Mathf.PerlinNoise(seed, m_Time * frequency.y * ((i+1) * 1.7153f)) - .5f) * (amplitude.y / (i * lacunarity + 1)),
                    (Mathf.PerlinNoise(seed, m_Time * frequency.z * ((i+1) * 1.7153f)) - .5f) * (amplitude.z / (i * lacunarity + 1))
                    );
            }
            return v;
        }
    }

    [Serializable, AnimationHandler("Vector3 Curve", typeof(Vector3))]
    public class Vector3CurveAnimationHandler : Vector3AnimationHandler
    {
        static AnimationCurve defaultCurve {
            get
            {
                var c = new AnimationCurve(
                            new Keyframe[] {
                                new Keyframe(0,0, 0, 4f),
                                new Keyframe(0.25f,1),
                                new Keyframe(0.75f,-1),
                                new Keyframe(1,0, 4f, 0)
                            });
                c.preWrapMode = WrapMode.Loop;
                c.postWrapMode = WrapMode.Loop;
                return c;
            }
        }

        [SerializeField]
        AnimationCurve curveX = defaultCurve;
        [SerializeField]
        AnimationCurve curveY = defaultCurve;
        [SerializeField]
        AnimationCurve curveZ = defaultCurve;

        [SerializeField]
        Vector3 amplitude = Vector3.one;

        Vector3 m_Base;
        float m_Time;

        public override void OnStart(Vector3 defaultValue)
        {
            m_Base = defaultValue;
            m_Time = 0;
        }

        public override Vector3 OnUpdate(float deltaTime)
        {
            m_Time += deltaTime;
            return m_Base + new Vector3 (
                curveX.Evaluate(m_Time) * amplitude.x,
                curveY.Evaluate(m_Time) * amplitude.y,
                curveZ.Evaluate(m_Time) * amplitude.z
                );
        }
    }
}
