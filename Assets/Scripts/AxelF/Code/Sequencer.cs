
// #define SEQUENCER_PARANOIA

using System.Collections.Generic;
using UnityEngine;

namespace AxelF {

public enum CueStatus {
    Playing,
    Stopped,
    Repeating
}

public struct Cue {
    public AudioEmitter emitter;
    public int index;
    public uint cueHandle;
    public uint keyHandle;
    public int lastFrame;
    public float modVolume;
    public float currentTime;
    public float totalTime;
    public int repeatIndex;
    public int repeatCount;
    public bool looping;

    bool UpdateModVolume(out float vol, float dt) {
        float v = 1f;
        bool set = false;

        if (emitter.zone) {
            if (emitter.zone.hasPeripheralFade) {
                v *= emitter.zone.volumeInfluence;
                set = true;
            }
            if (emitter.zone.isVolumeExcluded) {
                v *= 1f - emitter.zone.volumeExclusion;
                set = true;
            }
        }

        if (emitter.isModulated) {
            var c = emitter.modulation.custom;
            if (c != null) {
                v *= c.GetCustomModulation();
                set = true;
            } else {
                float t = currentTime / emitter.modulation.period * (Mathf.PI * 2f);
                float x = 1f - (Mathf.Cos(t) + 1f) * 0.5f;
                float y = emitter.modulation.volume.GetRangedValue(x);
                if (emitter.modulation.inverted)
                    y = 1f - y;
                v *= y;
                set = true;
            }
        }

        vol = modVolume = Mathf.Lerp(modVolume, v, 2f * dt);
        return set;
    }

    public CueStatus Update(float dt) {
        var s = CueStatus.Playing;
        if (lastFrame == Time.frameCount)
            return s;
        lastFrame = Time.frameCount;
        if (!emitter.paused) {
            currentTime += dt;
            if (totalTime > 0f && currentTime >= totalTime) {
                s = CueStatus.Stopped;
                if (repeatCount < 0 || ++repeatIndex < repeatCount)
                    s = CueStatus.Repeating;
#if SEQUENCER_PARANOIA
                Debug.LogFormat(
                    Time.frameCount.ToString("X4") +
                    " Cue.Update: {0} {1} {2} {3}/{4} {5}/{6}",
                    emitter.name, emitter.patches[index] ? emitter.patches[index].name : "???",
                    s, currentTime, totalTime, repeatIndex, repeatCount);
#endif
            }

            float v;
            if (UpdateModVolume(out v, dt))
                if (keyHandle != 0)
                    Synthesizer.SetModVolume(keyHandle, v);
        }
        return s;
    }

    public bool KeyOn() {
#if SEQUENCER_PARANOIA
        Debug.LogFormat(
            Time.frameCount.ToString("X4") +
            " Sequencer.KeyOn: {0} {1}",
            emitter.name, emitter.patches[index] ? emitter.patches[index].name : "???");
#endif
        if (Randomizer.zeroToOne <= emitter.randomization.chance) {
            if (emitter.auxiliary.source) {
                if (!emitter.patches[index].hasTimings)
                    Synthesizer.KeyOn(emitter.patches[index], emitter.auxiliary.source, 0f, emitter.volume);
            } else {
                var t =
                    emitter.attachment.useListenerTransform ? Heartbeat.listenerTransform :
                    emitter.attachment.transform ? emitter.attachment.transform :
                    emitter.transform;
                float r = emitter.randomization.distance.GetRandomValue();
                var p = Vector3.zero;
                if (!Mathf.Approximately(r, 0f)) {
                    float a = Randomizer.plusMinusOne * Mathf.PI * 2f;
                    r *= emitter.zone.radius;
                    p.x = Mathf.Sin(a) * r;
                    p.z = Mathf.Cos(a) * r;
                }
                float v;
                UpdateModVolume(out v, 1000f);
                keyHandle = Synthesizer.KeyOn(
                    out looping, emitter.patches[index], t, p, 0f, emitter.volume, v);
            }
        }
        return emitter.patches[index].GetCueInfo(out totalTime, out repeatCount) || looping;
    }

    public void KeyOff(float release, EnvelopeMode mode) {
#if SEQUENCER_PARANOIA
        Debug.LogFormat(
            Time.frameCount.ToString("X4") +
            " Sequencer.KeyOff: {0} {1} : {2} {3}",
            emitter.name, emitter.patches[index] ? emitter.patches[index].name : "???",
            release, mode);
#endif
        if (keyHandle != 0)
            Synthesizer.KeyOff(keyHandle, release, mode);
        Reset();
    }

    public void Reset() {
        keyHandle = 0;
        currentTime = 0f;
        looping = false;
    }
}

public static class Sequencer {
    public static List<Cue> activeCues0 = new List<Cue>(64);
    public static List<Cue> activeCues1 = new List<Cue>(64);

    public static void Reset() {
        activeCues0.Clear();
        activeCues1.Clear();
    }

    public static uint CueIn(AudioEmitter e, int i) {
        var c = new Cue {emitter = e, index = i, cueHandle = Synthesizer.GetNextHandle()};
        if (!c.KeyOn())
            return 0;
        activeCues0.Add(c);
        return c.cueHandle;
    }

    public static void CueOut(uint handle, float release = 0f, EnvelopeMode mode = EnvelopeMode.None) {
        for (var x = activeCues0.GetEnumerator(); x.MoveNext();) {
            var z = x.Current;
            if (z.cueHandle == handle)
                z.KeyOff(release, mode);
            else
                activeCues1.Add(z);
        }
        Swap(ref activeCues0, ref activeCues1);
        activeCues1.Clear();
    }

    static void Swap<T>(ref List<T> a, ref List<T> b) {
        var y = a;
        a = b;
        b = y;
    }

    public static void Update(float dt) {
        for (var x = activeCues0.GetEnumerator(); x.MoveNext();) {
            var z = x.Current;
            var s = z.Update(dt);
            if (s == CueStatus.Repeating) {
                z.Reset();
                z.KeyOn();
            }
            if (s != CueStatus.Stopped)
                activeCues1.Add(z);
        }
        Swap(ref activeCues0, ref activeCues1);
        activeCues1.Clear();
    }
}

} // AxelF

