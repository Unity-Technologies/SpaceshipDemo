using NaughtyAttributes;
using UnityEngine;

namespace GameplayIngredients.Events
{
    [AddComponentMenu(ComponentMenu.eventsPath + "On Joint Break Event")]
    [RequireComponent(typeof(Joint))]
    public class OnJointBreakEvent : EventBase
    {
        public Callable[] onJointBreak;

        private void OnJointBreak(float breakForce)
        {
            Callable.Call(onJointBreak, gameObject);
        }
    }
}