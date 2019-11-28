using NaughtyAttributes;

namespace GameplayIngredients.Events
{
    public class OnUpdateEvent : EventBase
    {
        [ReorderableList, ShowIf("AllowUpdateCalls"), InfoBox("Update Calls are disabled on this project. Check your GameplayIngredientsSettings asset if you want to allow them.", InfoBoxType.Warning, "ForbidUpdateCalls")]
        public Callable[] OnUpdate;

        private void OnEnable()
        {
            
        }

        private void OnDisable()
        {
            
        }

        private void Update()
        {
            Callable.Call(OnUpdate, gameObject); 
        }
    }
}


