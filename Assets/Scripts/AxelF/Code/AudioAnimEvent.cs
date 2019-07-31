
using UnityEngine;
using UnityEngine.Serialization;

namespace AxelF {

public sealed class AudioAnimEvent : StateMachineBehaviour {
    [FormerlySerializedAs("asset")]
    public Patch patch;
    [Range(0, 30)] public float delay;
    public Vector3 offset = Vector3.up;
    uint handle;

    public void KeyOn(Animator a) {
        bool looping;
        handle = Synthesizer.KeyOn(out looping, patch, a.transform, offset, delay);
    }

    public void KeyOff() {
        if (handle != 0) {
            Synthesizer.KeyOff(handle);
            handle = 0;
        }
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo info, int layer) {
        KeyOn(animator);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo info, int layer) {
        KeyOff();
    }
}

} // AxelF

