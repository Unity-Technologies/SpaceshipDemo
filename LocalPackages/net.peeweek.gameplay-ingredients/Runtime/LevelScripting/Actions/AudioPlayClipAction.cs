using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

namespace GameplayIngredients.Actions
{
    public class AudioPlayClipAction : ActionBase
    {
        public AudioClip Clip;
        public AudioSource Source;

        public override void Execute(GameObject instigator = null)
        {
            if (Source != null)
            {
                Source.Stop();

                if (Clip != null)
                    Source.clip = Clip;

                Source.Play();
            }
        }

    }
}
