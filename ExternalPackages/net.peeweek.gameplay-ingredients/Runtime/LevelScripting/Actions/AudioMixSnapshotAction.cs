using UnityEngine;
using UnityEngine.Audio;

namespace GameplayIngredients.Actions
{
    [AddComponentMenu(ComponentMenu.actionsPath + "Audio Mix Snapshot Action")]
    [Callable("Audio", "Actions/ic-action-audio.png")]
    public class AudioMixSnapshotAction : ActionBase
    {
        [NonNullCheck]
        public AudioMixer Mixer;
        [Min(0.0f)]
        public float TimeToReach = 1.0f;
        public string SnapshotName = "master";

        public override void Execute(GameObject instigator = null)
        {
            Mixer?.TransitionToSnapshots(new AudioMixerSnapshot[] { Mixer.FindSnapshot(SnapshotName) }, new float[] { 1.0f }, TimeToReach);
        }

        public override string GetDefaultName()
        {
            return $"Set Mixer Snapshot:'{SnapshotName}' ({TimeToReach})s";
        }
    }
}

