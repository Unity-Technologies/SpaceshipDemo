using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace GameplayIngredients
{
    [ManagerDefaultPrefab("FullScreenFadeManager")]
    public class FullScreenFadeManager : Manager
    {
        public enum FadeMode
        {
            FromBlack = 0,
            ToBlack = 1
        }

        public Image FullScreenFadePlane;

        private Coroutine m_Coroutine;

        public void Fade(float duration, FadeMode mode, Callable[] OnComplete, GameObject instigator = null)
        {
            if (m_Coroutine != null)
            {
                StopCoroutine(m_Coroutine);
                m_Coroutine = null;
            }

            if (duration <= 0.0f)
            {
                var color = FullScreenFadePlane.color; ;
                switch (mode)
                {
                    case FadeMode.ToBlack:
                        color.a = 1.0f;
                        break;
                    case FadeMode.FromBlack:
                        color.a = 0.0f;
                        break;
                    default: throw new NotImplementedException();
                }
                FullScreenFadePlane.gameObject.SetActive(color.a == 1.0f);
                FullScreenFadePlane.color = color;
                Callable.Call(OnComplete, instigator);
            }
            else
            {
                switch (mode)
                {
                    case FadeMode.ToBlack:
                        m_Coroutine = StartCoroutine(FadeCoroutine(duration, 1.0f, 1.0f, OnComplete, instigator));
                        break;
                    case FadeMode.FromBlack:
                        m_Coroutine = StartCoroutine(FadeCoroutine(duration, 0.0f, -1.0f, OnComplete, instigator));
                        break;
                    default: throw new NotImplementedException();
                }
            }

        }

        IEnumerator FadeCoroutine(float duration, float target, float sign, Callable[] OnComplete, GameObject instigator)
        {
            FullScreenFadePlane.gameObject.SetActive(true);
            Color c = FullScreenFadePlane.color;

            while (sign > 0 ? FullScreenFadePlane.color.a <= target : FullScreenFadePlane.color.a >= target)
            {
                c = FullScreenFadePlane.color;
                c.a += sign * Time.unscaledDeltaTime / duration;
                FullScreenFadePlane.color = c;
                yield return new WaitForEndOfFrame();
            }

            Color finalColor = FullScreenFadePlane.color;
            finalColor.a = target;
            FullScreenFadePlane.color = finalColor;
            Callable.Call(OnComplete, instigator);
            FullScreenFadePlane.gameObject.SetActive(target != 0.0f);

            yield return new WaitForEndOfFrame();
            m_Coroutine = null;
        }

    }

}

