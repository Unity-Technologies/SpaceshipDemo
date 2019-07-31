using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayIngredients
{
    public static class Messager
    {
        public delegate void Message();

        private static Dictionary<string, List<Message>> m_RegisteredMessages;

        static Messager()
        {
            m_RegisteredMessages = new Dictionary<string, List<Message>>();
        }

        public static void RegisterMessage(string messageName, Message message)
        {
            if (!m_RegisteredMessages.ContainsKey(messageName))
                m_RegisteredMessages.Add(messageName, new List<Message>());

            if (!m_RegisteredMessages[messageName].Contains(message))
                m_RegisteredMessages[messageName].Add(message);
            else
            {
                Debug.LogWarning(string.Format("Messager : {0} entry already contains reference to message.", messageName));
            }
        }

        public static void RemoveMessage(string messageName, Message message)
        {
            var currentEvent = m_RegisteredMessages[messageName];
            if(currentEvent.Contains(message))
                currentEvent.Remove(message);

            if (currentEvent == null || currentEvent.Count == 0)
                m_RegisteredMessages.Remove(messageName);
        }

        public static void Send(string eventName)
        {
            if(GameplayIngredientsSettings.currentSettings.verboseCalls)
                Debug.Log(string.Format("[MessageManager] Broadcast: {0}", eventName));

            if (m_RegisteredMessages.ContainsKey(eventName))
            {
                try
                {
                    foreach (var message in m_RegisteredMessages[eventName])
                    {
                        message.Invoke();
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(string.Format("Messager : Caught {0} while sending Message {1}", e.GetType().Name, eventName));
                    Debug.LogException(e);
                }
            }
            else
            {
                Debug.Log("[MessageManager] could not find any listeners for event : " + eventName);
            }
        }
    }
}

