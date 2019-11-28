using GameplayIngredients.Actions;
using UnityEngine;
using UnityEngine.VFX;
using NaughtyAttributes;
using UnityEngine.Serialization;

public class VFXSetPropertyAction : ActionBase
{
    public enum DataType
    {
        Bool,
        Float,
        Vector2,
        Vector3,
        Vector4,
        Texture2D,
        Texture3D,
        UInt,
        Int
    }

    public VisualEffect visualEffect;

    [FormerlySerializedAs("parameter")]
    public string property = "Property";
    public bool Override = true;

    [SerializeField]
    protected DataType dataType = DataType.Float;

    [ShowIf("isBool")]
    public bool BoolValue = false;
    [ShowIf("isFloat")]
    public float FloatValue = 0.0f;
    [ShowIf("isVector2")]
    public Vector2 Vector2Value = Vector2.zero;
    [ShowIf("isVector3")]
    public Vector3 Vector3Value = Vector3.zero;
    [ShowIf("isVector4")]
    public Vector4 Vector4Value = Vector4.zero;
    [ShowIf("isTexture2D")]
    public Texture2D Texture2DValue;
    [ShowIf("isTexture3D")]
    public Texture3D Texture3DValue;
    [ShowIf("isUInt")]
    public uint UIntValue = 0;
    [ShowIf("isInt")]
    public int IntValue = 0;

    public override void Execute(GameObject instigator = null)
    {
        int id = Shader.PropertyToID(property);

        if(HasParameter(id))
        {
            if (!Override)
                visualEffect.ResetOverride(id);
            else
            {
                switch (dataType)
                {
                    case DataType.Bool:             visualEffect.SetBool(id, BoolValue);          break;
                    case DataType.Float:            visualEffect.SetFloat(id, FloatValue);        break;
                    case DataType.Vector2:          visualEffect.SetVector2(id, Vector2Value);    break;
                    case DataType.Vector3:          visualEffect.SetVector3(id, Vector3Value);    break;
                    case DataType.Vector4:          visualEffect.SetVector4(id, Vector4Value);    break;
                    case DataType.Texture2D:        visualEffect.SetTexture(id, Texture2DValue);  break;
                    case DataType.Texture3D:        visualEffect.SetTexture(id, Texture3DValue);  break;
                    case DataType.UInt:             visualEffect.SetUInt(id, UIntValue);          break;
                    case DataType.Int:              visualEffect.SetInt(id, IntValue);            break;
                }
            }
        }
    }

    bool HasParameter(int id)
    {
        switch(dataType)
        {
            case DataType.Bool:         return visualEffect.HasBool(id);
            case DataType.Float:        return visualEffect.HasFloat(id);
            case DataType.Vector2:      return visualEffect.HasVector2(id);
            case DataType.Vector3:      return visualEffect.HasVector3(id);
            case DataType.Vector4:      return visualEffect.HasVector4(id);
            case DataType.Texture2D:    return visualEffect.HasTexture(id);
            case DataType.Texture3D:    return visualEffect.HasTexture(id);
            case DataType.UInt:         return visualEffect.HasUInt(id);
            case DataType.Int:          return visualEffect.HasInt(id);
        }
        return false;
    }

    public override string GetDefaultName()
    {
        if (!Override)
            return $"Reset VFX override for parameter :'{property}' ({dataType})";
        else
        {
            switch (dataType)
            {

                case DataType.Bool:         return $"{visualEffect.name} {property} {BoolValue} ";
                case DataType.Float:        return $"{visualEffect.name} {property} {FloatValue} ";
                case DataType.Vector2:      return $"{visualEffect.name} {property} {Vector2Value} ";
                case DataType.Vector3:      return $"{visualEffect.name} {property} {Vector3Value} ";
                case DataType.Vector4:      return $"{visualEffect.name} {property} {Vector4Value} ";
                case DataType.Texture2D:    return $"{visualEffect.name} {property} {Texture2DValue} ";
                case DataType.Texture3D:    return $"{visualEffect.name} {property} {Texture3DValue} ";
                case DataType.UInt:         return $"{visualEffect.name} {property} {UIntValue} ";
                case DataType.Int:          return $"{visualEffect.name} {property} {IntValue} ";
                default:
                    return $"Set VFX <UNKNOWN> parameter :'{property}'";
            }
        }
    }

    bool isBool()       { return dataType == DataType.Bool;       }
    bool isFloat()      { return dataType == DataType.Float;      }
    bool isVector2()    { return dataType == DataType.Vector2;    }
    bool isVector3()    { return dataType == DataType.Vector3;    }
    bool isVector4()    { return dataType == DataType.Vector4;    }
    bool isTexture2D()  { return dataType == DataType.Texture2D;  }
    bool isTexture3D()  { return dataType == DataType.Texture3D;  }
    bool isUInt()       { return dataType == DataType.UInt;       }
    bool isInt()        { return dataType == DataType.Int;        }
}
