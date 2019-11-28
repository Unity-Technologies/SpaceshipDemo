using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

[VFXBinder("VFX Volume Mixer/Vector Property Binder")]
public class VFXVolumeMixerVectorPropertyBinder : VFXVolumeMixerPropertyBinderBase
{
    [VFXVolumeMixerProperty(VFXVolumeMixerPropertyAttribute.PropertyType.Vector)]
    public int VectorMixerProperty = 0;
    [VFXPropertyBinding("UnityEngine.Vector3"), FormerlySerializedAs("VectorParameter")]
    public ExposedProperty VectorProperty = "VectorProperty";

    public override bool IsValid(VisualEffect component)
    {
        return base.IsValid(component) && VectorMixerProperty < 8 && VectorMixerProperty >= 0 && computedTransform != null && component.HasVector3(VectorProperty);
    }

    public override void UpdateBinding(VisualEffect component)
    {
        component.SetVector3(VectorProperty, VFXVolumeMixer.GetVectorValueAt(VectorMixerProperty, computedTransform, Layer));
    }

    public override string ToString()
    {
        return "VFXVolumeMixer Vector3 #"+ VectorMixerProperty+ " : " + VectorProperty.ToString()+" "+ base.ToString();
    }
}
