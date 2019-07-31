using NaughtyAttributes;
using UnityEngine;

namespace GameplayIngredients.Logic
{
    public abstract class LogicBase : Callable
    {
        public override sealed string ToString()
        {
            return "Logic : " + Name;
        }
    }
}
