using UnityEngine;

namespace GameplayIngredients.Actions
{
    public class SendMessageAction : ActionBase
    {
        public string MessageToSend = "Message";

        public override void Execute(GameObject instigator = null)
        {
            Messager.Send(MessageToSend);
        }
    }
}


