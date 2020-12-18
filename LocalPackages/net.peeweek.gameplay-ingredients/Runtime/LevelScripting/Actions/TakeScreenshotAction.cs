using UnityEngine;

namespace GameplayIngredients.Actions
{
    public class TakeScreenshotAction : ActionBase
    {
        [Tooltip("Super Sampling multiplier")]
        public int supersampleRate = 1;
        [Tooltip("Base Filename (will be numbered) ")]
        public string fileName = "screenshot";
        [Tooltip("How many digits in the sequence numbers.")]
        public int figureCount = 2;
        private int screenshotNumber = 0;

        public override void Execute(GameObject instigator = null)
        {
            ScreenCapture.CaptureScreenshot(fileName + screenshotNumber.ToString().PadLeft(figureCount, '0') + ".png", supersampleRate);
            screenshotNumber += 1;
        }
    }
}
