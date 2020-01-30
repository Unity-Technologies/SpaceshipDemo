using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

enum ChangeType
{
    Color,
    Intensity,
    Both
}

[ExecuteInEditMode]
public class MaterialEmissionOverride : MonoBehaviour
{
    public int materialID = 0; //which material ID to modify? (typically, set to 0)
    [ColorUsage(false,true)]
    public Color color = Color.white;
    public float intensity = 1f;
    string emissiveColorName = "_EmissionColor";
    string emissiveIntensityName = "_EmissiveIntensity";
    [Range(0,1)]
    public float emissiveDimmer = 1.0f;
    public bool driveEmissiveWithLight = true;
    public HDAdditionalLightData additionalLight;
    private Renderer rend;
    private MaterialPropertyBlock mpb;

    private Light drivingLight;
    private Color _oldColor;
    private Color _originalColor;
    private float _oldIntensity;
    private float _originalIntensity;

    private void OnEnable()
    {
        if (rend == null)
            rend = GetComponent<Renderer>();
        _originalColor = rend.sharedMaterial.GetColor(emissiveColorName).gamma;
        _originalIntensity = rend.sharedMaterial.GetFloat(emissiveIntensityName);
        if (driveEmissiveWithLight && additionalLight != null)
        {
            drivingLight = additionalLight.GetComponent<Light>();
        }
        if (driveEmissiveWithLight && additionalLight == null)
        {
            Debug.LogWarning(gameObject.name + "'s emissive tries to read a light's intensity but the reference is null.");
        }
        _oldColor = _originalColor;
        _oldIntensity = _originalIntensity;
    }

    // Update is called once per frame
    void Update()
    {
        if (additionalLight != null && driveEmissiveWithLight)
        {
            if (!additionalLight.gameObject.activeInHierarchy || !drivingLight.enabled)
            {
                emissiveDimmer = 0.0f;
            }
            else
            {
                emissiveDimmer = additionalLight.lightDimmer;
            }
            if (drivingLight != null)
                color = drivingLight.color;
        }

        //Change to HDR color, intensity is in the color now
        var finalColor = _originalColor * color;
        var finalIntensity = _originalIntensity * intensity * emissiveDimmer;

        if(finalColor != _oldColor && finalIntensity != _oldIntensity)
        {
            SetPropertyBlock(ChangeType.Both, finalColor, finalIntensity);
        }

        //update color if the value has changed
        else if (_oldColor != finalColor)
        {
            SetPropertyBlock(ChangeType.Color, finalColor, finalIntensity);
        }

        //update intensity if the value has changed
        else if (_oldIntensity != finalIntensity)
        {
            SetPropertyBlock(ChangeType.Intensity, finalColor, finalIntensity);
        }
    }

    void SetPropertyBlock(ChangeType type, Color color, float intensity)
    {
        //ensure a renderer is available
        if (rend == null)
            rend = GetComponent<Renderer>();

        if (rend != null)
        {
            //create a new material property block
            mpb = new MaterialPropertyBlock();

            //get material property block
            rend.GetPropertyBlock(mpb, materialID);

            //change color
            if (type == ChangeType.Color)
                mpb.SetColor(emissiveColorName, color);

            //change intensity
            if (type == ChangeType.Intensity)
                mpb.SetFloat(emissiveIntensityName, intensity);

            //set material property block
            rend.SetPropertyBlock(mpb, materialID);

            _oldColor = color;
            _oldIntensity = intensity;
        }
    }
}
