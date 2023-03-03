using UnityEngine;
using UnityEngine.Timeline;
using GameplayIngredients.Rigs;

namespace GameplayIngredients.Actions
{
    [AddComponentMenu(ComponentMenu.actionsPath + "Director Control Rig Action")]
    [Callable("Sequencing", "Actions/ic-action-director.png")]
    public class DirectorControlRigAction : ActionBase
    {
        [NonNullCheck]
        public DirectorControlRig directorControlRig;

        [Header("Play Mode")]
        public bool SetPlayMode = true;
        public DirectorControlRig.PlayMode PlayMode = DirectorControlRig.PlayMode.Play;

        [Header("Wrap Mode")]
        public bool SetWrapMode = false;
        public DirectorControlRig.WrapMode WrapMode = DirectorControlRig.WrapMode.Loop;

        [Header("Time")]
        public bool SetTime = false;
        public float Time = 0.0f;

        public bool SetStopTime = false;
        public float StopTime = 1.0f;

        [Header("Timeline Asset")]
        public bool SetTimeline = false;
        public TimelineAsset TimelineAsset;

        public override void Execute(GameObject instigator = null)
        {
            if (directorControlRig == null)
            {
                Debug.LogWarning("No DirectorControlRig set, ignoring Call", this.gameObject);
                return;
            }

            if (SetTime)
                directorControlRig.time = Time;

            if (SetPlayMode)
                directorControlRig.playMode = PlayMode;

            if (SetWrapMode)
                directorControlRig.wrapMode = WrapMode;

            if (SetStopTime)
                directorControlRig.stopTime = StopTime;

            if (SetTimeline)
                directorControlRig.timeline = TimelineAsset;

        }

        public override string GetDefaultName()
        {
            return $"Set DirectorControlRig:{(SetPlayMode? " "+PlayMode.ToString():"")}{(SetWrapMode ? " " + WrapMode.ToString() : "")}{(SetTime ? " Time: " + Time.ToString() : "")}{(SetStopTime ? " Stop Time: " + Time.ToString() : "")}{(SetTimeline ? " Timeline: " + TimelineAsset?.name : "")}";
        }
    }
}
