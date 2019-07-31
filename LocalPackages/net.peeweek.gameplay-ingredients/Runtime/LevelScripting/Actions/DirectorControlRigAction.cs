using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using GameplayIngredients.Rigs;

namespace GameplayIngredients.Actions
{
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
    }
}
