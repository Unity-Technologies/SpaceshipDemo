using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayIngredients.Pickup
{
    public abstract class PickupOwnerBase : MonoBehaviour
    {
        public bool PickUp(PickupItem pickup)
        {
            foreach (var effect in pickup.effects)
            {
                effect.ApplyPickupEffect(this);
            }
            return true;
        }
    }
}

