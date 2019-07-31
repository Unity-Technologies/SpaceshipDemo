using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering.HDPipeline;

public class LightAnimationManager : MonoBehaviour
{
    private HDAdditionalLightData additionalLightData;
    private MaterialEmissionOverride matBlockHandler;
    private float _oldValue;

    //private LightEventAnimation[] lightAnimations;
    private List<LightEventAnimation> lightAnimations = new List<LightEventAnimation>();

    private void OnEnable()
    {
        //lightAnimations = GetComponents<LightEventAnimation>();
        if (gameObject.GetComponent<HDAdditionalLightData>())
            additionalLightData = gameObject.GetComponent<HDAdditionalLightData>();
        else if (gameObject.GetComponent<MaterialEmissionOverride>())
            matBlockHandler = gameObject.GetComponent<MaterialEmissionOverride>();
    }

    public void RegisterEvent(LightEventAnimation lightEvent)
    {
        lightAnimations.Add(lightEvent);
    }

    void Update()
    {
        if (additionalLightData == null && matBlockHandler == null)
            return;
        if (lightAnimations.Count <= 0)
            return;
        var currentValue = 1.0f;
        foreach (var lightAnimator in lightAnimations)
        {
            currentValue *= lightAnimator.getCurrentValue();
        }
        //if( _oldValue != currentValue )
        //{
            if (matBlockHandler != null)
                matBlockHandler.emissiveDimmer = currentValue;
            else if (additionalLightData != null)
                additionalLightData.lightDimmer = currentValue;

            _oldValue = currentValue;
        //}
    }

    public void SetDimmer(float dimmer)
    {
        OnEnable();

        if (matBlockHandler != null)
            matBlockHandler.emissiveDimmer = dimmer;
        if (additionalLightData != null)
            additionalLightData.lightDimmer = dimmer;
    }
}