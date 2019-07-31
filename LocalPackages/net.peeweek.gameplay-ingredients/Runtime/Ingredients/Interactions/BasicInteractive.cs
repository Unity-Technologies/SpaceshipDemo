using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayIngredients.Interactions
{
    public class BasicInteractive : Interactive
    {
        [Header("Settings")]
        public float Distance = 1.0f;

        public override bool CanInteract(InteractiveUser user)
        {
            return Vector3.Distance(user.transform.position, this.transform.position) < Distance;
        }
    }
}
