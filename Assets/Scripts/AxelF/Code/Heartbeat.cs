
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Profiling;

namespace AxelF {

public class Heartbeat : MonoBehaviour {
    public static Transform hierarchyTransform { get; private set; }
    public static Transform listenerTransform;
    public static Transform playerTransform;

    public AudioMixer audioMixer;
    public string rotationAngleParameter = "AxelF Rotation Angle";

    public static float ambienceWindStrength { get; private set; }
    public static float ambienceWindThreshold = 0.05f;
    public static float ambienceWindInterpolatedThreshold = 0.05f;

    protected void Awake() {
        hierarchyTransform = transform;
    }

    protected void OnDestroy() {
        Sequencer.Reset();
        Synthesizer.Reset();

        if (hierarchyTransform == transform)
            hierarchyTransform = null;
    }

    protected void Update() {
        var dt = Time.deltaTime;
        int tf = Time.frameCount;

        Profiler.BeginSample("Update Zones");
        Zone.Update(tf);
        Profiler.EndSample();

        Profiler.BeginSample("Update Sequencer");
        Sequencer.Update(dt);
        Profiler.EndSample();

        Profiler.BeginSample("Update Synthesizer");
        Synthesizer.Update(dt);
        Profiler.EndSample();

        // Ship hack: let wind ambience influence wind strength

        ambienceWindInterpolatedThreshold =
            Mathf.Lerp(ambienceWindInterpolatedThreshold, ambienceWindThreshold, dt);

        float normalized = 0 / ambienceWindInterpolatedThreshold;
        float capped = Mathf.Min(normalized, 3f);
        float curve = (capped * 2f) * (capped * 2f) - 1f;

        ambienceWindStrength = Mathf.Lerp(ambienceWindStrength, curve, dt * normalized);
    }

    protected void LateUpdate() {
        if (playerTransform && audioMixer) {
            float halfRadians = playerTransform.localEulerAngles.y * Mathf.Deg2Rad * 0.5f;
            if (!audioMixer.SetFloat(rotationAngleParameter, halfRadians))
                Debug.LogWarning("Failed to set audio mixer parameter: " + rotationAngleParameter);
        }
    }

    public void StartRecording(string name) {
        if (listenerTransform == null)
            Debug.LogWarning("StartRecording: no listener");
        else {
            var r = listenerTransform.GetComponent<RecordToFile>();
            if (!r) r = listenerTransform.gameObject.AddComponent<RecordToFile>();
            r.StartRecording(name);
        }
    }

    public int StopRecording() {
        if (listenerTransform == null) {
            Debug.LogWarning("StopRecording: no listener");
            return -1;
        } else {
            var r = listenerTransform.GetComponent<RecordToFile>();
            return r ? r.StopRecording() : -1;
        }
    }
}

} // AxelF

