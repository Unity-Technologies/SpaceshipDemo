
using UnityEngine;

namespace AxelF {

[System.Serializable]
public struct MinMaxFloat {
    public float min;
    public float max;

    public float GetRandomValue() {
        return GetRangedValue(Randomizer.zeroToOne);
    }

    public float GetRangedValue(float v) {
        return v * (max - min) + min;
    }

    public float GetClampedValue(float v) {
        return Mathf.Clamp(v, min, max);
    }
}

} // AxelF

