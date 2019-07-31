
using UnityEngine;

namespace AxelF {

public struct WeightedDecay {
    const float restoreSpeed = 0.05f;
    const float redrawPenalty = 1f;

    public float[] weights;
    public float weightSum;

    public int count {
        get { return weights != null ? weights.Length : 0; }
        set {
            if (weights == null || weights.Length != value) {
                weights = new float[value];
                for (int i = 0; i < value; ++i)
                    weights[i] = 1f;
                weightSum = value;
            }
        }
    }

    public int Draw(float q, int n) {
        n = Mathf.Max(0, n);
        count = n;
        
        float v = Mathf.Clamp01(q) * weightSum;
        int i, j;
        for (i = 0; i < n - 1 && v >= weights[i]; ++i)
            v -= weights[i];

        if (i >= 0 && i < weights.Length)
            weights[i] = Mathf.Max(weights[i] - redrawPenalty, 0f);
        weightSum = 0f;
        for (j = 0; j < n; ++j) {
            weights[j] = Mathf.Min(weights[j] + restoreSpeed, 1f);
            weightSum += weights[j];
        }

        return i;
    }
}

} // AxelF

