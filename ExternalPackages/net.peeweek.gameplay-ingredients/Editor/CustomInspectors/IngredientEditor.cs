using GameplayIngredients.Editor;
using NaughtyAttributes.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public abstract class IngredientEditor : PingableEditor
{
    protected List<SerializedProperty> baseProperties;
    protected Dictionary<string, CallableReorderableList> reorderableLists;


    protected override void OnEnable()
    {
        base.OnEnable();

        if (baseProperties == null)
            baseProperties = new List<SerializedProperty>();
        else
            baseProperties.Clear();

        Type inspectedType = this.serializedObject.targetObject.GetType();
        foreach (FieldInfo info in inspectedType.FindMembers(MemberTypes.Field,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            null, null))
        {
            if (info.IsNotSerialized)
                continue;

            var property = serializedObject.FindProperty(info.Name);

            if (property != null)
            {
                baseProperties.Add(property);

                if (isCallableArray(property))
                {
                    if (reorderableLists == null)
                        reorderableLists = new Dictionary<string, CallableReorderableList>();
                    string key = GetPropertyKeyName(property);
                    reorderableLists[key] = new CallableReorderableList(serializedObject, property);
                }
            }
        }
    }

    static bool isCallableArray(SerializedProperty property) => property.isArray && property.arrayElementType == "PPtr<$Callable>";

    static string GetPropertyKeyName(SerializedProperty property) => property.serializedObject.targetObject.GetInstanceID() + "." + property.name;


    protected void DrawBaseProperties(params string[] excludedNames)
    {
        EditorGUILayout.Space();

        using (new EditorGUI.IndentLevelScope(1))
        {
            foreach (var prop in baseProperties)
            {
                if (excludedNames.Any(o => o == prop.name))
                    continue;
                if (isCallableArray(prop))
                    reorderableLists[GetPropertyKeyName(prop)].DoLayoutList();
                else
                    NaughtyEditorGUI.PropertyField_Layout(prop, true);
            }    
        }

        DrawButtons();
    }

    protected void DrawBreadCrumb(string label, Color color, Action content = null)
    {
        using (new GUILayout.HorizontalScope(Styles.breadCrumbBar))
        {
            Color c = GUI.backgroundColor;

            float bg = EditorGUIUtility.isProSkin ? .3f : .9f;
            GUI.backgroundColor = new Color(bg, bg, bg, 1);
            Rect r = GUILayoutUtility.GetRect(8,8, 22,22, GUILayout.Width(8));
            EditorGUI.DrawRect(r, color);
            GUILayout.Label(label, Styles.breadCrumb, GUILayout.MinWidth(28));

            GUI.backgroundColor = c;

            if (content != null)
                content.Invoke();
            else
                GUILayout.FlexibleSpace();
        }
    }

    protected void DrawDebugButton(GUIContent content, Action onClick, params GUILayoutOption[] options)
    {
        if (GUILayout.Button(content, Styles.drawDebugButton, options))
        {
            onClick?.Invoke();
        }
    }

    protected void OpenIngredientsExplorerButton(MonoBehaviour target)
    {
        DrawDebugButton(Styles.callableIconContent, ()=> IngredientsExplorerWindow.OpenWindow(target), GUILayout.Width(28));
    }

    protected static class Styles
    {
        public static GUIContent callableIconContent;
        public static GUIStyle drawDebugButton;
        public static GUIStyle breadCrumb;
        public static GUIStyle breadCrumbBar;
        static Texture2D bgTexture;
        static Styles()
        {
            callableIconContent = new GUIContent(AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/net.peeweek.gameplay-ingredients/Icons/Misc/ic-callable.png"));

            drawDebugButton = new GUIStyle(EditorStyles.miniButton);
            drawDebugButton.margin = new RectOffset(1,1,1,1);
            drawDebugButton.fixedHeight = 20;


            breadCrumbBar = new GUIStyle(EditorStyles.toolbar);
            breadCrumbBar.fixedHeight = 22;
            breadCrumbBar.margin = new RectOffset(0, 0, 0, 8);


            bgTexture = new Texture2D(1, 1);
            bgTexture.SetPixel(0, 0, new Color(0, 0, 0, 0.1f));
            bgTexture.Apply();

            breadCrumbBar.onNormal.background = bgTexture;
            breadCrumbBar.onActive.background = bgTexture;
            breadCrumbBar.onFocused.background = bgTexture;
            breadCrumbBar.onHover.background = bgTexture;
            breadCrumbBar.normal.background = bgTexture;
            breadCrumbBar.active.background = bgTexture;
            breadCrumbBar.focused.background = bgTexture;
            breadCrumbBar.hover.background = bgTexture;

            var bc = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/net.peeweek.gameplay-ingredients/Icons/BreadCrumb.png");
            breadCrumb = new GUIStyle(EditorStyles.boldLabel);
            breadCrumb.fixedHeight = 22;
            breadCrumb.border = new RectOffset(8, 28, 0, 0);
            breadCrumb.padding = new RectOffset(8, 32, 0, 0);
            breadCrumb.margin = new RectOffset();

            breadCrumb.onNormal.background = bc;
            breadCrumb.onNormal.textColor = EditorGUIUtility.isProSkin? Color.white : Color.black;
            breadCrumb.onHover = breadCrumb.onNormal;
            breadCrumb.onActive = breadCrumb.onNormal;
            breadCrumb.onFocused = breadCrumb.onNormal;
            breadCrumb.normal = breadCrumb.onNormal;
            breadCrumb.hover = breadCrumb.onNormal;
            breadCrumb.active = breadCrumb.onNormal;
            breadCrumb.focused = breadCrumb.onNormal;

        }
    }
}