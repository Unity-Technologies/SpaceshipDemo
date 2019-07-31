using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

namespace GameplayIngredients.Actions
{
    public class AudioMixParameterCurveAction : ActionBase
    {
        public AudioMixer AudioMixer;

        public string Parameter;
        public AnimationCurve Curve;
        public float InterpDuration = 2.0f;

        public UnityEvent OnInterpComplete;

        Coroutine m_Coroutine;
        public override void Execute(GameObject instigator = null)
        {

            if (m_Coroutine != null)
                StopCoroutine(m_Coroutine);
            m_Coroutine = StartCoroutine(InterpParameterCoroutine(AudioMixer, InterpDuration, Parameter, Curve, OnInterpComplete));
        }

        IEnumerator InterpParameterCoroutine(AudioMixer mixer, float duration, string parameter, AnimationCurve curve, UnityEvent onInterpComplete)
        {
            float t = 0.0f;

            while (t < 1.0f)
            {
                mixer.SetFloat(parameter, curve.Evaluate(t));
                yield return new WaitForEndOfFrame();
                t += Time.deltaTime / duration;
            }
            mixer.SetFloat(parameter, curve.Evaluate(1.0f));
            yield return new WaitForEndOfFrame();
            onInterpComplete.Invoke();

        }
    }

}
