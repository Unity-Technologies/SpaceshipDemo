using GameplayIngredients;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

[ManagerDefaultPrefab("FPSManager")]
public class FPSManager : Manager
{
    public KeyCode ToggleKey = KeyCode.F8;
    public GameObject FPSRoot;
    public Text FPSCounter;
    public Text MillisecondCounter;
    public float smoothDeltaTimePeriod = 1.5f;

    private void Update()
    {
        float dt = GetSmoothDeltaTime();

        if(Input.GetKeyDown(ToggleKey) && FPSRoot != null)
            FPSRoot.SetActive(!FPSRoot.activeInHierarchy);

        if(FPSRoot.activeInHierarchy)
        {
            if (FPSCounter != null)
                FPSCounter.text = $"FPS: {((1.0f / dt).ToString("F1"))}";

            if (MillisecondCounter != null)
                MillisecondCounter.text = $"{((dt * 1000).ToString("F2"))}ms.";
        }
    }

    Queue<float> deltaTimeSamples = new Queue<float>();

    float GetSmoothDeltaTime()
    {
        float time = Time.unscaledTime;
        deltaTimeSamples.Enqueue(time);

        if (deltaTimeSamples.Count > 1)
        {
            float startTime = deltaTimeSamples.Peek();
            float delta = time - startTime;

            float smoothDelta = delta / deltaTimeSamples.Count;

            if (delta > smoothDeltaTimePeriod)
                deltaTimeSamples.Dequeue();

            return smoothDelta;
        }
        else
            return Time.unscaledDeltaTime;
    }

}
