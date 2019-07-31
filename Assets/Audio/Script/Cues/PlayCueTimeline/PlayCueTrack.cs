using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(0.5377358f, 0.3115655f, 0.1699448f)]
[TrackClipType(typeof(PlayCueClip))]
public class PlayCueTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<PlayCueMixerBehaviour>.Create(graph, inputCount);
    }
}

