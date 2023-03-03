using UnityEngine;

namespace GameplayIngredients.Actions
{
    [AddComponentMenu(ComponentMenu.actionsPath + "Game Manager : Send Startup Message Action")]
    [Callable("Game", "GameManager Icon.png")]
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
                    Messager.Send(GameManager.GameLevelStartMessageID);
                    break;
                case MessageType.MainMenuStart:
                    Messager.Send(GameManager.MainMenuStartMessageID);
                    break;
            }
        }

        public override string GetDefaultName()
        {
            return $"Game Manager Send : {messageType}";
        }
    }
}
