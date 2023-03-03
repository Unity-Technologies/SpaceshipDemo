using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayIngredients
{
    public static class Messager
    {
        public delegate void Message(GameObject instigator = null);

        private static Dictionary<int, List<Message>> m_RegisteredMessages;
        private static Dictionary<int, string> m_MessageNames;

        static Messager()
        {
            m_RegisteredMessages = new Dictionary<int, List<Message>>();
            m_MessageNames = new Dictionary<int, string>();
        }

        public static void RegisterMessage(string messageName, Message message)
        {
            int messageID = Shader.PropertyToID(messageName);
    
            if(!m_MessageNames.ContainsKey(messageID))
                m_MessageNames[messageID] = messageName;
            
            RegisterMessage(messageID, message);
        }

        public static void RegisterMessage(int messageID, Message message)
        {
            if (!m_RegisteredMessages.ContainsKey(messageID))
                m_RegisteredMessages.Add(messageID, new List<Message>());

            if (!m_RegisteredMessages[messageID].Contains(message))
                m_RegisteredMessages[messageID].Add(message);
            else
            {
                Debug.LogError(string.Format("Messager : Entry already contains reference to message."));
            }
        }

        public static void RemoveMessage(string messageName, Message message)
        {
            int messageID = Shader.PropertyToID(messageName);
            RemoveMessage(messageID, message);
        }

        public static void RemoveMessage(int messageID, Message message)
        {

            var currentEvent = m_RegisteredMessages[messageID];
            if(currentEvent.Contains(message))
                currentEvent.Remove(message);

            if (currentEvent == null || currentEvent.Count == 0)
                m_RegisteredMessages.Remove(messageID);
        }

        public static void Send(string messageName, GameObject instigator = null)
        {
            int messageID = Shader.PropertyToID(messageName);
            Send(messageID, instigator);
        }

        public static void Send(int messageID, GameObject instigator = null)
        {
            if (GameplayIngredientsSettings.currentSettings.verboseCalls)
                if(m_MessageNames.ContainsKey(messageID))
                    Debug.Log(string.Format("[MessageManager] Broadcast: {0}", m_MessageNames[messageID]));
                else
                    Debug.Log(string.Format("[MessageManager] Broadcast: ID# {0} (probably not registered)", messageID));


            if (m_RegisteredMessages.ContainsKey(messageID))
            {
                try
                {
                    // Get a copy of registered messages to iterate on. This prevents issues while deregistering message recievers while iterating.
                    var messages = m_RegisteredMessages[messageID].ToArray();
                    foreach (var message in messages)
                    {
                        if(message != null)
                            message.Invoke(instigator);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(string.Format("Messager : Caught {0} while sending Message {1}", e.GetType().Name, m_MessageNames[messageID]));
                    Debug.LogException(e);
                }
            }
            else
            {
                if(GameplayIngredientsSettings.currentSettings.verboseCalls)
                    Debug.Log("[MessageManager] could not find any listeners for message ID# " + messageID);
            }
        }
    }
}

