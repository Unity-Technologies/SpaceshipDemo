using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace GameplayIngredients.Actions
{
    [AddComponentMenu(ComponentMenu.actionsPath + "Toggle UI Action")]
    [Callable("UI", "Actions/ic-action-ui.png")]
    public class ToggleUIAction : ActionBase
    {
        [ReorderableList]
        public UIToggle[] Targets;

        public override void Execute(GameObject instigator = null)
        {
            foreach (var target in Targets)
            {
                if (target.Selectable == null)
                {
                    Debug.LogWarning($"({gameObject.name}) > ToggleUIAction ({this.Name}) Target is null, ignoring", this.gameObject);
                }
                else
                {
                    switch (target.State)
                    {
                        case UIToggle.UIToggleState.Disable:
                            target.Selectable.interactable = false;
                            break;
                        case UIToggle.UIToggleState.Enable:
                            target.Selectable.interactable = true;
                            break;
                        case UIToggle.UIToggleState.Toggle:
                            target.Selectable.interactable = !target.Selectable.interactable;
                            break;
                    }
                }
            }
        }

        public override string GetDefaultName()
        {
            return $"Toggle UI";
        }

        [System.Serializable]
        public struct UIToggle
        {
            [System.Serializable]
            public enum UIToggleState
            {
                Disable = 0,
                Enable = 1,
                Toggle = 2
            }

            public Selectable Selectable;
            public UIToggleState State;
        }
    }
}
