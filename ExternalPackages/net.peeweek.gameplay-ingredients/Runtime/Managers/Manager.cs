using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

namespace GameplayIngredients
{
    
    [HelpURL(Help.URL + "managers")]
    public abstract class Manager : MonoBehaviour
    {
        private static Dictionary<Type, Manager> s_Managers = new Dictionary<Type, Manager>();

        public static bool TryGet<T>(out T manager) where T: Manager
        {
            manager = null;
            if(s_Managers.ContainsKey(typeof(T)))
            {
                manager = (T)s_Managers[typeof(T)];
                return true;
            }
            else
                return false;
        }

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

        static readonly Type[] kAllManagerTypes = TypeUtility.GetConcreteTypes<Manager>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void AutoCreateAll()
        {
            s_Managers.Clear();

            var exclusionList = GameplayIngredientsSettings.currentSettings.excludedeManagers;

            if(GameplayIngredientsSettings.currentSettings.verboseCalls)
                Debug.Log("Initializing all Managers...");

            foreach(var type in kAllManagerTypes)
            {
                // Check for any Do Not Create Attribute
                var doNotCreateAttr = type.GetCustomAttribute<DoNotCreateManagerAttribute>();
                if (doNotCreateAttr != null)
                    continue;

                // Check for entries in exclusion List
                if (exclusionList != null && exclusionList.ToList().Contains(type.Name))
                {
                    Debug.LogWarning($"Manager : {type.Name} is in GameplayIngredientSettings.excludedeManagers List: ignoring Creation");
                    continue;
                }

                var prefabAttr = type.GetCustomAttribute<ManagerDefaultPrefabAttribute>(); 
                GameObject gameObject;

                if(prefabAttr != null)
                {
                    var prefab = Resources.Load<GameObject>(prefabAttr.prefab);

                    if(prefab == null) // Try loading the "Default_" prefixed version of the prefab
                    {
                        prefab = Resources.Load<GameObject>("Default_"+prefabAttr.prefab);
                    }

                    if(prefab != null)
                    {
                        gameObject = GameObject.Instantiate(prefab);
                    }
                    else
                    {
                        Debug.LogError($"Could not instantiate default prefab for {type.ToString()} : No prefab '{prefabAttr.prefab}' found in resources folders. Ignoring...");
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

                if (GameplayIngredientsSettings.currentSettings.verboseCalls)
                    Debug.Log(string.Format(" -> <{0}> OK", type.Name));
            }
        }
    }
}
