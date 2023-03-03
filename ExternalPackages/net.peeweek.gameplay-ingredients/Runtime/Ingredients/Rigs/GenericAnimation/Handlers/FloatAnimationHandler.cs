using System;
using UnityEngine;

namespace GameplayIngredients.Rigs
{
    [Serializable]
    public abstract class FloatAnimationHandler : AnimationHandler<float> { }

    [Serializable, AnimationHandler("Float Continuous", typeof(float))]
    public class FloatContinuousAnimationHandler : FloatAnimationHandler
    {
        [SerializeField]
        float Rate = 1.0f;

        float m_Base;
        float m_Time;

        public override void OnStart(float defaultValue)
        {
            m_Base = defaultValue;
            m_Time = 0;
        }

        public override float OnUpdate(float deltaTime)
        {
            m_Time += deltaTime;
            return m_Base + m_Time * Rate;
        }
    }

    [Serializable, AnimationHandler("Float Sine Wave", typeof(float))]
    public class FloatSineAnimationHandler : FloatAnimationHandler
    {
        [SerializeField]
        float frequency = 1.0f;
        [SerializeField]
        public float amplitude = 1.0f;

        float m_Base;
        float m_Time;

        public override void OnStart(float defaultValue)
        {
            m_Base = defaultValue;
            m_Time = 0;
        }

        public override float OnUpdate(float deltaTime)
        {
            m_Time += deltaTime;
            return m_Base + Mathf.Sin(m_Time * frequency * Mathf.PI) * amplitude;
        }
    }

    [Serializable, AnimationHandler("Float Noise", typeof(float))]
    public class FloatNoiseAnimationHandler : FloatAnimationHandler
    {
        [SerializeField]
        float frequency = 1.0f;
        [SerializeField]
        float amplitude = 1.0f;
        [SerializeField, Range(0f, 5f)]
        float lacunarity = 0.3f;
        [SerializeField, Range(1,5)]
        int octaves = 1;
        [SerializeField]
        int seed = -1485472;

        float m_Base;
        float m_Time;

        public override void OnStart(float defaultValue)
        {
            m_Base = defaultValue;
            m_Time = 0;
        }

        public override float OnUpdate(float deltaTime)
        {
            m_Time += deltaTime;
            return m_Base + GetRandom();
        }

        float GetRandom()
        {
            float v = 0;
            for(int i = 0; i < octaves; i++)
            {
                v += (Mathf.PerlinNoise(seed, m_Time * frequency * ((i+1) * 1.7153f)) - .5f) * (amplitude / (i * lacunarity + 1));
            }
            return v;
        }
    }

    [Serializable, AnimationHandler("Float Curve", typeof(float))]
    public class FloatCurveAnimationHandler : FloatAnimationHandler
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
        AnimationCurve curve = defaultCurve;
        [SerializeField]
        float amplitude = 1.0f;

        float m_Base;
        float m_Time;

        public override void OnStart(float defaultValue)
        {
            m_Base = defaultValue;
            m_Time = 0;
        }

        public override float OnUpdate(float deltaTime)
        {
            m_Time += deltaTime;
            return m_Base + curve.Evaluate(m_Time) * amplitude;
        }
    }
}
