using UnityEngine;
using UnityEngine.Events;

namespace GameplayIngredients.Actions
{
    public class UnityEventAction : ActionBase
    {
        public UnityEvent OnExecute;

        public override void Execute(GameObject instigator = null)
        {
            OnExecute.Invoke();
        }
    }
}
