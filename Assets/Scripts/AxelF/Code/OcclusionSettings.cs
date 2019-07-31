
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace AxelF {

public enum OcclusionFunction {
    None,
    Distance,
    Raycast,
    Slapback
}

public class OcclusionSettings : ScriptableObject {
    public static readonly string path = "Assets/Features/AxelF/Resources/OcclusionSettings.asset";

    public static OcclusionSettings instance {
        get {
            var s = Resources.Load<OcclusionSettings>("OcclusionSettings");
#if UNITY_EDITOR
            if (s == null) {
                s = ScriptableObject.CreateInstance<OcclusionSettings>();
                AssetDatabase.CreateAsset(s, path);
            }
#endif
            return s;
        }
    }

#if UNITY_EDITOR
    //[MenuItem("AxelF/Settings/Occlusion Settings")]
    static void PingImportSettingst() {
        EditorGUIUtility.PingObject(instance);
    }
#endif

    public LayerMask layerMask = 1;

    [MinMax(0, 22000)]
    public MinMaxFloat highPassRange = new MinMaxFloat {
        min = 0,
        max = 1100,
    };

    [MinMax(0, 22000)]
    public MinMaxFloat lowPassRange = new MinMaxFloat {
        min = 4400,
        max = 22000,
    };

    public float speedOfSound = 340f;
}

} // AxelF

