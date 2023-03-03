using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace GameplayIngredients.Editor
{
    public abstract class Check
    {
        public abstract string name { get; }

        public abstract bool defaultEnabled { get; }
        public abstract IEnumerable<CheckResult> GetResults(SceneObjects sceneObjects);
        public abstract void Resolve(CheckResult result);
        public abstract string[] ResolutionActions { get; }
        public abstract int defaultResolutionActionIndex { get; }

        #region STATIC

        public static List<Check> allChecks { 
            get { 
                if (s_Checks == null) 
                    Initialize(); 
                return s_Checks.Values.ToList(); 
            } 
        }
        static Dictionary<Type, Check> s_Checks;

        [InitializeOnLoadMethod]
        static void Initialize()
        {
            s_Checks = new Dictionary<Type, Check>();
            var types = GetAllTypes();
            foreach(var type in types)
            {
                Check check = (Check)Activator.CreateInstance(type);
                s_Checks.Add(type, check);
            }
        }

        public static T Get<T>() where T : Check
        {
            if (s_Checks.ContainsKey(typeof(T)))
                return (T)s_Checks[typeof(T)];
            else
            {
                Debug.LogError($"Check of type '{typeof(T)}' could not be accessed.");
                return null;
            }
        }

        public static bool Has<T>() where T : Check
        {
            return (s_Checks.ContainsKey(typeof(T)));
        }

        static Type[] GetAllTypes()
        {
            List<Type> types = new List<Type>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] assemblyTypes = null;

                try
                {
                    assemblyTypes = assembly.GetTypes();
                }
                catch
                {
                    Debug.LogError($"Could not load types from assembly : {assembly.FullName}");
                }

                if (assemblyTypes != null)
                {
                    foreach (Type t in assemblyTypes)
                    {
                        if (typeof(Check).IsAssignableFrom(t) && !t.IsAbstract)
                        {
                            types.Add(t);
                        }
                    }
                }

            }
            return types.ToArray();
        }
        #endregion
    }

}
