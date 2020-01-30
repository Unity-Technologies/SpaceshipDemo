using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace GameplayIngredients.Editor
{
    public class GlobalsDebugWindow : EditorWindow
    {
        [MenuItem("Window/Gameplay Ingredients/Globals Debug")]
        static void Open()
        {
            GetWindow<GlobalsDebugWindow>();
        }

        private void OnEnable()
        {
            titleContent = new GUIContent("Globals Debug");
            minSize = new Vector2(360, 140);
            Globals.OnGlobalsUpdated += Globals_OnGlobalsUpdated;
        }

        private void Globals_OnGlobalsUpdated (Globals.Type t, string name, object value)
        {
            Repaint();
        }

        private void OnDisable()
        {
            Globals.OnGlobalsUpdated -= Globals_OnGlobalsUpdated;
        }

        Vector2 scroll;
        private void OnGUI()
        {
            var localBools = Globals.GetBoolNames(Globals.Scope.Local);
            var globalBools = Globals.GetBoolNames(Globals.Scope.Global);
            var localInts = Globals.GetIntNames(Globals.Scope.Local);
            var globalInts = Globals.GetIntNames(Globals.Scope.Global);
            var localFloats = Globals.GetFloatNames(Globals.Scope.Local);
            var globalFloats = Globals.GetFloatNames(Globals.Scope.Global);
            var localStrings = Globals.GetStringNames(Globals.Scope.Local);
            var globalStrings = Globals.GetStringNames(Globals.Scope.Global);
            var localObjects = Globals.GetObjectNames(Globals.Scope.Local);
            var globalObjects = Globals.GetObjectNames(Globals.Scope.Global);

            GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 1);
            using(new GUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                GUILayout.Label("Name", Styles.header);
                GUILayout.Label("Global", Styles.header, GUILayout.Width(64));
                GUILayout.Label("Type", Styles.header, GUILayout.Width(64));
                GUILayout.Label("Value", Styles.header, GUILayout.Width(128));
            }
            GUI.backgroundColor = Color.white;

            scroll = EditorGUILayout.BeginScrollView(scroll);
            
            foreach (var item in globalBools.OrderBy(o => o))   { DrawItem(item, Globals.Scope.Global,  Globals.Type.Boolean);       }
            foreach (var item in localBools.OrderBy(o => o))    { DrawItem(item, Globals.Scope.Local,   Globals.Type.Boolean);       }
            foreach (var item in globalInts.OrderBy(o => o))    { DrawItem(item, Globals.Scope.Global,  Globals.Type.Integer);       }
            foreach (var item in localInts.OrderBy(o => o))     { DrawItem(item, Globals.Scope.Local,   Globals.Type.Integer);       }
            foreach (var item in globalFloats.OrderBy(o => o))  { DrawItem(item, Globals.Scope.Global,  Globals.Type.Float);         }
            foreach (var item in localFloats.OrderBy(o => o))   { DrawItem(item, Globals.Scope.Local,   Globals.Type.Float);         }
            foreach (var item in globalStrings.OrderBy(o => o)) { DrawItem(item, Globals.Scope.Global,  Globals.Type.String);        }
            foreach (var item in localStrings.OrderBy(o => o))  { DrawItem(item, Globals.Scope.Local,   Globals.Type.String);        }
            foreach (var item in globalObjects.OrderBy(o => o)) { DrawItem(item, Globals.Scope.Global,  Globals.Type.GameObject);    }
            foreach (var item in localObjects.OrderBy(o => o))  { DrawItem(item, Globals.Scope.Local,   Globals.Type.GameObject);    }

            EditorGUILayout.EndScrollView();
        }

        void DrawItem(string name, Globals.Scope scope, Globals.Type type)
        {
            using(new GUILayout.HorizontalScope())
            {
                GUILayout.Label(name, Styles.cell);
                GUILayout.Label(scope.ToString(), Styles.cell, GUILayout.Width(64));
                GUILayout.Label(type.ToString(), Styles.cell, GUILayout.Width(64));
                switch (type)
                {
                    case Globals.Type.Boolean:
                        GUILayout.Toggle(Globals.GetBool(name, scope),"", GUILayout.Width(128));
                        break;
                    case Globals.Type.Integer:
                        GUILayout.TextField(Globals.GetInt(name, scope).ToString(), GUILayout.Width(128));
                        break;
                    case Globals.Type.String:
                        GUILayout.TextField(Globals.GetString(name, scope).ToString(), GUILayout.Width(128));
                        break;
                    case Globals.Type.Float:
                        GUILayout.TextField(Globals.GetFloat(name, scope).ToString(), GUILayout.Width(128));
                        break;
                    case Globals.Type.GameObject:
                        EditorGUILayout.ObjectField("",Globals.GetObject(name, scope), typeof(GameObject), true, GUILayout.Width(128));
                        break;
                    default:
                        break;
                }
            }
        }

        static class Styles
        {
            public static GUIStyle header;
            public static GUIStyle cell;

            static Styles()
            {
                header = new GUIStyle(EditorStyles.toolbarButton);
                header.alignment = TextAnchor.MiddleLeft;
                header.fontStyle = FontStyle.Bold;

                cell = new GUIStyle(EditorStyles.toolbarButton);
                cell.alignment = TextAnchor.MiddleLeft;
                cell.fontSize = 10;
                
            }
        }
    }
}
