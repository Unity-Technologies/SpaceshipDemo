using NaughtyAttributes;
using UnityEngine;

namespace GameplayIngredients.Events
{
    [AdvancedHierarchyIcon("Packages/net.peeweek.gameplay-ingredients/Icons/Events/ic-event-message.png")]
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

        void Execute(GameObject instigator)
        {
            try
            {
                Callable.Call(OnMessageRecieved, instigator);
            }
            catch(System.Exception e)
            {
                UnityEngine.Debug.LogError(string.Format("OnMessageEvent : Exception Caught while catching message '{0}' on Object '{1}'", MessageName, gameObject.name));
                UnityEngine.Debug.LogException(e);
            }
        }


    }
}
