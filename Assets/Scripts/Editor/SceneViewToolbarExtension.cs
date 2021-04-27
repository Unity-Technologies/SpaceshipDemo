using GameplayIngredients.Editor;
using UnityEditor;
using UnityEngine;

public static class SceneViewToolbarExtension
{
    [InitializeOnLoadMethod]
    static void RegisterToolbarCallback()
    {
        SceneViewToolbar.OnSceneViewToolbarGUI += SceneViewToolbar_OnSceneViewToolbarGUI;
    }

    private static void SceneViewToolbar_OnSceneViewToolbarGUI(SceneView sceneView)
    {
        int currentLevel = QualitySettings.GetQualityLevel();
        string[] names = QualitySettings.names;
        GUILayout.Space(16);
        Rect r = GUILayoutUtility.GetLastRect();
        r.yMax += 20;
        r.xMin += 16;
        if(GUILayout.Button($"Quality : {names[currentLevel]}",EditorStyles.toolbarDropDown))
        {
            GenericMenu m = new GenericMenu();
            for(int i = 0; i<names.Length;i++)
            {
                m.AddItem(new GUIContent(names[i]), i == currentLevel, SetQualityMenu, i);
            }
            m.DropDown(r);
        }
    }

    static void SetQualityMenu(object level)
    {
        QualitySettings.SetQualityLevel((int)level);
        var switches = Resources.FindObjectsOfTypeAll<QualitySwitch>();
        foreach(var sw in switches)
        {
            sw.UpdateQuality((int)level);
        }
    }
}
