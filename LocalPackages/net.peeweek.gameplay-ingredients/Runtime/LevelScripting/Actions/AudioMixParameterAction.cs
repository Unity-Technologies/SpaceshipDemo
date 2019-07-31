using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

namespace GameplayIngredients.Actions
{
    public class AudioMixParameterAction : ActionBase
    {
        public AudioMixer AudioMixer;

        public string Parameter;
        public float Value;
        public float InterpDuration = 0;

        public UnityEvent OnInterpComplete;

        Coroutine m_Coroutine;
        public override void Execute(GameObject instigator = null)
        {
            if (InterpDuration <= 0.0f)
            {
                AudioMixer.SetFloat(Parameter, Value);
            }
            else
            {
                if (m_Coroutine != null)
                    StopCoroutine(m_Coroutine);

                m_Coroutine = StartCoroutine(InterpParameterCoroutine(AudioMixer, InterpDuration, Parameter, Value, OnInterpComplete));
            }
        }

        IEnumerator InterpParameterCoroutine(AudioMixer mixer, float duration, string parameter, float targetvalue, UnityEvent onInterpComplete)
        {
            float initial = 0.0f;
            if (mixer.GetFloat(parameter, out initial))
            {
                float t = 0.0f;
                t += Time.deltaTime / duration;
                while (t < 1.0f)
                {
                    mixer.SetFloat(parameter, Mathf.Lerp(initial, targetvalue, t));
                    yield return new WaitForEndOfFrame();
                }
                mixer.SetFloat(parameter, targetvalue);
                yield return new WaitForEndOfFrame();
                onInterpComplete.Invoke();
            }
            else
            {
                throw new InvalidOperationException("Parameter " + parameter + " does not exist on target AudioMixer : " + mixer.name);
            }
        }
    }
}
