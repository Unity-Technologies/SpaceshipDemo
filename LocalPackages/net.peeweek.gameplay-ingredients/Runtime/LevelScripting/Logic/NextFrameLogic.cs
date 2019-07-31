using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace GameplayIngredients.Logic
{
    public class NextFrameLogic : LogicBase
    {
        [ReorderableList]
        public Callable[] OnNextFrame;
        IEnumerator m_Coroutine;

        public override void Execute(GameObject instigator = null)
        {
            m_Coroutine = RunDelay(instigator);
            StartCoroutine(m_Coroutine);
        }

        IEnumerator RunDelay(GameObject instigator = null)
        {
            yield return new WaitForEndOfFrame();
            Callable.Call(OnNextFrame, instigator);
            m_Coroutine = null;
        }
    }
}

