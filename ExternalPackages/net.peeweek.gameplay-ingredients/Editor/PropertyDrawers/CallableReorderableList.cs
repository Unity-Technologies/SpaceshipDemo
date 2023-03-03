using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Linq;
using System.Reflection;

namespace GameplayIngredients.Editor
{
    public class CallableReorderableList : ReorderableList
    {
        public CallableReorderableList(SerializedObject targetObject, SerializedProperty listProperty)
            : base(targetObject, listProperty, true, true, true, true)
        {
            onAddDropdownCallback = AddDropdown;
            drawHeaderCallback = DrawHeader;
            drawElementCallback = DrawElement;
        }

        void DrawHeader(Rect r)
        {
            GUI.Label(r, new GUIContent($"  {serializedProperty.displayName}", Styles.callableIcon), EditorStyles.boldLabel);
            HandleDrop(r, this);
        }

        void HandleDrop(Rect rect, CallableReorderableList list)
        {
            var currentEvent = Event.current;
            var usedEvent = false;

            switch (currentEvent.type)
            {
                case EventType.DragExited:
                    if (GUI.enabled)
                        HandleUtility.Repaint();
                    break;

                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (rect.Contains(currentEvent.mousePosition) && GUI.enabled)
                    {
                        bool acceptAtLeastOne = false;
                        Object[] droppedObjects = DragAndDrop.objectReferences;
                        foreach (Object obj in droppedObjects)
                        {
                            if(obj != null && obj is GameObject)
                            {
                                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                                if (currentEvent.type == EventType.DragPerform)
                                {
                                    GenericMenu m = new GenericMenu();
                                    var callables = (obj as GameObject).GetComponents<Callable>();
                                    foreach(var c in callables)
                                    {
                                        m.AddItem(new GUIContent($"{c.Name} ({c.GetType().Name})"), false, () => {
                                            list.serializedProperty.serializedObject.Update();
                                            list.serializedProperty.arraySize++;
                                            int arrayEnd = list.serializedProperty.arraySize - 1;
                                            list.serializedProperty.GetArrayElementAtIndex(arrayEnd).objectReferenceValue = c;
                                            list.serializedProperty.serializedObject.ApplyModifiedProperties();
                                        });
                                    }
                                    m.ShowAsContext();
                                    acceptAtLeastOne = true;
                                }
                            }
                            else if (obj != null && obj is Callable)
                            {
                                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                                if (currentEvent.type == EventType.DragPerform)
                                {
                                    list.serializedProperty.arraySize++;
                                    int arrayEnd = list.serializedProperty.arraySize - 1;
                                    list.serializedProperty.GetArrayElementAtIndex(arrayEnd).objectReferenceValue = obj as Callable;
                                    acceptAtLeastOne = true;
                                }
                            }
                        }

                        if (acceptAtLeastOne)
                        {
                            GUI.changed = true;
                            DragAndDrop.AcceptDrag();
                            usedEvent = true;
                        }
                    }
                    break;
            }

            if (usedEvent)
                currentEvent.Use();

        }

        void DrawElement(Rect r, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = serializedProperty.GetArrayElementAtIndex(index);
            r.y += 2.0f;
            r.x += 4.0f;
            r.width -= 4.0f;

            EditorGUI.PropertyField(new Rect(r.x, r.y, r.width, EditorGUIUtility.singleLineHeight), element, true);
        }

        public void AddDropdown(Rect buttonRect, ReorderableList list)
        {
            var component = list.serializedProperty.serializedObject.targetObject as Component;
            var gameObject = component.gameObject;
            var propertyName = list.serializedProperty.name;

            var provider = new CallableProvider(
                gameObject,
                component,
                propertyName
                ); ;
            BrowsePopup.Show(buttonRect.position, provider);
        }

        static class Styles
        {
            public static Texture2D callableIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/net.peeweek.gameplay-ingredients/Icons/Misc/ic-callable.png");
        }
    }

    public static class CallableExtensions
    {
        public static void AddCallable(this GameObject gameObject, Component component, string propertyName, System.Type t)
        {
            var field = component.GetType().GetFields(BindingFlags.Public| BindingFlags.Instance | BindingFlags.NonPublic).Where(f => f.Name == propertyName).FirstOrDefault();
            var val = field.GetValue(component) as Callable[];

            if (t != null && typeof(Callable).IsAssignableFrom(t))
            {
                var newCmp = Undo.AddComponent(gameObject, t);
                field.SetValue(component, val.Append(newCmp as Callable));
            }
            else
                field.SetValue(component, val.Append(null));
        }

        static T[] Append<T>(this T[] array, T item)
        {
            if (array == null)
            {
                return new T[] { item };
            }
            T[] result = new T[array.Length + 1];
            array.CopyTo(result, 0);
            result[array.Length] = item;
            return result;
        }
    }
}


