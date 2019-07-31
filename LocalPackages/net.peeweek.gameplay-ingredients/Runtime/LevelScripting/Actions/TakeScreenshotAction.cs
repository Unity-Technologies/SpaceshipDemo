using UnityEngine;

namespace GameplayIngredients.Actions
{
    public class TakeScreenshotAction : ActionBase
    {
        public int supersampleRate = 1;
        public string fileName = "screenshot";
        public int figureCount = 2;
        private int screenshotNumber = 0;

        public override void Execute(GameObject instigator = null)
        {
            ScreenCapture.CaptureScreenshot(name + screenshotNumber.ToString().PadLeft(figureCount, '0') + ".png", supersampleRate);
            screenshotNumber += 1;
        }
    }
}
