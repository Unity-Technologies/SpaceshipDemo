using NaughtyAttributes;

namespace GameplayIngredients.Events
{
    public class OnMessageEvent : EventBase
    {
        public string MessageName = "Message";

        [ReorderableList]
        public Callable[] OnMessageRecieved;

        void OnEnable()
        {
            Messager.RegisterMessage(MessageName, Execute);
        }

        void OnDisable()
        {
            Messager.RemoveMessage(MessageName, Execute);
        }

        void Execute()
        {
            try
            {
                Callable.Call(OnMessageRecieved, gameObject);
            }
            catch(System.Exception e)
            {
                UnityEngine.Debug.LogError(string.Format("OnMessageEvent : Exception Caught while catching message '{0}' on Object '{1}'", MessageName, gameObject.name));
                UnityEngine.Debug.LogException(e);
            }
        }


    }
}
