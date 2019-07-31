using UnityEngine;
using UnityEngine.UI;

namespace GameplayIngredients.Actions
{
    public class CycleWindowModeAction : ActionBase
    {
        public Text ButtonLabel;

        string getText(bool mode)
        {
            return string.Format("Window Mode: {0}", mode ? "Full Screen" : "Windowed");
        }

        private void OnEnable()
        {
            if (ButtonLabel != null)
                ButtonLabel.text = getText(Screen.fullScreen);
        }

        private void Update()
        {
            // We update this label each frame because setting Screen.fullScreen only update the fullScreen value at the next frame
            if (ButtonLabel != null)
                ButtonLabel.text = getText(Screen.fullScreen);
        }

        public override void Execute(GameObject instigator = null)
        {
            Screen.fullScreen = !Screen.fullScreen;
        }
    }
}
