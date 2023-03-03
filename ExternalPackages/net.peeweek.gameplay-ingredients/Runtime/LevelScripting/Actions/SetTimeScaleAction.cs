using UnityEngine;

namespace GameplayIngredients.Actions
{
    [AddComponentMenu(ComponentMenu.actionsPath + "Set Time Scale Action")]
    [Callable("Time", "Actions/ic-action-time.png")]
    public class SetTimeScaleAction : ActionBase
    {
        public float TimeScale = 1.0f;

        public override void Execute(GameObject instigator = null)
        {
            Time.timeScale = TimeScale;
        }

        public void SetTimeScale(float value)
        {
            TimeScale = value;
            Execute();
        }

        public override string GetDefaultName()
        {
            return $"Set Time Scale : {TimeScale}";
        }
    }
}
