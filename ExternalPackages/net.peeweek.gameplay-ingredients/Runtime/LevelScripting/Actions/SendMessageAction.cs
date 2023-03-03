using UnityEngine;

namespace GameplayIngredients.Actions
{
    [AddComponentMenu(ComponentMenu.actionsPath + "Send Message Action")]
    [HelpURL(Help.URL + "messager")]
    [Callable("Game", "Actions/ic-action-message.png")]
    public class SendMessageAction : ActionBase
    {
        public string message => MessageToSend;

        [SerializeField]
        string MessageToSend = "Message";
        int _messageID = int.MinValue;

        public override void Execute(GameObject instigator = null)
        {
            if (_messageID == int.MinValue)
                _messageID = Shader.PropertyToID(MessageToSend);

            Messager.Send(_messageID, instigator);
        }

        public void SetMessageName(string messageName)
        {
            MessageToSend = messageName;
            _messageID = Shader.PropertyToID(MessageToSend);
        }

        public override string GetDefaultName()
        {
            return $"Send Message : '{MessageToSend}'";
        }
    }
}


