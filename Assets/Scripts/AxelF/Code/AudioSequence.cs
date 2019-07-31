
using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace AxelF {

[Serializable]
public class Timing {
    [FormerlySerializedAs("asset")]
    public Patch patch;

    [MinMax(0, 600)]
    public MinMaxFloat delay;

    [Serializable]
    public struct RandomizationParams {
        [Range(0, 1)]
        public float chance;
    }

    [Colorize(order = 0)]
    public RandomizationParams randomization = new RandomizationParams {
        chance = 1f
    };

    public float GetDelay() {
        return delay.GetRandomValue();
    }
}

[Serializable]
public class AudioSequence {
    [Space(10)]
    [Colorize]
    public Timing[] timing;

    [Space(10)]
    [MinMax(0, 600, colorize = true)]
    public MinMaxFloat duration;

    [Serializable]
    public struct RepeatParams {
        [Range(0, 99)]
        public int count;
        public bool forever;
    }

    [Colorize]
    public RepeatParams repeat = new RepeatParams {
        forever = true
    };

    [NonSerialized]
    public uint lastHandle;

    [NonSerialized]
    public Patch patch;

    internal float GetMaxDuration() {
        return duration.max * (1 + repeat.count);
    }

    public float GetDuration() {
        return duration.GetRandomValue();
    }

    public bool GetCueInfo(out float duration, out int repeats) {
        duration = GetDuration();
        repeats = repeat.forever ? -1 : repeat.count;
        return duration > Mathf.Epsilon;
    }

    public bool Activate(ActivationParams ap) {
        if (lastHandle != ap.handle || !Application.isPlaying) {
            lastHandle = ap.handle;
            return Synthesizer.Activate(this, ap);
        }
        return false;
    }
}

} // AxelF

