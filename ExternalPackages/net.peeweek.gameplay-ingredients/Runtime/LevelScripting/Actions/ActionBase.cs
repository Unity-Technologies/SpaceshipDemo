using UnityEngine;

namespace GameplayIngredients.Actions
{
    [HelpURL(Help.URL + "events-logic-actions")]
    public abstract class ActionBase : Callable
    {
        public override sealed string ToString()
        {
            return "Action : " + Name;
        }
    }
}
