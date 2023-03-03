using UnityEngine;
using GameplayIngredients.StateMachines;

namespace GameplayIngredients.Actions
{
    [HelpURL(Help.URL + "state-machines")]
    [AddComponentMenu(ComponentMenu.stateMachinePath + "Set State Action")]
    [Callable("State Machines", "Misc/ic-StateMachine-SetState.png")]
    public class SetStateAction : ActionBase
    {
        [NonNullCheck]
        public StateMachine StateMachine;

        public string state
        {
            get { return m_State; }
            set { m_State = value; }
        }

        [SerializeField, StateMachineState("StateMachine")]
        protected string m_State = "State";

        public override void Execute(GameObject instigator = null)
        {
            if(StateMachine != null)
                StateMachine.SetState(m_State);
        }
    }
}
