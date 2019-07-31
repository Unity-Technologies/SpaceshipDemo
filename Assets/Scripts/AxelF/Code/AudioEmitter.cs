
using UnityEngine;
using UnityEngine.Serialization;

namespace AxelF {

[AddComponentMenu("AxelF/AudioEmitter")]
public class AudioEmitter : MonoBehaviour {
    [FormerlySerializedAs("assets")]
    public Patch[] patches = null;
    uint[] cueHandles;

    [Range(0, 1)]
    public float volume = 1f;
    public bool autoCue = true;
    public bool singleShot = false;
    bool cuedOnce;

    public enum Controller {
        None,
        User,
        Zone
    }

    [HideInInspector]
    public Controller controller;

    [System.Serializable]
    public struct RandomizationParams {
        [Range(0, 1)]
        public float chance;
        [MinMax(0, 1)]
        public MinMaxFloat distance;
    }

    [Colorize]
    public RandomizationParams randomization = new RandomizationParams {
        chance = 1f
    };

    [System.Serializable]
    public struct ModulationParams {
        [MinMax(0, 2)]
        public MinMaxFloat volume;
        [Range(0, 1800)]
        public float period;
        public bool inverted;
        internal CustomModulator custom;
    }

    public interface CustomModulator {
        float GetCustomModulation();
    }

    [Colorize]
    public ModulationParams modulation = new ModulationParams {
        volume = new MinMaxFloat {min = 0.95f, max = 1.05f}
    };

    [System.Serializable]
    public struct AttachmentParams {
        public Transform transform;
        public bool useListenerTransform;
    }

    [Colorize]
    public AttachmentParams attachment;

    [System.Serializable]
    public struct AuxiliaryParams {
        public AudioSource source;
    }

    [Colorize]
    public AuxiliaryParams auxiliary;

    [System.Serializable]
    public struct GizmoParams {
        public Color color;
    }

    [Colorize]
    public GizmoParams gizmo = new GizmoParams {
        color = Color.red,
    };

    internal AudioZone zone;
    internal bool paused;

    public bool isModulated { get { return modulation.custom != null || modulation.period > 0f; }}

    public void SetPaused(bool p) { paused = p; }
    public bool IsPaused() { return paused; }

    protected void Reset() {
        if (controller == Controller.None)
            controller = !zone && !GetComponent<Zone>() ? Controller.User : Controller.Zone;
        enabled = controller != Controller.Zone;
    }

    protected void Awake() {
        Reset();
    }

    protected void OnEnable() {
        if (autoCue)
            CueIn();
    }

    protected void OnDisable() {
        CueOut();
    }

    public void CueIn() {
        if (!singleShot || !cuedOnce) {
            CueOut();
            if (cueHandles == null || cueHandles.Length != patches.Length)
                cueHandles = new uint[patches.Length];
            for (int i = 0, n = cueHandles.Length; i < n; ++i)
                cueHandles[i] = Sequencer.CueIn(this, i);
            cuedOnce = true;
        }
    }

    public void CueOut() {
        CueOutWithRelease(0f, EnvelopeMode.None);
    }

    public void CueOutWithRelease(float release, EnvelopeMode mode) {
        if (cueHandles != null)
            for (int i = 0, n = cueHandles.Length; i < n; ++i) {
                Sequencer.CueOut(cueHandles[i], release, mode);
                cueHandles[i] = 0;
            }
    }

    public void CueOutInstant() {
        CueOutWithRelease(0f, EnvelopeMode.Exact);
    }
}

} // AxelF

