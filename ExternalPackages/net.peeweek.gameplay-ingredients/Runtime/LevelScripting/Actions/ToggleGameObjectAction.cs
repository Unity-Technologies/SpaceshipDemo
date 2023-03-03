using NaughtyAttributes;
using UnityEngine;

namespace GameplayIngredients.Actions
{
    [AddComponentMenu(ComponentMenu.actionsPath + "Toggle GameObject Action")]
    [Callable("Game Objects", "Actions/ic-action-list.png")]
    public class ToggleGameObjectAction : ActionBase
    {
        [ContextMenu("Update Toggles from Current State")]
        void UpdateFromCurrentState()
        {
            for (int i = 0; i < Targets.Length; i++)
            {
                if (Targets[i].GameObject == null)
                    continue;

                Targets[i].State = Targets[i].GameObject.activeSelf ? GameObjectToggle.GameObjectToggleState.Enable : GameObjectToggle.GameObjectToggleState.Disable;

            }
        }

        [ReorderableList]
        public GameObjectToggle[] Targets;

        public override void Execute(GameObject instigator = null)
        {
            foreach(var target in Targets)
            {
                if (target.GameObject == null)
                {
                    Debug.LogWarning($"({gameObject.name}) > ToggleGameObjectAction ({this.Name}) Target is null, ignoring", this.gameObject);
                }
                else
                {
                    switch(target.State)
                    {
                        case GameObjectToggle.GameObjectToggleState.Disable:
                            target.GameObject.SetActive(false);
                            break;
                        case GameObjectToggle.GameObjectToggleState.Enable:
                            target.GameObject.SetActive(true);
                            break;
                        case GameObjectToggle.GameObjectToggleState.Toggle:
                            target.GameObject.SetActive(!target.GameObject.activeSelf);
                            break;
                    }
                }
            }
        }

        public override string GetDefaultName()
        {
            return $"Toggle GameObject(s)";
        }

        [System.Serializable]
        public struct GameObjectToggle
        {
            [System.Serializable]
            public enum GameObjectToggleState
            {
                Disable = 0,
                Enable = 1,
                Toggle = 2
            }

            public GameObject GameObject;
            public GameObjectToggleState State;
        }
    }
}
