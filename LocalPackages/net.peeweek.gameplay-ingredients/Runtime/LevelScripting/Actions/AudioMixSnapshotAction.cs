using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameplayIngredients;
using GameplayIngredients.Actions;
using UnityEngine.Audio;

public class AudioMixSnapshotAction : ActionBase
{
    [NonNullCheck]
    public AudioMixer Mixer;
    [Min(0.0f)]
    public float TimeToReach = 1.0f;
    public string SnapshotName = "master";

    public override void Execute(GameObject instigator = null)
    {
        Mixer.TransitionToSnapshots(new AudioMixerSnapshot[]{ Mixer.FindSnapshot(SnapshotName)}, new float[]{ 1.0f}, TimeToReach);
    }
}
