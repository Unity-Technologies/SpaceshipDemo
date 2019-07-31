using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEngine.Experimental.VFX;
using System.Collections.Generic;

public class VFXSceneUsage : EditorWindow
{
    [MenuItem("Window/Visual Effects/Utilities/VFX Scene Usage")]
    public static void OpenWindow()
    {
        GetWindow<VFXSceneUsage>();
    }

    public Dictionary<VisualEffectAsset, int> assets;
    public Vector2 scrollPosition;

    public void OnGUI()
    {
        if (assets == null)
            PopulateAssets();

        using (new GUILayout.HorizontalScope())
        {
            GUILayout.Label("Visual Effect Assets", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            if(GUILayout.Button("Refresh", EditorStyles.miniButton))
            {
                PopulateAssets();
            }
        }
        GUILayout.Space(8);

        EditorGUILayout.BeginScrollView(scrollPosition);

        foreach(var kvp in assets)
        {
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Space(16);
                if (GUILayout.Button(kvp.Key.name, EditorStyles.label))
                {
                    EditorGUIUtility.PingObject(kvp.Key);
                }
                GUILayout.FlexibleSpace();
                GUILayout.Label(kvp.Value.ToString());

            }
        }
        EditorGUILayout.EndScrollView();
    }

    void PopulateAssets()
    {
        assets = new Dictionary<VisualEffectAsset, int>();
        var all = Resources.FindObjectsOfTypeAll<VisualEffect>();
        foreach(var vfx in all)
        {
            if (vfx.visualEffectAsset == null)
                continue;

            if (!assets.ContainsKey(vfx.visualEffectAsset))
                assets.Add(vfx.visualEffectAsset, 1);
            else
                assets[vfx.visualEffectAsset]++;
        }
    }
}
