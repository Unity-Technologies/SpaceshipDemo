using NaughtyAttributes;
using System.Linq;
using UnityEngine;
using GameplayIngredients.Actions;
using GameplayIngredients.Managers;

namespace GameplayIngredients.StateMachines
{
    [HelpURL(Help.URL + "state-machines")]
    [AddComponentMenu(ComponentMenu.stateMachinePath + "State Machine")]
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

        [Button("Create/Update SetStateAction Components")]
        private void UpdateSetStateActionComponents()
        {
            var components = this.GetComponents<SetStateAction>();
            foreach (var state in States)
            {
                if (!components.Any(o => o.state == state.StateName))
                {
                    var action = gameObject.AddComponent<SetStateAction>();
                    action.state = state.StateName;
                    action.StateMachine = this;
                }
            }

            var todelete = GetComponents<SetStateAction>().Where(a => !States.Any(s => s.StateName == a.state)).ToArray();
            for (int i = 0; i < todelete.Length; i++)
            {
                DestroyImmediate(todelete[i]);
            }

            components = this.GetComponents<SetStateAction>();
            foreach(var action in components)
            {
                action.Name = $"Set State {action.state}";
            }
        }


        private void OnEnable()
        {
            if (GameplayIngredientsSettings.currentSettings.allowUpdateCalls)
                Manager.Get<SingleUpdateManager>().Register(SingleUpdate);
        }

        private void OnDisable()
        {
            if (GameplayIngredientsSettings.currentSettings.allowUpdateCalls)
                Manager.Get<SingleUpdateManager>().Remove(SingleUpdate);
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

                // Call State enter
                Callable.Call(m_CurrentState.OnStateEnter, gameObject);
            }
            else
                Debug.LogWarning(string.Format("{0} : Trying to set unknown state {1}", gameObject.name, stateName), gameObject);
        }

        void SingleUpdate()
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

