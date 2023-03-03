using NaughtyAttributes;
using UnityEngine;

namespace GameplayIngredients.Logic
{
    [HelpURL(Help.URL + "events-logic-actions")]
    public abstract class LogicBase : Callable
    {
        public override sealed string ToString()
        {
            return "Logic : " + Name;
        }
    }
}
