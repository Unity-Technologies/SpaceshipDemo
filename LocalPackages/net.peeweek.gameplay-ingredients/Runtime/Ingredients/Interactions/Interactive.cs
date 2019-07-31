using GameplayIngredients.Events;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayIngredients.Interactions
{
    public abstract class Interactive : EventBase
    {
        [Header("Events")]
        [SerializeField, ReorderableList]
        protected Callable[] OnInteract;

        protected virtual void OnEnable()
        {
            InteractionManager.RegisterInteractive(this);
        }

        protected virtual void OnDisable()
        {
            InteractionManager.RemoveInteractive(this);
        }

        public bool Interact(InteractiveUser user)
        {
            if (user.CanInteract(this) && CanInteract(user))
            {
                Callable.Call(OnInteract, user.gameObject);
                return true;
            }
            else
                return false;
        }

        public abstract bool CanInteract(InteractiveUser user);

    }
}
