using GameplayIngredients;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(PlayableDirector))]
public class PlayableDirectorPausable : MonoBehaviour
{

    public string PauseMessage = "PAUSE";
    public string UnPauseMessage = "UNPAUSE";

    PlayableDirector director;
    bool wasPlaying;

    private void OnEnable()
    {
        Messager.RegisterMessage(PauseMessage, OnPause);
        Messager.RegisterMessage(UnPauseMessage, OnUnPause);
        director = GetComponent<PlayableDirector>();
    }
    private void OnDisable()
    {
        Messager.RemoveMessage(PauseMessage, OnPause);
        Messager.RemoveMessage(UnPauseMessage, OnUnPause);
    }

    void OnPause(GameObject instigator = null)
    {
        if (director.state == PlayState.Playing)
        {
            director.Pause();
            wasPlaying = true;
        }
        else
        {
            wasPlaying = false;
        }
    }
    void OnUnPause(GameObject instigator = null)
    {
        if(wasPlaying)
        {
            director.Play();
            wasPlaying = false;
        }
    }
}
