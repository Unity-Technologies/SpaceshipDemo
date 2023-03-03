using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;

namespace GameplayIngredients.Editor
{
    [CustomPropertyDrawer(typeof(ReflectedMember))]
    public class ReflectedMemberyPropertyDrawer : PropertyDrawer
    {
        Dictionary<(Object, System.Type), string[]> cachedMemberNames;

        GenericMenu nullMenu;

        void CacheMembersForObject(Object obj, System.Type filterType)
        {
            if (obj == null)
                return;

            if (cachedMemberNames == null)
                cachedMemberNames = new Dictionary<(Object, System.Type), string[]>();

            if (!cachedMemberNames.ContainsKey((obj, filterType)))
            {
                List<string> names = new List<string>();
                foreach (var p in obj.GetType().GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.SetField))
                {
                    if (p.MemberType == MemberTypes.Field && filterType.IsAssignableFrom((p as FieldInfo).FieldType))
                        names.Add(p.Name);
                    else if (p.MemberType == MemberTypes.Property && filterType.IsAssignableFrom((p as PropertyInfo).PropertyType))
                        names.Add(p.Name);
                }
                cachedMemberNames.Add((obj, filterType), names.ToArray());
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight + 8;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.Box(position, GUIContent.none, EditorStyles.helpBox);

            position = new RectOffset(4, 4, 4, 4).Remove(position);

            SerializedProperty obj = property.FindPropertyRelative("m_TargetObject");
            SerializedProperty propName = property.FindPropertyRelative("m_MemberName");

            float width = EditorGUIUtility.currentViewWidth;

            var filterType = typeof(object);
            var p = property.serializedObject.targetObject.GetType().GetMember(property.name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.GetField | BindingFlags.FlattenHierarchy).First();
            var attr = p.GetCustomAttribute<ReflectedMemberAttribute>();

            if (attr != null) // If using a ReflectedMemberAttribute for filtering type
            {
                var typeMember = property.serializedObject.targetObject.GetType().GetMember(attr.typeMemberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.GetField).First();
                if (typeMember.DeclaringType is System.Type)
                {

                    if (typeMember is PropertyInfo)
                        filterType = (typeMember as PropertyInfo).GetValue(property.serializedObject.targetObject) as System.Type;
                    else if (typeMember is FieldInfo)
                        filterType = (typeMember as FieldInfo).GetValue(property.serializedObject.targetObject) as System.Type;
                }
            }

            Rect objRect = position;
            Rect btnRect = position;
            Rect propRect = position;

            objRect.xMax = (width / 3) - 4;
            btnRect.xMin = (width / 3);
            btnRect.xMax = (2 * width / 3);
            propRect.xMin = (2 * width / 3);

            EditorGUI.ObjectField(objRect, obj, GUIContent.none);

            var tgt = obj.objectReferenceValue;

            if (tgt == null || !(tgt is GameObject || tgt is Component))
            {
                if (EditorGUI.DropdownButton(btnRect, GUIContent.none, FocusType.Passive))
                {
                    if (nullMenu == null)
                    {
                        nullMenu = new GenericMenu();
                        nullMenu.AddDisabledItem(new GUIContent("No Game Object or Component Selected"), false);
                    }

                    nullMenu.DropDown(btnRect);
                }
            }
            else
            {
                Component[] comps = null;
                int sel = -1;
                if (tgt is GameObject)
                {
                    sel = 0;
                    comps = (tgt as GameObject).GetComponents(typeof(Component));
                }
                else if (tgt is Component)
                {

                    comps = (tgt as Component).GetComponents(typeof(Component));
                }

                List<string> names = new List<string>();
                names.Add("Game Object");

                int i = 1;
                foreach (var comp in comps)
                {
                    if (comp is Callable)
                        names.Add($"{comp.GetType().Name} ({(comp as Callable).Name})");
                    else
                        names.Add(comp.GetType().Name);

                    if (tgt == comp)
                        sel = i;
                    i++;
                }

                EditorGUI.BeginChangeCheck();

                int newSel = EditorGUI.Popup(btnRect, sel, names.ToArray());

                if (EditorGUI.EndChangeCheck())
                {
                    if (newSel == 0)
                    {
                        if (tgt is GameObject)
                            obj.objectReferenceValue = (tgt as GameObject);
                        else if (tgt is Component)
                            obj.objectReferenceValue = (tgt as Component).gameObject;
                    }
                    else
                        obj.objectReferenceValue = comps[newSel - 1];

                    propName.stringValue = string.Empty;
                }

            }

            if (obj.objectReferenceValue != null)
            {
                CacheMembersForObject(obj.objectReferenceValue, filterType);
                int propIdx = -1;
                int i = 0;
                if (!string.IsNullOrEmpty(propName.stringValue))
                    foreach (var name in cachedMemberNames[(obj.objectReferenceValue, filterType)])
                    {
                        if (propName.stringValue == name)
                            propIdx = i;
                        i++;
                    }

                EditorGUI.BeginChangeCheck();

                int newPropIdx = EditorGUI.Popup(propRect, propIdx, cachedMemberNames[(obj.objectReferenceValue, filterType)]);
                if (EditorGUI.EndChangeCheck())
                {
                    propName.stringValue = cachedMemberNames[(obj.objectReferenceValue, filterType)][newPropIdx];
                }
            }
            else
            {
                using (new EditorGUI.DisabledGroupScope(true))
                {
                    EditorGUI.DropdownButton(propRect, new GUIContent("(Please Select an object first)"), FocusType.Passive);
                }
            }

            // Validate data
            if (obj.objectReferenceValue != null)
            {
                if (!(obj.objectReferenceValue is GameObject || obj.objectReferenceValue is Component)) // Invalid Component Type
                {
                    obj.objectReferenceValue = null;
                    propName.stringValue = string.Empty;
                }
            }
        }
    }
}
