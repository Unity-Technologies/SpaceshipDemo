using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayIngredients.Pickup
{
    [HelpURL(Help.URL + "pickup")]
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

