using NaughtyAttributes;

namespace GameplayIngredients.Events
{
    [AdvancedHierarchyIcon("Packages/net.peeweek.gameplay-ingredients/Icons/Events/ic-event-start.png")]
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


