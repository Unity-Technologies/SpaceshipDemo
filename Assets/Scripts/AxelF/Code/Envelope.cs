
using UnityEngine;

namespace AxelF {

public enum EnvelopeMode {
    None,
    Exact,
    Min,
    Max
}

[System.Serializable]
public struct Envelope {
    internal enum Cadence {
        Snappy,
        Normal,
        Hesitant,
    }

    public static readonly Envelope instant = new Envelope {
        attackTime = 0f,
        attackValue = 1f,
        releaseTime = 0f,
        releaseValue = 0f,
        sustainValue = 1f
    };

    public float attackTime;
    internal float attackValue;
    internal Cadence attackCadence;

    public float releaseTime;
    internal float releaseValue;

    public float sustainValue;

    public void SetAttack(float t) {
        attackTime = t;
        attackValue = 0f;
        attackCadence =
            t <= 1f ? Cadence.Snappy :
            t <= 10f ? Cadence.Normal :
            Cadence.Hesitant;
    }

    public float GetAttackValue() {
        return attackValue;
    }

    public void SetRelease(float t) {
        releaseTime = t;
        releaseValue = 0f;
    }

    public float GetReleaseValue() {
        return releaseValue;
    }

    public float GetValue() {
        float a = attackValue;
        float r = releaseValue;
        if (attackCadence == Cadence.Snappy)
            a = (a - 1f) * (a - 1f) * (a - 1f) + 1f;
        else if (attackCadence == Cadence.Hesitant)
            a = a * a;
        r = 1f - ((r - 1f) * (r - 1f) * (r - 1f) + 1f);
        return a * r * sustainValue;
    }

    public float UpdateAttack(float dt) {
        float speed = attackTime > 0f ? 1f / attackTime : Mathf.Infinity;
        return attackValue = Mathf.Clamp01(attackValue + dt * speed);
    }

    public float UpdateRelease(float dt) {
        float speed = releaseTime > 0f ? 1f / releaseTime : Mathf.Infinity;
        return releaseValue = Mathf.Clamp01(releaseValue + dt * speed);
    }
}

} // AxelF

