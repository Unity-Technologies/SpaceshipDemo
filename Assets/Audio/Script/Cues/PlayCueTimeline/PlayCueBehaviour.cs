using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using GameplayIngredients;

[Serializable]
public class PlayCueBehaviour : PlayableBehaviour
{
    public AudioCue Cue;

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        base.OnBehaviourPlay(playable, info);
        if(Cue != null && Application.isPlaying)
        {
            Manager.Get<AudioManager>().PlayCueAsset(Cue);
        }
    }

    public override void OnGraphStart (Playable playable)
    {

    }
}
