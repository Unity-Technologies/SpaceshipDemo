using NaughtyAttributes;
using UnityEngine;

namespace GameplayIngredients.Events
{
    [RequireComponent(typeof(Joint))]
    public class OnJointBreakEvent : EventBase
    {
        [ReorderableList]
        public Callable[] onJointBreak;

        private void OnJointBreak(float breakForce)
        {
            Callable.Call(onJointBreak, gameObject);
        }
    }
}