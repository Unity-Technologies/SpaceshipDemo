using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Experimental.Rendering.HDPipeline;

public class LightSourceEvents : MonoBehaviour
{
    public enum LightState { ON, SwitchingON, SwitchingOFF, OFF, Breaking, Broken, Special };
    private LightState currentState;

    public bool useRandomDelay;
    public float maxRandomDelay;
    [Range(0, 1)]
    public float bakedIndirectMultiplier = 1.0f;

    public LightState defaultState;

    public List<LightEvents> eventsReceivers;

    private void Start()
    {
        switch (defaultState)
        {
            case LightState.ON:
                {
                    SetON();
                    break;
                }
            case LightState.OFF:
                {
                    SetOFF();
                    break;
                }
            case LightState.SwitchingON:
                {
                    SwitchON();
                    break;
                }
            case LightState.SwitchingOFF:
                {
                    SwitchOFF();
                    break;
                }
            case LightState.Special:
                {
                    SpecialEvent();
                    break;
                }
        }
    }

    public void SwitchON()
    {
        if (useRandomDelay)
        {
            StartCoroutine(Delay(Random.Range(0, maxRandomDelay), "internalSwitchON"));
        }
        else
        {
            internalSwitchON();
        }
    }

    public void internalSwitchON()
    {
        if (currentState == LightState.OFF || currentState == LightState.SwitchingOFF)
        {
            onSwitchON();
            currentState = LightState.SwitchingON;
        }
    }

    private void onSwitchON()
    {
        foreach(LightEvents receiver in eventsReceivers)
        {
            receiver.onSwitchON.Invoke();
        }
    }

    public void SwitchOFF()
    {
        if (useRandomDelay)
        {
            StartCoroutine(Delay(Random.Range(0, maxRandomDelay), "internalSwitchOFF"));
        }
        else
        {
            internalSwitchOFF();
        }
    }

    public void internalSwitchOFF()
    {
        if (currentState == LightState.ON || currentState == LightState.SwitchingON)
        {
            onSwitchOFF();
            currentState = LightState.SwitchingOFF;
        }
    }

    private void onSwitchOFF()
    {
        foreach (LightEvents receiver in eventsReceivers)
        {
            receiver.onSwitchOFF.Invoke();
        }
    }

    public void  SetON()
    {
        if (useRandomDelay)
        {
            StartCoroutine(Delay(Random.Range(0, maxRandomDelay), "internalSetON"));
        }
        else
        {
            internalSetON();
        }
    }

    public void internalSetON()
    {
        if (currentState != LightState.Broken && currentState != LightState.ON)
        {
            onSetON();
            currentState = LightState.ON;
        }
    }

    private void onSetON()
    {
        foreach (LightEvents receiver in eventsReceivers)
        {
            receiver.onSetON.Invoke();
        }
    }

    public void SetOFF()
    {
        if (!useRandomDelay)
        {
            StartCoroutine(Delay(Random.Range(0, maxRandomDelay), "internalSetOFF"));
        }
        else
        {
            internalSetOFF();
        }
    }

    public void internalSetOFF()
    {
        if (currentState != LightState.Broken && currentState != LightState.OFF)
        {
            onSetOFF();
            currentState = LightState.OFF;
        }
    }

    private void onSetOFF()
    {
        foreach (LightEvents receiver in eventsReceivers)
        {
            receiver.onSetOFF.Invoke();
        }
    }

    public void Break()
    {
        if (useRandomDelay)
        {
            StartCoroutine(Delay(Random.Range(0, maxRandomDelay), "internalBreak"));
        }
        else
        {
            internalBreak();
        }
    }

    public void internalBreak()
    {
        if (currentState != LightState.Broken && currentState != LightState.Breaking)
        {
            currentState = LightState.Breaking;
            onBreak();
        }
    }

    private void onBreak()
    {
        foreach (LightEvents receiver in eventsReceivers)
        {
            receiver.onBreak.Invoke();
        }
    }

    public void SpecialEvent()
    {
        if (useRandomDelay)
        {
            StartCoroutine(Delay(Random.Range(0, maxRandomDelay), "internalSpecialEvent"));
        }
        else
        {
            internalSpecialEvent();
        }
    }

    public void internalSpecialEvent()
    {
        if (currentState != LightState.Broken && currentState != LightState.Breaking)
        {
            onSpecialEvent();
        }
    }

    private void onSpecialEvent()
    {
        foreach (LightEvents receiver in eventsReceivers)
        {
            receiver.onSpecialEvent.Invoke();
        }
    }

    public void SetBroken()
    {
        if(currentState != LightState.Broken)
            onSetBroken();
        currentState = LightState.Broken;
    }

    private void onSetBroken()
    {
        foreach (LightEvents receiver in eventsReceivers)
        {
            receiver.onSetBroken.Invoke();
        }
    }

    IEnumerator Delay(float duration, string function)
    {
        yield return new WaitForSeconds(duration);
        SendMessage(function);
    }

#if UNITY_EDITOR
    public void SetIndirectMultiplier()
    {
        var lights = GetComponentsInChildren<Light>();
        foreach (var light in lights)
        {
            if (light.lightmapBakeType != LightmapBakeType.Realtime)
                light.bounceIntensity = bakedIndirectMultiplier;

            light.SetLightDirty();
        }
    }

    public void SetLightDimmer()
    {
        if (defaultState == LightState.OFF || defaultState == LightState.SwitchingON || defaultState == LightState.Broken)
            internalSetOFF();
        else
            internalSetON();

        var lights = GetComponentsInChildren<Light>();
        foreach (var light in lights)
        {
            light.SetLightDirty();
        }
    }
#endif

    public void TriggerNextState()
    {
        switch (currentState)
        {
            case LightState.Breaking:
                {
                    SetBroken();
                    break;
                }
            case LightState.SwitchingON:
                {
                    SetON();
                    break;
                }
            case LightState.SwitchingOFF:
                {
                    SetOFF();
                    break;
                }
            case LightState.OFF:
                {
                    SwitchON();
                    break;
                }
            case LightState.Special:
                {
                    SetON();
                    break;
                }
        }
    }
}
