using NaughtyAttributes;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace GameplayIngredients.Logic
{
    public abstract class ConditionalLogic : LogicBase
    {
        [ReorderableList]
        public Callable[] OnConditionValid;

        [ReorderableList]
        public Callable[] OnConditionInvalid;

        public override void Execute(GameObject instigator = null)
        {
            if (GetCondition())
                Callable.Call(OnConditionValid, instigator);
            else
                Callable.Call(OnConditionInvalid, instigator);
        }

        public abstract bool GetCondition();

        protected static bool Compare<T>(T A, T B, Comparison c) where T : IComparable
        {
            int comp = A.CompareTo(B);
            switch (c)
            {
                case Comparison.Equal: return comp == 0;
                case Comparison.NotEqual: return comp != 0;
                case Comparison.Greater: return comp > 0;
                case Comparison.GreaterOrEqual: return comp >= 0;
                case Comparison.Less: return comp < 0;
                case Comparison.LessOrEqual: return comp <= 0;
            }
            return false;
        }

        public enum Comparison
        {
            Equal,
            NotEqual,
            Greater,
            GreaterOrEqual,
            Less,
            LessOrEqual
        }

    }

}
