using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayIngredients.Pickup
{
    public abstract class PickupEffectBase : MonoBehaviour
    {
        public abstract void ApplyPickupEffect(PickupOwnerBase owner);
    }
}
