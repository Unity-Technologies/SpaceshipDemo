using System;
using UnityEngine;
using UnityEngine.Events;

public enum LightAnimationMode { Curve, Noise, Constant };

[Serializable]
public class CurveLightAnimationSettings
{
    public AnimationCurve intensityCurve;
    
    public bool loopAnimation;
}

[Serializable]
public class NoiseLightAnimationSettings
{
    public float frequency = 5.0f;
    public float minimumValue = 0;
    public float maximumValue = 1;
    [Range(0.0f, 1.0f)]
    public float jumpFrequency = 0;
}

[Serializable]
public class CustomLightAnimationSettings
{
    public float value = 1.0f;
}

public class LightEventAnimation : MonoBehaviour
{

    public LightAnimationManager animationManager;
    public LightAnimationMode animationMode;
    public float animationLength = 1;

    public CurveLightAnimationSettings curveSettings;
    public NoiseLightAnimationSettings noiseSettings;
    public CustomLightAnimationSettings customSettings;

    public UnityEvent onAnimationEnd;

    private float currentValue = 1;
    private bool animate = false;
    private float localTime = 0;
    private int seed;

    void Start()
    {
        //animationManager = GetComponent<LightAnimationManager>();
        if (animationManager == null)
        {
            Debug.LogWarning("Could not find LightAnimationManager on " + gameObject.name);
            return;
        }
        animationManager.RegisterEvent(this);

        seed = (int)UnityEngine.Random.Range(0, 10000);
    }

    public float getCurrentValue()
    {
        if (animate)
        {
            localTime += Time.deltaTime;

            switch(animationMode)
            {
                case LightAnimationMode.Constant:
                    {
                        currentValue = customSettings.value;
                        break;
                    }
                case LightAnimationMode.Noise:
                    {
                        EvaluateNoiseAnimation();
                        break;
                    }
                case LightAnimationMode.Curve:
                    {
                        EvaluateCurveAnimation();
                        break;
                    }
            }
            
            //Animation END
            if (animationLength != 0 && localTime >= animationLength)
            {
                StopAnimate();
                onAnimationEnd.Invoke();
            }
        }
        return currentValue;
    }

    private void EvaluateCurveAnimation()
    {
        if(animationLength>0)
        {
            if (curveSettings.loopAnimation) { localTime = localTime % animationLength; }
            currentValue = curveSettings.intensityCurve.Evaluate(localTime / animationLength);
        }
    }

    private void EvaluateNoiseAnimation()
    {
        if (noiseSettings.jumpFrequency > 0)
        {
            float jumpRand;
            jumpRand = UnityEngine.Random.value;
            jumpRand = Mathf.Round(jumpRand * 10) / 10;
            if (jumpRand < noiseSettings.jumpFrequency)
            {
                localTime = localTime + 1;
            }
        }
        currentValue = samplePerlinNoise(localTime, noiseSettings.frequency, seed);
        currentValue = currentValue * (noiseSettings.maximumValue - noiseSettings.minimumValue) + noiseSettings.minimumValue;
    }

    private float samplePerlinNoise(float localtime, float frequency, int seed)
    {
        float noiseFade;
        noiseFade = Mathf.PerlinNoise(localtime * frequency, (float)seed);
        return noiseFade;
    }

    void Animate()
    {
        animate = true;
    }

    void PauseAnimate()
    {
        animate = false;
    }

    void StopAnimate()
    {
        PauseAnimate();
        currentValue = 1.0f;
        localTime = 0;
    }

    //Public Events
    public void StartAnimation()
    {
        Animate();
    }

    public void StopAnimation()
    {
        StopAnimate();
    }

    public void PauseAnimation()
    {
        PauseAnimate();
    }
}

public class SwitchOnLightAnimation : LightEventAnimation
{

}