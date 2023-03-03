using System;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace GameplayIngredients.Events
{
#if ENABLE_INPUT_SYSTEM
    [Serializable]
    public struct PlayerInputAction
    {
        public PlayerInput playerInput;
        public string path;

        public InputAction action
        {
            get
            {
                if (playerInput == null)
                    return null;
                else
                {
                    if(m_CachedPath != path || m_CachedInputAction == null)
                    {
                        string[] split = path.Split(pathSeparator);
                        if(split.Length != 2)
                        {
                            Debug.LogWarning($"Invalid Path '{path}'", playerInput);
                            return null;
                        }
                        int mapIdx = playerInput.actions.actionMaps.IndexOf(o => o.name == split[0]);
                        
                        if(mapIdx == -1) // not found
                        {
                            Debug.LogWarning($"Could not find action map '{split[0]}' in asset {playerInput.name}", playerInput);
                            return null;
                        }
                        var map = playerInput.actions.actionMaps[mapIdx];
                        int actionIdx = map.actions.IndexOf(o => o.name == split[1]);
                        if(actionIdx == -1) // not found
                        {
                            Debug.LogWarning($"Could not find action '{split[1]}' of map '{map.name}' in asset {playerInput.name}", playerInput);
                            return null;
                        }
                        m_CachedInputAction = map.actions[actionIdx];
                        m_CachedPath = path;
                    }
                    return m_CachedInputAction;
                }
            }
        }
        public const char pathSeparator = '/';
        string m_CachedPath;
        InputAction m_CachedInputAction;
    }


#endif
#if !ENABLE_INPUT_SYSTEM
    [WarnDisabledModule("New Input System")]
#endif
    [AddComponentMenu(ComponentMenu.eventsPath + "On Player Input Action Event (New Input System)")]
    public class OnPlayerInputActionEvent : EventBase
    {
#if ENABLE_INPUT_SYSTEM
        [SerializeField]
        PlayerInputAction inputAction;

        [Header("Action")]
        public Callable[] onButtonDown;

        private void OnEnable()
        {
            InputActionManager.Request(inputAction.action, InputAction_performed);
        }

        private void OnDisable()
        {
            InputActionManager.Release(inputAction.action, InputAction_performed);
        }

        private void InputAction_performed(InputAction.CallbackContext obj)
        {
            Callable.Call(onButtonDown, gameObject);
        }
#endif
    }
}


