using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameplayIngredients
{
    [RequireComponent(typeof(EventSystem))]
    [RequireComponent(typeof(StandaloneInputModule))]
    [ManagerDefaultPrefab("UIEventManager")]
    public class UIEventManager : Manager
    {
        public EventSystem eventSystem { get { return m_EventSystem; } }
        [SerializeField]
        private EventSystem m_EventSystem;

        private void OnEnable()
        {
            m_EventSystem = GetComponent<EventSystem>();
        }
    }
}


