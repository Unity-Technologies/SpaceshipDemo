using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace GameplayIngredients.Playables
{
    [Serializable]
    public class SendMessageBehaviour : PlayableBehaviour
    {
        public string StartMessage;
        public int _messageID = int.MinValue;
        public GameObject Instigator;

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            base.OnBehaviourPlay(playable, info);
            if(StartMessage != "")
            {
                if (_messageID == int.MinValue)
                    _messageID = Shader.PropertyToID(StartMessage);

                if (Application.isPlaying)
                    Messager.Send(_messageID, Instigator);
                else
                    Debug.Log("[SendMessageBehaviour] Would have sent broadcast message : '" + StartMessage + "'");
            }
        }

        public override void OnGraphStart (Playable playable)
        {

        }
    }
}
