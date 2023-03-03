using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GameplayIngredients.Rigs;
using System.Reflection;
using System;
using System.Linq;

namespace GameplayIngredients.Editor
{
    [CustomPropertyDrawer(typeof(AnimationHandler), true)]
    public class AnimationHandlerPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true) + 8;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var targetObj = property.serializedObject.targetObject;
            MemberInfo selfInfo = targetObj.GetType().GetMember(property.name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.GetField).First();

            if (typeToAdd != null)
            {
                if (selfInfo.MemberType == MemberTypes.Property)
                {
                    Undo.RecordObject(targetObj, "Change Animation Handler");
                    (selfInfo as PropertyInfo).SetValue(targetObj, Activator.CreateInstance(typeToAdd));
                    typeToAdd = null;
                }
                else if (selfInfo.MemberType == MemberTypes.Field)
                {
                    Undo.RecordObject(targetObj, "Change Animation Handler");
                    (selfInfo as FieldInfo).SetValue(targetObj, Activator.CreateInstance(typeToAdd));
                    typeToAdd = null;
                }
                else
                    throw new Exception($"Could not find field/property of name {property.name} on object {targetObj.name}");
            }
            else
            {
                GUI.Box(position, GUIContent.none, EditorStyles.helpBox);

                position = new RectOffset(4, 4, 4, 4).Remove(position);

                using (new EditorGUI.IndentLevelScope(1))
                {
                    Rect r = position;
                    r.xMin += EditorGUIUtility.labelWidth;
                    r.height = EditorGUIUtility.singleLineHeight;

                    var p = property.serializedObject.targetObject.GetType().GetMember(property.name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.GetField).First();
                    var attr = p.GetCustomAttribute<HandlerTypeAttribute>();
                    if (attr != null)
                    {
                        var type = attr.type;
                        Type curType = typeof(object);
                        if (selfInfo.MemberType == MemberTypes.Property)
                            curType = (selfInfo as PropertyInfo).GetValue(targetObj).GetType();
                        else if (selfInfo.MemberType == MemberTypes.Field)
                            curType = (selfInfo as FieldInfo).GetValue(targetObj).GetType();

                        string name = curType.Name;
                        var typeAttr = curType.GetCustomAttribute<AnimationHandlerAttribute>();
                        if (typeAttr != null)
                            name = typeAttr.menuPath;

                        if (EditorGUI.DropdownButton(r, new GUIContent(name), FocusType.Passive))
                        {
                            PromptMenuFor(r, type, curType);
                        }

                    }
                    else
                    {
                        using (new EditorGUI.DisabledGroupScope(true))
                            EditorGUI.DropdownButton(r, new GUIContent("(Property does not implement [HandlerType] attribute)"), FocusType.Passive);
                    }


                    EditorGUI.PropertyField(position, property, true);
                }
            }
        }

        Type typeToAdd = null;

        void PromptMenuFor(Rect position, Type filterType, Type currentType)
        {
            var allHandlers = TypeUtility.GetConcreteTypes<AnimationHandler>();
            GenericMenu m = new GenericMenu();
            foreach(var handlerType in allHandlers)
            {
                var attr = handlerType.GetCustomAttribute<AnimationHandlerAttribute>();
                if (attr != null && attr.type == filterType)
                {
                    m.AddItem(new GUIContent(attr.menuPath), handlerType == currentType, () => { typeToAdd = handlerType; });
                }
            }
            m.DropDown(position);
        }
    }

}
