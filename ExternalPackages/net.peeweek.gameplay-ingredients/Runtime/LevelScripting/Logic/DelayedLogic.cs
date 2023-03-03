using NaughtyAttributes;
using System.Collections;
using UnityEngine;
namespace GameplayIngredients.Logic
{
    [AddComponentMenu(ComponentMenu.logicPath + "Delayed Logic")]
    [Callable("Time", "Logic/ic-generic-logic.png")]
    public class DelayedLogic : LogicBase
    {
        public enum DelayMode { Constant, Random };
        public DelayMode delayMode;
        [ShowIf("DelayIsConstant")]
        public float Delay = 1.0f;
        [ShowIf("DelayIsRandom")]
        public Vector2 DelayRange = Vector2.one;
        public Callable[] OnDelayComplete;
        public Callable[] OnCanceled;
        IEnumerator m_Coroutine;
        private void OnValidate()
        {
            if (DelayIsConstant())
                DelayRange = new Vector2(Delay, Delay + 1);
        }
        private bool DelayIsRandom()
        {
            bool random = delayMode == DelayMode.Random ? true : false;
            return random;
        }
        private bool DelayIsConstant() { return !DelayIsRandom(); }
        public void Cancel(GameObject instigator = null)
        {
            if (m_Coroutine != null)
            {
                StopCoroutine(m_Coroutine);
                Callable.Call(OnCanceled, instigator);
                m_Coroutine = null;
            }
        }

        public override void Execute(GameObject instigator = null)
        {
            float newDelay;
            if (m_Coroutine != null) Cancel();
            if (delayMode == DelayMode.Random)
                newDelay = Random.Range(DelayRange.x, DelayRange.y);
            else
                newDelay = Delay;
            m_Coroutine = RunDelay(newDelay, instigator);
            StartCoroutine(m_Coroutine);
        }

        IEnumerator RunDelay(float Seconds, GameObject instigator = null)
        {
            yield return new WaitForSeconds(Seconds);
            Callable.Call(OnDelayComplete, instigator);
            m_Coroutine = null;
        }

        public override string GetDefaultName()
        {
            return $"{delayMode} Delay ({(delayMode == DelayMode.Constant? Delay+"s.":DelayRange.x+"s.~"+DelayRange.y+"s.")})";
        }
    }
}