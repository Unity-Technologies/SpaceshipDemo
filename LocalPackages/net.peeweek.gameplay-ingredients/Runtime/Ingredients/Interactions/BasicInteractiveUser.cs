using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayIngredients.Interactions
{
    public class BasicInteractiveUser : InteractiveUser
    {
        public Camera Camera;
        public float InteractionAngle = 60.0f;

        public override bool CanInteract(Interactive interactive)
        {
            Vector3 toInteractive = (interactive.transform.position - Camera.transform.position).normalized;
            return Mathf.Acos(Vector3.Dot(toInteractive, Camera.transform.forward)) < InteractionAngle;
        }

        public override Interactive[] SortCandidates(IEnumerable<Interactive> candidates)
        {
            return candidates.OrderBy(a => Vector3.Distance(a.gameObject.transform.position, this.transform.position)).ToArray();
        }
    }
}

