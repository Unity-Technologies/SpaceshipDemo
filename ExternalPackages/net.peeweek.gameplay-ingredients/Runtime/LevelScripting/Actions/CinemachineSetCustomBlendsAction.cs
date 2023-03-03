using UnityEngine;
using Cinemachine;

namespace GameplayIngredients.Actions
{
    [Callable("Cinemachine", "Misc/ic-cinemachine.png")]
    [AddComponentMenu(ComponentMenu.cinemachinePath + "Cinemachine Set Custom Blends Action")]
    public class CinemachineSetCustomBlendsAction : ActionBase
    {
        public enum Action
        {
            Enable,
            Disable
        }

        [SerializeField]
        Action action;
        
        [SerializeField]
        CinemachineBlenderSettings settings;

        public override void Execute(GameObject instigator = null)
        {
            if(Manager.TryGet(out VirtualCameraManager vcm))
            {
                if (action == Action.Disable || settings == null)
                {
                    vcm.Brain.m_CustomBlends = null;
                }
                else
                {
                    vcm.Brain.m_CustomBlends = settings;
                }
            }
        }

        public override string GetDefaultName() => $"{action} CM Custom Blends : {settings}";

    }
}

