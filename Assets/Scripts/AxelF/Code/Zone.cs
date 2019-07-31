
using System.Collections.Generic;
using UnityEngine;
using NonSerializedAttribute = System.NonSerializedAttribute;

namespace AxelF {

public abstract class Zone : MonoBehaviour {
    public static readonly List<Zone> allZones = new List<Zone>();
    public static bool dontProbeZones;

    public static Collider FindTrigger(Zone z) {
        var c = z.GetComponent<Collider>();
        return (c != null && c.isTrigger) ? c : null;
    }

    static float GetTriggerRadius(Collider c) {
        var e = c.bounds.extents;
        return Mathf.Max(e.x, e.z);
    }

#if UNITY_EDITOR
    [System.Serializable]
    public struct GizmoParams {
        public Color activeColor;
        public Color inactiveColor;
    }

    [Colorize]
    public GizmoParams gizmo = new GizmoParams {
        activeColor = Color.magenta,
        inactiveColor = Color.blue
    };
#endif

    internal Zone parent;
    internal List<Zone> children;

    public float radius = 5f;

    [Range(0, 1)]
    public float parentExclusion;
    internal float volumeExclusion;
    internal float volumeInfluence;

    internal int hash;
    internal bool inited;
    internal bool wantActive;
    internal TernaryBool active;

    protected Collider _trigger;
    protected int _triggerRefs;

    public bool isVolumeExcluded { get { return volumeExclusion > 0f; }}

    public Collider trigger { get { return inited ? _trigger : FindTrigger(this); }}

#if UNITY_EDITOR
    public bool IsActive() { return active == true; }
    public Color GetGizmoColor() { return IsActive() ? gizmo.activeColor : gizmo.inactiveColor; }
#endif

    protected void OnEnable() {
        if (!inited)
            OnInit();
        allZones.Add(this);
        OnUpdateActivation(wantActive);
    }

    protected virtual void OnInit() {
        if ((_trigger = FindTrigger(this)) == null)
            RegisterWithParentZone();
        hash = (int) Synthesizer.GetNextHandle();
        inited = true;
    }

    void RegisterWithParentZone() {
        var z = FindParentZone();
        if (z != null) {
            if (z.children == null)
                z.children = new List<Zone>(4);
            z.children.Add(this);
            parent = z;
        }
    }

    public float GetRadius() {
        var t = trigger;
        return t ? GetTriggerRadius(t) : radius;
    }

    public Zone FindParentZone() {
        return FindParentZoneRecursive(transform.parent);
    }

    Zone FindParentZoneRecursive(Transform t) {
        if (t != null) {
            var z = t.GetComponent<Zone>();
            return (z != null) ? z : FindParentZoneRecursive(t.parent);
        }
        return null;
    }

    protected void OnDisable() {
        allZones.Remove(this);
        OnUpdateActivation(false);
        _triggerRefs = 0;
    }

    protected void SetActive(bool state) {
        wantActive = state;
    }

    protected bool OnUpdateActivation(bool state) {
        if (active == state)
            return false;
        active = state;
        if (children != null)
            foreach (var i in children)
                i.enabled = state;
        return true;
    }

    protected virtual void OnProbe(Vector3 lpos, int thisFrame) {
    }

    protected virtual void OnUpdateEmitters() {
    }

    internal static void Update(int thisFrame) {
        UpdateProbes(thisFrame);
        UpdateActivation();
        UpdateInfluence();
        UpdateExclusion();
        UpdateEmitters();
    }

    static void UpdateProbes(int thisFrame) {
        if (!dontProbeZones) {
            var l = Heartbeat.listenerTransform;
            if (l) {
                var lpos = l.position;
                foreach (var z in allZones)
                    z.OnProbe(lpos, thisFrame);
            }
        }
    }

    static void UpdateActivation() {
        _10:
        foreach (var z in allZones)
            if (z.OnUpdateActivation(z.wantActive))
                goto _10;
    }

    static void UpdateInfluence() {
        foreach (var z in allZones) {
            // default is full influence and no exclusion
            z.volumeExclusion = 0f;
            z.volumeInfluence = z.active == true ? 1f : 0f;

            // for active audio zones with peripheral fade, adjust the influence
            AudioZone az;
            if (z.active == true && (az = z as AudioZone) != null) {
                float pfMin = az.peripheralFade.min;
                float pfMax = az.peripheralFade.max;
                if (pfMin < 1f) {
                    float x = az.sqrDistance / az.sqrRadius;
                    float y = 1f - Mathf.Clamp01((x - pfMin) / (pfMax - pfMin));
                    az.volumeInfluence = y * y;
                }
            }
        }
    }

    static void UpdateExclusion() {
        foreach (var z in allZones)
            if (z.active == true && z.parent == null) {
                float e;
                UpdateExclusionDepthFirst(z, out e);
            }
    }

    static void UpdateExclusionDepthFirst(Zone z, out float exclusion) {
        // calculate max exclusion of all children
        float e = 0f;
        if (z.children != null)
            foreach (var c in z.children) {
                float f;
                UpdateExclusionDepthFirst(c, out f);
                e = Mathf.Max(e, f);
            }

        // keep track of our own exclusion
        z.volumeExclusion = e;

        // apply our own influence and exclusion and pass it up to parent
        float k = e + 1f;
        k *= z.volumeInfluence * z.parentExclusion + 1f;
        exclusion = Mathf.Clamp01(k - 1f);
    }

    static void UpdateEmitters() {
        foreach (var z in allZones)
            z.OnUpdateEmitters();
    }
}

} // AxelF

