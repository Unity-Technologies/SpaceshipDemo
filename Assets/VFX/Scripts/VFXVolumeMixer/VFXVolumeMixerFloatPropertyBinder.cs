using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

[VFXBinder("VFX Volume Mixer/Float Property Binder")]
public class VFXVolumeMixerFloatPropertyBinder : VFXVolumeMixerPropertyBinderBase
{
    [VFXVolumeMixerProperty(VFXVolumeMixerPropertyAttribute.PropertyType.Float)]
    public int FloatMixerProperty = 0;
    [VFXPropertyBinding("System.Single"), FormerlySerializedAs("FloatParameter")]
    public ExposedProperty FloatProperty = "FloatProperty";

    public override bool IsValid(VisualEffect component)
    {
        return base.IsValid(component) && FloatMixerProperty < 8 && FloatMixerProperty >= 0 && computedTransform != null && component.HasFloat(FloatProperty);
    }

    public override void UpdateBinding(VisualEffect component)
    {
        component.SetFloat(FloatProperty, VFXVolumeMixer.GetFloatValueAt(FloatMixerProperty, computedTransform, Layer));
    }

    public override string ToString()
    {
        return "VFXVolumeMixer Float #" + FloatMixerProperty + " : " + FloatProperty.ToString() + " " + base.ToString();
    }
}
