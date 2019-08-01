using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayIngredients.Actions
{
    public class GameManagerSendStartupMessageAction : ActionBase
    {
        public enum MessageType
        {
            MainMenuStart,
            GameLevelStart,
        }

        public MessageType messageType;

        public override void Execute(GameObject instigator = null)
        {
            switch(messageType)
            {
                case MessageType.GameLevelStart:
                    Messager.Send(GameManager.GameLevelStartMessage);
                    break;
                case MessageType.MainMenuStart:
                    Messager.Send(GameManager.MainMenuStartMessage);
                    break;
            }
        }
    }
}
