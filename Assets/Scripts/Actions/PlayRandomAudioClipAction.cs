using GameplayIngredients;
using GameplayIngredients.Actions;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayRandomAudioClipAction : ActionBase
{
    [NonNullCheck]
    public AudioSource AudioSource;

    [ReorderableList, NonNullCheck]
    public AudioClip[] Clips;

    public override void Execute(GameObject instigator = null)
    {
        AudioSource.clip = Clips[Random.Range(0, Clips.Length)];
        AudioSource.Play();
    }
}
