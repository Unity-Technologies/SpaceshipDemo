using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace GameplayIngredients.Logic
{
    public class DelayedLogic : LogicBase
    {
        public float Delay = 1.0f;

        [ReorderableList]
        public Callable[] OnDelayComplete;

        [ReorderableList]
        public Callable[] OnCanceled;

        IEnumerator m_Coroutine;

        public void Cancel(GameObject instigator = null)
        {
            if(m_Coroutine != null)
            {
                StopCoroutine(m_Coroutine);
                Callable.Call(OnCanceled, instigator);
                m_Coroutine = null;
            }
        }

        public override void Execute(GameObject instigator = null)
        {
            if (m_Coroutine != null) Cancel();

            m_Coroutine = RunDelay(Delay, instigator);
            StartCoroutine(m_Coroutine);
        }

        IEnumerator RunDelay(float Seconds, GameObject instigator = null)
        {
            yield return new WaitForSeconds(Seconds);
            Callable.Call(OnDelayComplete, instigator);
            m_Coroutine = null;
        }
    }
}

