using NaughtyAttributes;

namespace GameplayIngredients.Events
{
    public class OnStartEvent : EventBase
    {
        [ReorderableList]
        public Callable[] OnStart;

        private void Start()
        {
            Callable.Call(OnStart, gameObject); 
        }
    }
}


