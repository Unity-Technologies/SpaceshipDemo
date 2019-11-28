using NaughtyAttributes;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayIngredients.StateMachines
{
    public class StateMachine : MonoBehaviour
    {
        [StateMachineState]
        public string DefaultState;

        [ReorderableList, NonNullCheck]
        public State[] States;

        public State CurrentState { get { return m_CurrentState; } }

        State m_CurrentState;
        
        [ContextMenu("Reset State Objects")]
        private void UpdateFromState()
        {
            foreach(var state in States)
            {
                state.gameObject.SetActive(state == States.FirstOrDefault(o => o.StateName == DefaultState));
            }
        }

        void Start()
        {
            foreach (var state in States)
            {
                if(state.gameObject.activeSelf)
                    state.gameObject.SetActive(false);
            }

            SetState(DefaultState);
        }

        public void SetState(string stateName)
        {
            State newState = States.FirstOrDefault(o => o.StateName == stateName);

            if(newState != null)
            {
                if (m_CurrentState != null)
                {
                    // Call Exit Actions
                    Callable.Call(m_CurrentState.OnStateExit, gameObject);
                    // Then finally disable old state
                    m_CurrentState.gameObject.SetActive(false);
                }

                // Switch Active new state
                newState.gameObject.SetActive(true);

                // Then Set new current state
                m_CurrentState = newState;

                // Finally, call State enter
                Callable.Call(m_CurrentState.OnStateEnter, gameObject);
            }
            else
                Debug.LogWarning(string.Format("{0} : Trying to set unknown state {1}", gameObject.name, stateName), gameObject);
        }

        void Update()
        {
            if (GameplayIngredientsSettings.currentSettings.allowUpdateCalls 
                && m_CurrentState != null 
                && m_CurrentState.OnStateUpdate != null 
                && m_CurrentState.OnStateUpdate.Length > 0)
            {
                Callable.Call(m_CurrentState.OnStateUpdate, this.gameObject);
            }
        }

    }
}

