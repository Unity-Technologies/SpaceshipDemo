using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace GameplayIngredients.Logic
{
    [AddComponentMenu(ComponentMenu.logicPath + "Next Frame Logic")]
    [Callable("Time", "Logic/ic-generic-logic.png")]
    public class NextFrameLogic : LogicBase
    {
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

        public override string GetDefaultName()
        {
            return $"Call Next Frame";
        }
    }
}

