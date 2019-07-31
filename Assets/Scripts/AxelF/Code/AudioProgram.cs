
using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

namespace AxelF {

public enum Priority {
    VeryHigh = 64,
    High = 96,
    Normal = 128,
    Low = 160,
    VeryLow = 192
}

[Serializable]
public struct Parameters {
    public const float defaultVolume = 0.95f;
    public const float defaultPitch = 1f;
    public const float defaultVariation = 0.05f;
    public const float defaultBlend = 1f;
    public const float defaultDoppler = 0.8f;
    public const float defaultMinDistance = 1f;
    public const float defaultMaxDistance = 40f;
    public const float defaultOcclusionMinDistance = 20f;
    public const float defaultOcclusionMaxDistance = 50f;
    public const float defaultSlapbackVolumeAttenuation = 0.4f;
    public const float defaultSlapbackPitchAttenuation = 0.8f;

    public static readonly Parameters defaultParameters = new Parameters {
        loop = false,
        volume = new MinMaxFloat {
            min = defaultVolume - defaultVariation,
            max = defaultVolume + defaultVariation
        },
        pitch = new MinMaxFloat {
            min = defaultPitch - defaultVariation,
            max = defaultPitch + defaultVariation
        },
        spatial = new SpatialParams {
            blend = defaultBlend,
            distance = new MinMaxFloat {
                min = defaultMinDistance,
                max = defaultMaxDistance
            },
            doppler = defaultDoppler
        },
        randomization = new RandomizationParams {
        },
        occlusion = new OcclusionParams {
            function = OcclusionFunction.None
        },
        slapback = new SlapbackParams {
        },
        runtime = new RuntimeParams {
            priority = Priority.Normal,
        }
    };

    public bool loop;

    [MinMax(0, 1)]
    public MinMaxFloat volume;

    [MinMax(0, 10)]
    public MinMaxFloat pitch;

    [Range(-1, 1)]
    public float panning;

    [Serializable]
    public struct EnvelopeParams {
        [Range(0, 60)]
        public float attack;
        [Range(0, 60)]
        public float release;
    }

    [Colorize]
    public EnvelopeParams envelope;

    [Serializable]
    public struct SpatialParams {
        [Range(0, 1)]
        public float blend;
        [MinMax(0, 1000)]
        public MinMaxFloat distance;
        [Range(0, 5)]
        public float doppler;
    }

    [Colorize]
    public SpatialParams spatial;

    [Serializable]
    public struct RandomizationParams {
        [MinMax(0, 1)]
        public MinMaxFloat distance;
    }

    [Colorize]
    public RandomizationParams randomization;

    [Serializable]
    public struct OcclusionParams {
        public OcclusionFunction function;
    }

    [Colorize]
    public OcclusionParams occlusion;

    [Serializable]
    public struct SlapbackParams {
        [FormerlySerializedAs("asset")]
        public Patch patch;
    }

    [Colorize]
    public SlapbackParams slapback;

    [Serializable]
    public struct RuntimeParams {
        public Priority priority;
    }

    [Colorize]
    public RuntimeParams runtime;

    public float GetVolume() {
        return volume.GetRandomValue();
    }

    public float GetPitch() {
        return pitch.GetRandomValue();
    }
}

[Serializable]
public class AudioProgram {
    [Serializable]
    public struct AudioClipParams {
        public AudioClip clip;
        [Range(-1, 1)]
        public float gain;
    }

    [Space(10)]
    [Colorize]
    public AudioClipParams[] clips;
    [Colorize]
    public bool randomize = true;
    [Colorize]
    public bool increment = true;
    [HideInInspector]
    public WeightedDecay weighted;

    [Space(10)]
    [Colorize]
    public AudioMixerGroup mixerGroup;

    [Space(10)]
    [Colorize]
    public Parameters parameters = Parameters.defaultParameters;

    [NonSerialized]
    public int lastFrame;
    [NonSerialized]
    public int clipIndex;

    [NonSerialized]
    public Patch patch;

    public void Initialize() {
        lastFrame = -1;
        clipIndex = 0;
    }

    internal float GetMaxDuration() {
        float n = 0f;
        foreach (var c in clips)
            n = Mathf.Max(n, c.clip.length);
        return n;
    }

    public AudioClip GetClip(out float gain) {
        if (randomize)
            return GetClip(Randomizer.zeroToOne, out gain);
        int i = clipIndex;
        var c = clips[i];
        if (increment)
            clipIndex = (clipIndex + 1) % clips.Length;
        gain = c.gain;
        return c.clip;
    }

    public AudioClip GetClip(float q, out float gain) {
        return GetClipAt(clipIndex = weighted.Draw(q, clips.Length), out gain);
    }

    public AudioClip GetClipAt(int i, out float gain) {
        var c = clips[i];
        gain = c.gain;
        return c.clip;
    }

    public bool Activate(
            ActivationParams ap
#if UNITY_EDITOR
            , Patch patch
#endif
    ) {
        bool delayed = !Mathf.Approximately(ap.delay, 0f);
        if (delayed || (!randomize && !increment) ||
                lastFrame != Time.frameCount || !Application.isPlaying) {
            if (!delayed)
                lastFrame = Time.frameCount;
            return Synthesizer.Activate(this, ap
#if UNITY_EDITOR
                , patch
#endif
            );
        }
        return false;
    }

    public bool Activate(
            ActivationParams ap, Parameters.EnvelopeParams eo
#if UNITY_EDITOR
            , Patch patch
#endif
    ) {
        bool delayed = !Mathf.Approximately(ap.delay, 0f);
        if (delayed || (!randomize && !increment) ||
                lastFrame != Time.frameCount || !Application.isPlaying) {
            if (!delayed)
                lastFrame = Time.frameCount;
            return Synthesizer.Activate(
                this, eo, ap
#if UNITY_EDITOR
                , patch
#endif
            );
        }
        return false;
    }
}

} // AxelF

