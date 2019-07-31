using GameplayIngredients;
using GameplayIngredients.Interactions;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class ARUIInteractive : Interactive
{
    public float MaxDistance = 5.0f;

    [ReorderableList]
    public Callable[] OnHoverIn;

    [ReorderableList]
    public Callable[] OnHoverOut;

    bool m_Hover;

    protected override void OnEnable()
    {
        base.OnEnable();
        m_Hover = false;
    }

    public void Hover(InteractiveUser user, bool state)
    {
        if (state != m_Hover)
        {
            if(state) // hover in
            {
                Callable.Call(OnHoverIn, user.gameObject);
            }
            else // hover out
            {
                Callable.Call(OnHoverOut, user.gameObject);
            }
            m_Hover = state;
        }
    }

    public override bool CanInteract(InteractiveUser user)
    {
        return Vector3.SqrMagnitude(user.transform.position - transform.position) < (MaxDistance * MaxDistance);
    }
}
