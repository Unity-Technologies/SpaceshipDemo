using UnityEngine;
using UnityEngine.Playables;

namespace GameplayIngredients.Actions
{
    [AddComponentMenu(ComponentMenu.actionsPath + "Timeline Control Action")]
    [Callable("Sequencing", "Actions/ic-action-timeline.png")]
    public class TimelineControlAction : ActionBase
    {
        public enum TimelineControlMode
        {
            Play,
            Stop,
            Pause,
            Loop,
            Hold
        }

        public PlayableDirector director;
        public TimelineControlMode mode = TimelineControlMode.Play;

        public override void Execute(GameObject instigator = null)
        {
            switch(mode)
            {
                case TimelineControlMode.Play: director.Play(); break;
                case TimelineControlMode.Stop: director.Stop();  break;
                case TimelineControlMode.Pause: director.Pause();  break;
                case TimelineControlMode.Loop: director.extrapolationMode = DirectorWrapMode.Loop; break;
                case TimelineControlMode.Hold: director.extrapolationMode = DirectorWrapMode.Hold; break;
            }
        }

        public override string GetDefaultName()
        {
            return $"Timeline {mode} : '{director?.gameObject.name}'";
        }
    }
}


