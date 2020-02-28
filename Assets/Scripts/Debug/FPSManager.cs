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

    public KeyCode PauseKey = KeyCode.F5;
    public KeyCode StepKey = KeyCode.F6;

    bool paused = false;
    bool step = false;

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

        if(paused)
        {
            if(step)
            {
                Time.timeScale = 1.0f;
                step = false;
            }
            else
            {
                Time.timeScale = 0.0f;
            }
        }
        else
        {
            Time.timeScale = 1.0f;
        }

        if (Input.GetKeyDown(PauseKey))
        {
            paused = !paused;
        }
        else if (Input.GetKeyDown(StepKey))
        {
            paused = true;
            step = true;
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
