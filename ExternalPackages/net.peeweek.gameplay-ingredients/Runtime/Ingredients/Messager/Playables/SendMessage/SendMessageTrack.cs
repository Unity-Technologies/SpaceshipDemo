using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace GameplayIngredients.Playables
{
    [TrackColor(0.5377358f, 0.3115655f, 0.1699448f)]
    [TrackClipType(typeof(SendMessageClip))]
    public class SendMessageTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<SendMessageMixerBehaviour>.Create (graph, inputCount);
        }
    }
}
