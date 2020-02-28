using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameplayIngredients;
using NaughtyAttributes;

[ManagerDefaultPrefab("AudioManager")]
public class AudioManager : Manager
{
    [Header("Cues")]
    [ReorderableList, NonNullCheck]
    public AudioSource[] CueSources;

    public bool DisableCues = false;

    [Header("Pause Management")]
    public string PauseMessage = "PAUSE";
    public string UnPauseMessage = "UNPAUSE";

    List<AudioSource> PauseManagedSources;

    private void Awake()
    {
        PauseManagedSources = new List<AudioSource>();
    }

    void OnEnable()
    {
        Messager.RegisterMessage(PauseMessage, Pause);
        Messager.RegisterMessage(UnPauseMessage, UnPause);
    }

    private void OnDisable()
    {
        Messager.RemoveMessage(PauseMessage, Pause);
        Messager.RemoveMessage(UnPauseMessage, UnPause);
    }

    public void RegisterPausedManagedSource(AudioSource source)
    {
        if (!PauseManagedSources.Contains(source))
            PauseManagedSources.Add(source);
    }

    public void RemovePausedManagedSource(AudioSource source)
    {
        if (PauseManagedSources.Contains(source))
            PauseManagedSources.Remove(source);
    }

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

    public void Pause(GameObject instigator = null)
    {
        foreach(var CueSource in CueSources)
        {
            if(CueSource != null)
                CueSource.Pause();
        }

        foreach(var source in PauseManagedSources)
        {
            if (source != null)
                source.Pause();
        }
    }

    public void UnPause(GameObject instigator = null)
    {
        foreach (var CueSource in CueSources)
        {
            if (CueSource != null)
                CueSource.UnPause();
        }

        foreach (var source in PauseManagedSources)
        {
            if (source != null)
                source.UnPause();
        }
    }
}
