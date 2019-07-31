using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayCueAnimationEvent : MonoBehaviour
{
    public PlayCueAction[] playCueActions;
    
    public void PlayCue(int cueIndex)
    {
        playCueActions[cueIndex].Execute(gameObject);
    }
}
