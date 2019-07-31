using GameplayIngredients.Interactions;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using GameplayIngredients;

public class ARUIInteractiveUser : InteractiveUser
{
    public string Layer = "Gameplay";

    public override bool CanInteract(Interactive interactive)
    {
        Transform c = Manager.Get<VirtualCameraManager>().transform;

        SphereCollider collider = interactive.GetComponent<SphereCollider>();
        if (collider == null)
            return false;

        RaycastHit hit;
        if(Physics.Raycast(new Ray(c.position, c.forward),out hit, ((ARUIInteractive)interactive).MaxDistance, LayerMask.GetMask(Layer)))
        {
            return hit.collider == collider;
        }
        return false;
    }

    public override Interactive[] SortCandidates(IEnumerable<Interactive> candidates)
    {
        return candidates.OrderBy(o => Vector3.SqrMagnitude(o.gameObject.transform.position - gameObject.transform.position)).ToArray();
    }

    private void Update()
    {
        var all = InteractionManager.interactives;
        var candidates = InteractionManager.GetCandidates(this);

        foreach (var interactive in all)
        {
            var arui = (ARUIInteractive)interactive;
            bool hover = (candidates.Length > 0) && (arui != null) && (CanInteract((ARUIInteractive)interactive));
            arui.Hover(this, hover);
        }
    }

}
