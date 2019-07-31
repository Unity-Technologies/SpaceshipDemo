using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class PlayCueClip : PlayableAsset, ITimelineClipAsset
{
    public PlayCueBehaviour template = new PlayCueBehaviour();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<PlayCueBehaviour>.Create(graph, template);
        PlayCueBehaviour clone = playable.GetBehaviour();
        return playable;
    }
}
