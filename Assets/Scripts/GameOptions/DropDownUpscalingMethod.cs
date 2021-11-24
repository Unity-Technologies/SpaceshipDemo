using GameOptionsUtility.HDRP;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;

namespace GameOptionsUtility
{
    [RequireComponent(typeof(Dropdown))]
    public class DropDownUpscalingMethod : MonoBehaviour
    {
        [SerializeField]
        HDRPAntiAliasingDropdown antiAliasingDropdown;

        public string[] Methods = new string[5] {
            "Catmull-Rom",
            "Contrast-Adaptive Sharpen",
            "TAA Upsampling",
            "EASU (FSR)",
            "DLSS"
        };
        private void OnEnable()
        {
            var dropdown = GetComponent<Dropdown>();
            InitializeEntries(dropdown);
            dropdown.onValueChanged.AddListener(UpdateOptions);
            UpdateOptions(dropdown.value);
        }

        private void OnDisable()
        {
            GetComponent<Dropdown>().onValueChanged.RemoveListener(UpdateOptions);
        }

        public void InitializeEntries(Dropdown dropdown)
        {
            dropdown.options.Clear();
            foreach (var method in Methods)
            {
                if(!method.Contains("DLSS"))
                    dropdown.options.Add(new Dropdown.OptionData(method));
                else
                {
                    // Add DLSS only if supported
                    if(HDDynamicResolutionPlatformCapabilities.DLSSDetected)
                        dropdown.options.Add(new Dropdown.OptionData(method));
                }
            }

            int current = (int)GameOption.Get<SpaceshipOptions>().upsamplingMethod;
            dropdown.SetValueWithoutNotify(current);
        }

        void UpdateOptions(int value)
        {
            SpaceshipOptions.UpsamplingMethod val = (SpaceshipOptions.UpsamplingMethod)value;

            if (val == SpaceshipOptions.UpsamplingMethod.TAAU)
            {
                GameOption.Get<HDRPCameraOption>().antiAliasing = HDAdditionalCameraData.AntialiasingMode.TemporalAntialiasing;

                // Force Refresh
                if(antiAliasingDropdown != null)
                {
                    antiAliasingDropdown.enabled = false;
                    antiAliasingDropdown.enabled = true;
                }
            }

            GameOption.Get<SpaceshipOptions>().upsamplingMethod = val;
        }
    }
}