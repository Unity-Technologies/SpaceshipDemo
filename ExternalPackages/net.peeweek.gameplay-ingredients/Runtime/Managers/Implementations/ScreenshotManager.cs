using GameplayIngredients.Managers;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
namespace GameplayIngredients
{
#if !MODULE_SCREENCAPTURE
    [WarnDisabledModule("Screen Capture")]
#endif
    [AddComponentMenu(ComponentMenu.managersPath + "Screenshot Manager")]
    [ManagerDefaultPrefab("ScreenshotManager")]
    public class ScreenshotManager : Manager
    {
        [Header("Capture")]
#if ENABLE_LEGACY_INPUT_MANAGER
        public KeyCode ScreenshotKeyCode = KeyCode.F11;
#endif
#if ENABLE_INPUT_SYSTEM
        public Key ScreenshotKey = Key.F11;
#endif

        [Range(1, 5)]
        public int SuperSize = 1;

        [Header("File name")]
        public string Prefix = "Screenshot";

        [Header("Actions")]
        public Callable[] OnBeforeScreenshot;
        public Callable[] OnAfterScreenshot;

        public void Update()
        {
#if ENABLE_LEGACY_INPUT_MANAGER
            if (Input.GetKeyDown(ScreenshotKeyCode))
#elif ENABLE_INPUT_SYSTEM
            if (InputSystemUtility.GetButton(ScreenshotKey).wasPressedThisFrame)
#else
            if(false)
#endif
            {
#if MODULE_SCREENCAPTURE
                var now = System.DateTime.Now;
                Callable.Call(OnBeforeScreenshot);
                string path = $"{Application.dataPath}/../{Prefix}-{now.Year}{now.Month}{now.Day}-{now.Hour}{now.Minute}{now.Second}{now.Millisecond}.png";
                Debug.Log($"Capturing Screenshot (Supersampled to {SuperSize}x) to the file : {path}");
                ScreenCapture.CaptureScreenshot(path, SuperSize);
                Callable.Call(OnAfterScreenshot);
#else
                Debug.Log("Screenshot Manager Cannot Take Screenshot : Unity Module Screen Capture is Disabled.");
#endif
            }
        }
    }
}
