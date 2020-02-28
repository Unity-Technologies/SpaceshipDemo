using GameplayIngredients;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSourcePausable : MonoBehaviour
{
    private void OnEnable()
    {
        Manager.Get<AudioManager>().RegisterPausedManagedSource(GetComponent<AudioSource>());
    }

    private void OnDisable()
    {
        Manager.Get<AudioManager>().RemovePausedManagedSource(GetComponent<AudioSource>());
    }
}
