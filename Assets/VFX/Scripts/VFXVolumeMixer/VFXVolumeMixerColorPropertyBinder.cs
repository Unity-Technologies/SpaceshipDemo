using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

[VFXBinder("VFX Volume Mixer/Color Property Binder")]
public class VFXVolumeMixerColorPropertyBinder : VFXVolumeMixerPropertyBinderBase
{
    [VFXVolumeMixerProperty(VFXVolumeMixerPropertyAttribute.PropertyType.Color)]
    public int ColorMixerProperty = 0;
    [VFXPropertyBinding("UnityEngine.Color"), FormerlySerializedAs("ColorParameter")]
    public ExposedProperty ColorProperty = "ColorProperty";

    public override bool IsValid(VisualEffect component)
    {
        return base.IsValid(component) && ColorMixerProperty < 8 && ColorMixerProperty >= 0 && computedTransform != null && component.HasVector4(ColorProperty);
    }

    public override void UpdateBinding(VisualEffect component)
    {
        component.SetVector4(ColorProperty, VFXVolumeMixer.GetColorValueAt(ColorMixerProperty, computedTransform, Layer)); 
    }

    public override string ToString()
    {
        return "VFXVolumeMixer Color #" + ColorMixerProperty + " : " + ColorProperty.ToString() + " " + base.ToString();
    }
}
