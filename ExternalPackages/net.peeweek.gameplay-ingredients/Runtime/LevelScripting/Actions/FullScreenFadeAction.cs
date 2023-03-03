using UnityEngine;
using NaughtyAttributes;

namespace GameplayIngredients.Actions
{
    [AddComponentMenu(ComponentMenu.actionsPath + "Full Screen Fade Action")]
    [Callable("Screen", "Actions/ic-action-screen.png")]
    public class FullScreenFadeAction : ActionBase
    {
        public FullScreenFadeManager.FadeMode Fading = FullScreenFadeManager.FadeMode.ToBlack;
        public float Duration = 2.0f;
        public FullScreenFadeManager.FadeTimingMode fadeTimingMode = FullScreenFadeManager.FadeTimingMode.UnscaledGameTime;

        public Callable[] OnComplete;

        public override void Execute(GameObject instigator = null)
        {
            Manager.Get<FullScreenFadeManager>().Fade(Duration, Fading, fadeTimingMode, OnComplete, instigator);
        }

        public override string GetDefaultName()
        {
            return $"Full Screen Fade {Fading} {Duration}s ({fadeTimingMode})";
        }
    }

}
