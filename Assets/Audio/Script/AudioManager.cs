using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameplayIngredients;
using NaughtyAttributes;

[ManagerDefaultPrefab("AudioManager")]
public class AudioManager : Manager
{
    [ReorderableList, NonNullCheck]
    public AudioSource[] CueSources;

    public bool DisableCues = false;

    public void PlayCueAsset(AudioCue cue)
    {
        if(!DisableCues && CueSources.Length > cue.CueSourceIndex && CueSources[cue.CueSourceIndex] != null)
        {
            CueSources[cue.CueSourceIndex].Stop();
            CueSources[cue.CueSourceIndex].PlayOneShot(cue.Clip);

            Get<SubtitleManager>().PlaySubtitles(cue.Subtitles);
        }
    }

    public void PlayAudioSource(AudioSource source)
    {
        if (source == null) return;

        source.Stop();
        if (source.clip != null)
        {
            source.Play();
        }
    }

    public void PlayAudioSourceWithClip(AudioSource source, AudioClip clip, bool loop)
    {
        if (source == null || clip == null) return;

        source.Stop();
        if (loop)
        {
            source.loop = true;
            source.clip = clip;
            source.Play();
        }
        else
        {
            source.PlayOneShot(clip);
        } 
    }

    public void Pause()
    {
        foreach(var CueSource in CueSources)
        {
            if(CueSource != null)
                CueSource.Pause();
        }

    }

    public void UnPause()
    {
        foreach (var CueSource in CueSources)
        {
            if (CueSource != null)
                CueSource.UnPause();
        }
    }
}
