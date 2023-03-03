using UnityEngine;
using NaughtyAttributes;

namespace GameplayIngredients.Rigs
{
    [HelpURL(Help.URL + "rigs")]
    public abstract class Rig : MonoBehaviour
    {
        public UpdateMode updateMode
        {
            get { return m_UpdateMode; }
        }

        public int rigPriority
        {
            get { return m_RigPriority; }
        }

        public enum UpdateMode
        {
            Update,
            LateUpdate,
            FixedUpdate,
        }

        public abstract UpdateMode defaultUpdateMode { get; }
        public abstract int defaultPriority { get; }
        public virtual bool canChangeUpdateMode { get { return false; } }

        protected bool CanChangeUpdateMode() { return canChangeUpdateMode; }

        [SerializeField, EnableIf("CanChangeUpdateMode")]
        private UpdateMode m_UpdateMode;
        [SerializeField]
        private int m_RigPriority = 0;


        private void Reset()
        {
            if(!canChangeUpdateMode)
            m_UpdateMode = defaultUpdateMode;
            m_RigPriority = defaultPriority;
        }

        protected virtual void OnEnable()
        {
            if (Manager.TryGet(out RigManager rigManager))
                rigManager.RegistedRig(this);
            else
                Debug.LogWarning($"{gameObject.name} : Could not register the Rig of type {GetType().Name}. Rig Manager is not present or has been excluded. Please check your Assets/GameplayIngredientsSettings asset");
        }

        protected virtual void OnDisable()
        {
            if (Manager.TryGet(out RigManager rigManager))
                rigManager.RemoveRig(this);
            else
                Debug.LogWarning($"{gameObject.name} : Could not remove the Rig of type {GetType().Name}. Rig Manager is not present or has been excluded. Please check your Assets/GameplayIngredientsSettings asset");
        }

        public abstract void UpdateRig(float deltaTime);

    }
}


