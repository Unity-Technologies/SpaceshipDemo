using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace GameplayIngredients.Logic
{
    [AddComponentMenu(ComponentMenu.logicPath + "N Times Logic")]
    [Callable("Logic", "Logic/ic-generic-logic.png")]
    public class NTimesLogic : LogicBase
    {
        public Callable[] Calls;
        [Min(1), SerializeField]
        protected int Count = 1;

        int m_RemainingCount;

        void Awake()
        {
            ResetCount();
        }

        public void ResetCount()
        {
            m_RemainingCount = Count;
        }

        public override void Execute(GameObject instigator = null)
        {
            if(m_RemainingCount > 0)
            {
                m_RemainingCount--;
                Callable.Call(Calls, instigator);
            }
        }

        public override string GetDefaultName()
        {
            return $"Call only {Count} Times";
        }
    }
}

