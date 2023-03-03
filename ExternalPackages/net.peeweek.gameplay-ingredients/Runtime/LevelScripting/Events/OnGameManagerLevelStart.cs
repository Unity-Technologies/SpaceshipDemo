using UnityEngine;

namespace GameplayIngredients.Events
{
    [AddComponentMenu(ComponentMenu.eventsPath + "On Game Manager Level Start")]
    public class OnGameManagerLevelStart : EventBase
    {
        public enum GameManagerLevelType
        {
            MainMenu,
            GameLevel
        }

        public GameManagerLevelType levelType { get { return m_LevelType; } }

        [SerializeField]
        protected GameManagerLevelType m_LevelType = GameManagerLevelType.GameLevel;

        int m_MessageID;

        public Callable[] OnMessageRecieved;

        void OnEnable()
        {
            m_MessageID = GetMessageID(m_LevelType);
            Messager.RegisterMessage(m_MessageID, Execute);
        }

        void OnDisable()
        {
            Messager.RemoveMessage(m_MessageID, Execute);
        }

        static int GetMessageID(GameManagerLevelType type)
        {
            switch(type)
            {
                case GameManagerLevelType.MainMenu:  return GameManager.MainMenuStartMessageID;
                default:
                case GameManagerLevelType.GameLevel: return GameManager.GameLevelStartMessageID;
            }
        }

        void Execute(GameObject instigator = null)
        {
            try
            {
                Callable.Call(OnMessageRecieved, instigator);
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError(string.Format("OnMessageEvent : Exception Caught while catching message '{0}' on Object '{1}'", m_MessageID, gameObject.name));
                UnityEngine.Debug.LogException(e);
            }
        }


    }
}


