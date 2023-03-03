using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace GameplayIngredients
{

    [Serializable]
    public struct ReflectedMember
    {
        public UnityEngine.Object targetObject { get => m_TargetObject; }
        public MemberInfo member { get { UpdateMember(); return m_Member; } }

        [SerializeField]
        UnityEngine.Object m_TargetObject;

        [SerializeField]
        string m_MemberName;

        [SerializeField]
        MemberInfo m_Member;

        public void SetMember(UnityEngine.Object obj, string memberName)
        {
            m_TargetObject = obj;
            m_MemberName = memberName;
            UpdateMember();
        }

        public void SetValue(object value)
        {
            if (m_TargetObject != null)
            {
                if (m_Member == null)
                    UpdateMember();

                if (m_Member.MemberType == MemberTypes.Field)
                    (m_Member as FieldInfo).SetValue(m_TargetObject, value);
                else if (m_Member.MemberType == MemberTypes.Property)
                    (m_Member as PropertyInfo).SetValue(m_TargetObject, value);
                else
                    throw new Exception($"Could not Set value of member {m_Member.Name} which is a {m_Member.MemberType}");
            }
            else
                throw new Exception($"Could not Find member '{m_MemberName}' of target object {m_TargetObject}");
        }

        public T GetValue<T>()
        {
            return (T)GetValue();
        }

        public object GetValue()
        {
            if (m_TargetObject != null)
            {
                if (m_Member == null)
                    UpdateMember();

                if (m_Member.MemberType == MemberTypes.Field)
                    return (m_Member as FieldInfo).GetValue(m_TargetObject);
                else if (m_Member.MemberType == MemberTypes.Property)
                    return (m_Member as PropertyInfo).GetValue(m_TargetObject);
            }
            return null;
        }

        public void UpdateMember()
        {
            if (m_TargetObject == null)
                return;

            m_Member = m_TargetObject.GetType().GetMember(m_MemberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.SetField).First();

            if (m_Member == null)
                Debug.LogWarning($"Could not find member of name {m_MemberName} on type {m_TargetObject.GetType().Name}");
        }
    }
}