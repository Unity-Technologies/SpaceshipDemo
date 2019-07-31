using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace GameplayIngredients.Logic
{
    public class NTimesLogic : LogicBase
    {
        [ReorderableList]
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
    }
}

