using NaughtyAttributes;
using UnityEngine;

namespace GameplayIngredients.Actions
{
    [AddComponentMenu(ComponentMenu.actionsPath + "Toggle Behaviour Action")]
    [Callable("Game Objects", "Actions/ic-action-list.png")]
    public class ToggleBehaviourAction : ActionBase
    {

        [ReorderableList]
        public BehaviourToggle[] Targets;

        public override void Execute(GameObject instigator = null)
        {
            foreach (var target in Targets)
            {
                if (target.Behaviour == null)
                {
                    Debug.LogWarning($"({gameObject.name}) > ToggleBehaviourAction ({this.Name}) Target is null, ignoring", this.gameObject);
                }
                else
                {
                    switch (target.State)
                    {
                        case BehaviourToggle.BehaviourToggleState.Disable:
                            target.Behaviour.enabled = false;
                            break;
                        case BehaviourToggle.BehaviourToggleState.Enable:
                            target.Behaviour.enabled = true;
                            break;
                        case BehaviourToggle.BehaviourToggleState.Toggle:
                            target.Behaviour.enabled = !target.Behaviour.enabled;
                            break;
                    }
                }
            }
        }

        public override string GetDefaultName()
        {
            return $"Toggle Behaviours";
        }

        [System.Serializable]
        public struct BehaviourToggle
        {
            [System.Serializable]
            public enum BehaviourToggleState
            {
                Disable = 0,
                Enable = 1,
                Toggle = 2
            }
            public Behaviour Behaviour;
            public BehaviourToggleState State;
        }
    }
}
