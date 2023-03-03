using System;
using UnityEngine;

namespace GameplayIngredients.Rigs
{
    [Serializable]
    public abstract class ColorAnimationHandler : AnimationHandler<Color> { }

    [Serializable, AnimationHandler("Color Gradient", typeof(Color))]
    public class ColorGradientAnimationHandler : ColorAnimationHandler
    {
        static Gradient defaultGradient {
            get
            {
                var c = new Gradient();
                c.SetKeys(new GradientColorKey[]
                {
                    new GradientColorKey(Color.red, .0f),
                    new GradientColorKey(Color.yellow, .16666f),
                    new GradientColorKey(Color.green, .33333f),
                    new GradientColorKey(Color.cyan, .5f),
                    new GradientColorKey(Color.blue, .6666666f),
                    new GradientColorKey(Color.magenta, .833333f),
                    new GradientColorKey(Color.red, .1f)
                },
                new GradientAlphaKey[] { new GradientAlphaKey(1, 0), new GradientAlphaKey(1, 1) });
                return c;
            }
        }

        [SerializeField]
        Gradient gradient = defaultGradient;
        [SerializeField]
        float blendSourceColor = 0f;

        Color m_Base;
        float m_Time;

        public override void OnStart(Color defaultValue)
        {
            m_Base = defaultValue;
            m_Time = 0;
        }

        public override Color OnUpdate(float deltaTime)
        {
            m_Time += deltaTime;
            return Color.Lerp(gradient.Evaluate(m_Time), m_Base, blendSourceColor);
        }
    }
}
