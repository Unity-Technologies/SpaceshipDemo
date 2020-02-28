using NaughtyAttributes;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayIngredients.StateMachines
{
    [AdvancedHierarchyIcon("Packages/net.peeweek.gameplay-ingredients/Icons/Misc/ic-StateMachine.png")]
    public class StateMachine : MonoBehaviour
    {
        [StateMachineState]
        public string DefaultState;

        [ReorderableList, NonNullCheck]
        public State[] States = new State[0];

        public State CurrentState { get { return m_CurrentState; } }

        State m_CurrentState;

        [Button("Create New State")]
        private void AddNewState()
        {
            var newState = new GameObject($"State {States.Length}");
            var state = newState.AddComponent<State>();
            newState.transform.parent = transform;
            newState.transform.localPosition = Vector3.zero;
            newState.transform.localRotation = Quaternion.identity;
            newState.transform.localScale = Vector3.one;
            States = States.Concat(new State[] { state }).ToArray();

            if (m_CurrentState == null)
                m_CurrentState = state;
        }

        [Button("Reset State Objects")]
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

