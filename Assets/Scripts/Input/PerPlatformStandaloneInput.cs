using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(StandaloneInputModule))]
public class PerPlatformStandaloneInput : MonoBehaviour
{
    [Header("Windows")]
    public string HorizontalAxisWindows = "UI Horizontal Windows";
    public string VerticalAxisWindows = "UI Vertical Windows";
    [Header("Linux")]
    public string HorizontalAxisLinux = "UI Horizontal Linux";
    public string VerticalAxisLinux = "UI Vertical Linux";
    [Header("MacOS")]
    public string HorizontalAxisMacOS = "UI Horizontal MacOS";
    public string VerticalAxisMacOS = "UI Vertical MacOS";

    private void OnEnable()
    {
        StandaloneInputModule sip = GetComponent<StandaloneInputModule>();

        switch (Application.platform)
        {
            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.OSXPlayer:
                sip.horizontalAxis = HorizontalAxisMacOS;
                sip.verticalAxis = VerticalAxisMacOS;
                break;
            default:
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.WindowsPlayer:
                sip.horizontalAxis = HorizontalAxisWindows;
                sip.verticalAxis = VerticalAxisWindows;
                break;
           case RuntimePlatform.LinuxEditor:
           case RuntimePlatform.LinuxPlayer:
                sip.horizontalAxis = HorizontalAxisLinux;
                sip.verticalAxis = VerticalAxisLinux;
                break;
        }
    }
}
