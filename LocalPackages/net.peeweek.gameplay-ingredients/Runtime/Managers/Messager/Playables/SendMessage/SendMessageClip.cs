using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace GameplayIngredients.Playables
{
    [Serializable]
    public class SendMessageClip : PlayableAsset, ITimelineClipAsset
    {
        public SendMessageBehaviour template = new SendMessageBehaviour ();

        public ClipCaps clipCaps
        {
            get { return ClipCaps.None; }
        }

        public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<SendMessageBehaviour>.Create (graph, template);
            SendMessageBehaviour clone = playable.GetBehaviour ();
            return playable;
        }
    }
}
