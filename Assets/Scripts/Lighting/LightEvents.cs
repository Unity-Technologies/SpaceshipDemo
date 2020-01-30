using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Rendering.HighDefinition;

public class LightEvents : MonoBehaviour
{
    public UnityEvent onSwitchON;
    public UnityEvent onSetON;
    public UnityEvent onSwitchOFF;
    public UnityEvent onSetOFF;
    public UnityEvent onBreak;
    public UnityEvent onSetBroken;
    public UnityEvent onSpecialEvent;

    public void SwitchON()
    {
        onSwitchON.Invoke();
    }
    public void SetON()
    {
        onSetON.Invoke();
    }
    public void SwitchOFF()
    {
        onSwitchOFF.Invoke();
    }
    public void SetOFF()
    {
        onSetOFF.Invoke();
    }
    public void Break()
    {
        onBreak.Invoke();
    }
    public void SpecialEvent()
    {
        onSpecialEvent.Invoke();
    }

    public void SetBroken()
    {
        onSetBroken.Invoke();
    }

}
