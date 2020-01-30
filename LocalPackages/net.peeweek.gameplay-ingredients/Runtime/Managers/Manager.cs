using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

namespace GameplayIngredients
{
    public abstract class Manager : MonoBehaviour
    {
        private static Dictionary<Type, Manager> s_Managers = new Dictionary<Type, Manager>();

        public static T Get<T>() where T: Manager
        {
            if(s_Managers.ContainsKey(typeof(T)))
                return (T)s_Managers[typeof(T)];
            else
            {
                Debug.LogError($"Manager of type '{typeof(T)}' could not be accessed. Check the excludedManagers list in your GameplayIngredientsSettings configuration file.");
                return null;
            }
        }

        public static bool Has<T>() where T:Manager
        {
            return(s_Managers.ContainsKey(typeof(T)));
        }

        static readonly Type[] kAllManagerTypes = GetAllManagerTypes();

        [RuntimeInitializeOnLoadMethod]
        static void AutoCreateAll()
        {
            var exclusionList = GameplayIngredientsSettings.currentSettings.excludedeManagers;

            Debug.Log("Initializing all Managers...");
            foreach(var type in kAllManagerTypes)
            {
                if(exclusionList != null && exclusionList.ToList().Contains(type.Name))
                {
                    Debug.Log($"Manager : {type.Name} is in GameplayIngredientSettings.excludedeManagers List: ignoring Creation");
                    continue;
                }
                var attrib = type.GetCustomAttribute<ManagerDefaultPrefabAttribute>(); 
                GameObject gameObject;

                if(attrib != null)
                {
                    var prefab = Resources.Load<GameObject>(attrib.prefab);

                    if(prefab == null) // Try loading the "Default_" prefixed version of the prefab
                    {
                        prefab = Resources.Load<GameObject>("Default_"+attrib.prefab);
                    }

                    if(prefab != null)
                    {
                        gameObject = GameObject.Instantiate(prefab);
                    }
                    else
                    {
                        Debug.LogError($"Could not instantiate default prefab for {type.ToString()} : No prefab '{attrib.prefab}' found in resources folders. Ignoring...");
                        continue;
                    }
                }
                else
                {
                    gameObject = new GameObject();
                    gameObject.AddComponent(type);
                }
                gameObject.name = type.Name;
                GameObject.DontDestroyOnLoad(gameObject);
                var comp = (Manager)gameObject.GetComponent(type);
                s_Managers.Add(type,comp);

                Debug.Log(string.Format(" -> <{0}> OK", type.Name));
            }
        }

        static Type[] GetAllManagerTypes()
        {
            List<Type> types = new List<Type>();
            foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies())
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

                if(assemblyTypes != null)
                {
                    foreach (Type t in assemblyTypes)
                    {
                        if (typeof(Manager).IsAssignableFrom(t) && !t.IsAbstract)
                        {
                            types.Add(t);
                        }
                    }
                }

            }
            return types.ToArray();
        }
    }
}
