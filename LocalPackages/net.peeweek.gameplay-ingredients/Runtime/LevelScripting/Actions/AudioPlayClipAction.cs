using UnityEngine;
using NaughtyAttributes;

namespace GameplayIngredients.Actions
{
    public class AudioPlayClipAction : ActionBase
    {
        public AudioClip Clip;
        public AudioSource Source;
        public bool RandomizePitch = false;
        [ShowIf("RandomizePitch")]
        public Vector2 PitchRange = new Vector2(0,3); 
        public bool RandomizeVolume = false;
        [ShowIf("RandomizeVolume")]
        public Vector2 VolumeRange = new Vector2(0, 1);

        public override void Execute(GameObject instigator = null)
        {
            if (Source != null)
            {
                Source.Stop();

                if (RandomizePitch)
                    Source.pitch = Random.Range(PitchRange.x, PitchRange.y);
                if (RandomizeVolume)
                    Source.volume = Random.Range(VolumeRange.x, VolumeRange.y);
                if (Clip != null)
                    Source.clip = Clip;

                Source.Play();
            }
        }
    }
}