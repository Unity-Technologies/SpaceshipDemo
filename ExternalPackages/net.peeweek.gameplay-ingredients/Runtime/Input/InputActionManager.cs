#if ENABLE_INPUT_SYSTEM
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameplayIngredients
{
    public static class InputActionManager
    {
        static Dictionary<InputAction, List<Action<InputAction.CallbackContext>>> m_Mappings = new Dictionary<InputAction, List<Action<InputAction.CallbackContext>>>();

        public static void Request(InputAction action, Action<InputAction.CallbackContext> onPerformed)
        {
            if(action == null || onPerformed == null)
            {
                Debug.LogWarning($"Cannot request InputAction, either action or callback is null");
                return;
            }

            if(!m_Mappings.ContainsKey(action))
            {
                m_Mappings.Add(action, new List<Action<InputAction.CallbackContext>>());
            }

            if (m_Mappings[action].Contains(onPerformed))
            {
                Debug.LogWarning($"InputAction already Registered : {action.name} : {onPerformed}");
                return;
            }
            else
            {
                m_Mappings[action].Add(onPerformed);

                if(!action.enabled)
                    action.Enable();

                action.performed += onPerformed;
            }
        }

        public static void Release(InputAction action, Action<InputAction.CallbackContext> onPerformed)
        {
            if (action == null || onPerformed == null)
            {
                Debug.LogWarning($"Cannot release InputAction, either action or callback is null");
                return;
            }

            if (!m_Mappings.ContainsKey(action))
            {
                Debug.LogWarning($"Tried to release action {action.name} that was not already registered");
            }
            else 
            {
                // Remove entry if present
                if(m_Mappings[action].Contains(onPerformed))
                {
                    action.performed -= onPerformed;
                    m_Mappings[action].Remove(onPerformed);
                }

                // If no more callbacks registered, remove entry and disable action.
                if(m_Mappings[action].Count == 0)
                {
                    action.Disable();
                    m_Mappings.Remove(action);
                }
                
            }
        }
    }
}
#endif