using GameplayIngredients.Rigs;
using System;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class LightMaterialBindingRig : Rig
{
    [Serializable]
    public struct MaterialOverride
    {
        public MeshRenderer meshRenderer;
        public int materialIndex;
        public string emissionPropertyName;
        public float emissionRange;
        public bool isHDRPStdMaterial;
    }

    [Header("Animation")]
    [SerializeField]
    bool autoPlay = false;
    [SerializeField]
    Vector2 minMaxStartTime = Vector2.zero;

    [SerializeField]
    AnimationCurve animationCurve = AnimationCurve.EaseInOut(0,1,1,0);

    [Header("Rig Elements")]

    [SerializeField]
    HDAdditionalLightData[] lights;

    [SerializeField]
    MaterialOverride[] materialOverrides;

    Material[] cachedMaterials;

    public override UpdateMode defaultUpdateMode => UpdateMode.Update;
    public override int defaultPriority => 0;

    float m_TTL;
    bool playing;

    protected override void OnEnable()
    {
        base.OnEnable();
        ResetAnimation();
        playing = autoPlay;

        cachedMaterials = new Material[materialOverrides.Length];
    }

    public void ResetAnimation()
    {
        m_TTL = Mathf.Lerp(minMaxStartTime.x, minMaxStartTime.y, UnityEngine.Random.Range(0f,1f));
    }

    public void Play()
    {
        playing = true;
    }

    public void Pause()
    {
        playing = false;
    }
    public void Stop()
    {
        Pause();
        ResetAnimation();
    }

    public override void UpdateRig(float deltaTime)
    {
        if (!playing)
            return;

        m_TTL += deltaTime;
        float value = animationCurve.Evaluate(m_TTL);

        if(lights != null)
        {
            foreach (var light in lights)
            {
                if (light != null)
                    light.SetLightDimmer(value, value);
            }
        }

        if(materialOverrides != null)
        {
            int i = 0;
            foreach(var ovr in materialOverrides)
            {
                if(cachedMaterials[i] == null)
                    cachedMaterials[i] = ovr.meshRenderer.materials[ovr.materialIndex];

                if (cachedMaterials[i] == null)
                    continue;

                cachedMaterials[i].SetFloat(ovr.emissionPropertyName, ovr.emissionRange * value);

                if(ovr.isHDRPStdMaterial)
                    UpdateEmissiveColorFromIntensityAndEmissiveColorLDR(cachedMaterials[i]);

                i++;
            }
        }
    }

    static void UpdateEmissiveColorFromIntensityAndEmissiveColorLDR(Material material)
    {
        const string kEmissiveColorLDR = "_EmissiveColorLDR";
        const string kEmissiveColor = "_EmissiveColor";
        const string kEmissiveIntensity = "_EmissiveIntensity";

        if (material.HasProperty(kEmissiveColorLDR) && material.HasProperty(kEmissiveIntensity) && material.HasProperty(kEmissiveColor))
        {
            // Important: The color picker for kEmissiveColorLDR is LDR and in sRGB color space but Unity don't perform any color space conversion in the color
            // picker BUT only when sending the color data to the shader... So as we are doing our own calculation here in C#, we must do the conversion ourselves.
            Color emissiveColorLDR = material.GetColor(kEmissiveColorLDR);
            Color emissiveColorLDRLinear = new Color(Mathf.GammaToLinearSpace(emissiveColorLDR.r), Mathf.GammaToLinearSpace(emissiveColorLDR.g), Mathf.GammaToLinearSpace(emissiveColorLDR.b));
            material.SetColor(kEmissiveColor, emissiveColorLDRLinear * material.GetFloat(kEmissiveIntensity));
        }
    }
}
