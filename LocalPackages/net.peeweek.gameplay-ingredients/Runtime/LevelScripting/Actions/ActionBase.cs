using UnityEngine;

namespace GameplayIngredients.Actions
{
    public abstract class ActionBase : Callable
    {
        public override sealed string ToString()
        {
            return "Action : " + Name;
        }
    }
}
