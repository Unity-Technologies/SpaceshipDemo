using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameplayIngredients.Actions;
using GameplayIngredients;

public class PlayCueAction : ActionBase
{
    public AudioCue Cue;

    public override void Execute(GameObject instigator)
    {
        if (Cue != null)
        {
            Manager.Get<AudioManager>().PlayCueAsset(Cue);
        }
        else
        {
            Debug.Log(string.Format("PlayCueAction : {0} gameobject has no Cue referenced", gameObject.name));
        }            
    }
}
