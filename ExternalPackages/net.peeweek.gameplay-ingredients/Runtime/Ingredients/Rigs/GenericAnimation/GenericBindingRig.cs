using NaughtyAttributes;
using System;
using UnityEngine;


namespace GameplayIngredients.Rigs
{

    public class GenericBindingRig : Rig
    {
        public enum BindingType
        {   
            Bool,
            Int,
            UInt,
            Float,
            Vector2,
            Vector3,
            Vector4,
            Quaternion,
            Color
        }
        [SerializeField]
        BindingType bindingType = BindingType.Float;

        [InfoBox("Reads the value of SOURCE property and stores it into TARGET property")]

        [SerializeField, ReflectedMember("typeForBinding")]
        ReflectedMember source;
        [SerializeField, ReflectedMember("typeForBinding")]
        ReflectedMember target;

        Type typeForBinding
        {
            get 
            {
                switch (bindingType)
                {
                    case BindingType.Bool:
                        return typeof(bool);
                    case BindingType.Int:
                        return typeof(int);
                    case BindingType.UInt:
                        return typeof(uint);
                    case BindingType.Float:
                        return typeof(float);
                    case BindingType.Vector2:
                        return typeof(Vector2);
                    case BindingType.Vector3:
                        return typeof(Vector3);
                    case BindingType.Vector4:
                        return typeof(Vector4);
                    case BindingType.Quaternion:
                        return typeof(Quaternion);
                    case BindingType.Color:
                        return typeof(Color);
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public override UpdateMode defaultUpdateMode => UpdateMode.Update;

        public override int defaultPriority => 0;

        public override void UpdateRig(float deltaTime)
        {
            target.SetValue(source.GetValue());
        }
    }
}

