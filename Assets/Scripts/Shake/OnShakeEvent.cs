using GameplayIngredients;
using GameplayIngredients.Events;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.VFX;
using UnityEngine.Experimental.VFX.Utility;

public class OnShakeEvent : EventBase
{
    [Header("Delay")]
    public float maxDelay = 0.25f;

    [Header("Rigid Body Interactions")]
    [ReorderableList, NonNullCheck]
    public Rigidbody[] rigidBodies;
    public float impulseGain = 1.0f;

    [Header("VisualEffect Interactions")]
    [ReorderableList, NonNullCheck]
    public VisualEffect[] visualEffects;
    public string eventName = "OnPlay";
    public string stopEventName = "OnStop";
    public bool playOnlyIfVisible = true;
    public float disableAfterDelay = 1.0f;

    [Tooltip("Sets the impact position to position attribute")]
    public bool setPositionAttribute = false;
    [Tooltip("Sets the impact strength to size attribute")]
    public bool setSizeAttribute = false;
    [Tooltip("Sets the impact force to velocity attribute")]
    public bool setVelocityAttribute = false;


    [Header("Events")]
    [ReorderableList]
    public Callable[] OnShake;

    private void OnEnable()
    {
        Manager.Get<ShakeManager>().RegisterShake(this);
        eventAttributes = new Dictionary<VisualEffect, VFXEventAttribute>();
    }
    private void OnDisable()
    {
        Manager.Get<ShakeManager>().UnRegisterShake(this);
    }

    Dictionary<VisualEffect, VFXEventAttribute> eventAttributes;
    ExposedParameter position = "position";
    ExposedParameter size = "size";
    ExposedParameter velocity = "velocity";

    Coroutine shakeCoroutine;

    public void Shake(ShakeManager.Settings shake, float attenuation)
    {
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        shakeCoroutine = StartCoroutine(ShakeCoroutine(shake, attenuation));
    }

    public IEnumerator ShakeCoroutine(ShakeManager.Settings shake, float attenuation)
    {
        float dist = Vector3.Distance(transform.position, shake.Position);
        //float intensity = shake.Intensity / Mathf.Pow(dist + 1, attenuation);
        float intensity = shake.Intensity;
        Vector3 forceVector = Vector3.Normalize(transform.position - shake.Position) * intensity;

        if(maxDelay > 0.0f)
            yield return new WaitForSeconds(Random.Range(0.0f,maxDelay));

        if (rigidBodies != null)
        {
            foreach(var rb in rigidBodies)
            {
                if (rb == null) continue;
                rb.AddForce(forceVector * impulseGain, ForceMode.Impulse);
            }
        }

        if(visualEffects != null)
        {
            foreach(var vfx in visualEffects)
            {
                if (vfx == null || (playOnlyIfVisible && vfx.culled))
                    continue;

                if (!eventAttributes.ContainsKey(vfx))
                    eventAttributes.Add(vfx, vfx.CreateVFXEventAttribute());

                if(setPositionAttribute)
                    eventAttributes[vfx].SetVector3(position, shake.Position);
                if(setSizeAttribute && eventAttributes[vfx].HasFloat(size))
                    eventAttributes[vfx].SetFloat(size, intensity);
                if(setVelocityAttribute)
                    eventAttributes[vfx].SetVector3(velocity, forceVector);

                vfx.SendEvent(eventName, eventAttributes[vfx]);
            }
            
            if (visualEffects.Length > 0 && disableAfterDelay > 0.0f)
            {
                if(stopVFXCoroutine != null)
                    StopCoroutine(stopVFXCoroutine);
                stopVFXCoroutine = StartCoroutine(StopAfter());
            }
        }
        Callable.Call(OnShake);
    }

    Coroutine stopVFXCoroutine;

    IEnumerator StopAfter()
    {
        yield return new WaitForSeconds(disableAfterDelay);
        foreach (var vfx in visualEffects)
        {
            if (vfx == null || (playOnlyIfVisible && vfx.culled))
                continue;

            vfx.SendEvent(stopEventName);
        }
    }
}
