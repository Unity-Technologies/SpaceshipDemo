
using UnityEngine;

namespace AxelF {

public sealed class AudioAnimHandler : MonoBehaviour {
    AudioAnimEvent[] events;

    void Awake() {
        var a = GetComponent<Animator>();
        if (a) events = a.GetBehaviours<AudioAnimEvent>();
    }

    void OnDisable() {
        foreach (var e in events)
            e.KeyOff();
    }
}

} // AxelF

