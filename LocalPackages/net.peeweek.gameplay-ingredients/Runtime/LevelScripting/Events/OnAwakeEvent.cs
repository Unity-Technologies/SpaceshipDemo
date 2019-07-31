using NaughtyAttributes;
using UnityEngine;

namespace GameplayIngredients.Events
{
    public class OnAwakeEvent : MonoBehaviour
    {
        [ReorderableList]
        public Callable[] onAwake;

        private void Awake()
        {
            Callable.Call(onAwake, gameObject);
        }
    }
}


