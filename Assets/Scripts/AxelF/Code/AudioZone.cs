
using System.Collections.Generic;
using UnityEngine;
using Serializable = System.SerializableAttribute;
using NonSerializedAttribute = System.NonSerializedAttribute;

namespace AxelF {

public class AudioZone : Zone {
    public static AudioEmitter[] FindEmitters(AudioZone z) {
        return z.ownership == Ownership.Local ?
            z.GetComponents<AudioEmitter>() : z.GetComponentsInChildren<AudioEmitter>();
    }

    public enum Ownership {
        Local,
        Deep
    }

    public Ownership ownership;
    public LayerMask layerMask = 0;

    [MinMax(0, 1)]
    public MinMaxFloat peripheralFade = new MinMaxFloat {
        min = 1f, max = 1f
    };

    internal AudioEmitter[] emitters;
    internal int lastFrame;
    internal float sqrDistance;
    internal float sqrRadius;

    TernaryBool hasEnabledEmitters;

    public bool hasPeripheralFade { get { return peripheralFade.min < 1f; }}

    protected override void OnInit() {
        base.OnInit();
        emitters = FindEmitters(this);
        for (int i = 0, n = emitters.Length; i < n; ++i)
            emitters[i].zone = this;
    }

    protected new void OnEnable() {
        base.OnEnable();
        lastFrame = -1;
    }

    protected new void OnDisable() {
        for (int i = 0, n = emitters.Length; i < n; ++i)
            emitters[i].enabled = false;
        base.OnDisable();
    }

    protected void OnTriggerEnter(Collider c) {
        if (_trigger != null
                && (layerMask & (1 << c.gameObject.layer)) != 0
                && _triggerRefs++ == 0)
            SetActive(true);
    }

    protected void OnTriggerExit(Collider c) {
        if (_trigger != null
                && (layerMask & (1 << c.gameObject.layer)) != 0
                && --_triggerRefs == 0)
            SetActive(false);
    }

    protected override void OnProbe(Vector3 lpos, int thisFrame) {
        if ((_trigger == null || layerMask == 0) && lastFrame != thisFrame) {
            lastFrame = thisFrame;

            bool makeActive = isActiveAndEnabled;
            if (makeActive) {
                if (_trigger != null)
                    makeActive = _trigger.bounds.Contains(lpos);
                else {
                    var pos = transform.position;
                    sqrDistance = (lpos - pos).sqrMagnitude;
                    sqrRadius = radius * radius;
                    makeActive = (sqrDistance <= sqrRadius);
                }
            }
            SetActive(makeActive);
        }
    }

    protected override void OnUpdateEmitters() {
        bool wantEnabledEmitters = active == true && volumeExclusion < 1f;
        if (hasEnabledEmitters != wantEnabledEmitters) {
            hasEnabledEmitters = wantEnabledEmitters;
            for (int i = 0, n = emitters.Length; i < n; ++i)
                emitters[i].enabled = wantEnabledEmitters;
        }
    }
}

} // AxelF

