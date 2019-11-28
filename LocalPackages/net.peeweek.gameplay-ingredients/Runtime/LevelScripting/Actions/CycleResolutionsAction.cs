using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace GameplayIngredients.Actions
{
    public class CycleResolutionsAction : ActionBase
    {
        public Text ButtonLabel;

        private int resolution = 3;

        class ResolutionComparer : IEqualityComparer<Resolution>
        {
            public bool Equals(Resolution r1, Resolution r2)
            {
                return r1.width == r2.width && r1.height == r2.height;
            }

            public int GetHashCode(Resolution obj) { return obj.width.GetHashCode() ^ obj.height.GetHashCode(); }
        }

        string getText(Resolution currentResolution)
        {
    #if !UNITY_EDITOR

            if (currentResolution.width == 0 || currentResolution.height == 0)
            {
                Debug.LogError("The selected resolution is not available in the resolution list");
                return "No resolution avaliable";
            }

            return string.Format("Resolution: {0}X{1}", currentResolution.width, currentResolution.height);
    #else
            return "No resolution switch in the editor";
    #endif
        }

        Resolution[] availableResolutions;

        bool IsAllowedAspectRatio(float ratio)
        {
            float[] allowedAspectRatios = new[]{
                16f / 9f,
                16f / 10f,
            };

            foreach (var allowedRatio in allowedAspectRatios)
                if (Mathf.Abs(ratio - allowedRatio) < 0.01f) // We allow 1% of error for in ratio difference due to floating precisions
                    return true;
            return false;
        }

        private void OnEnable()
        {
            availableResolutions = Screen.resolutions
                .Where(r => r.refreshRate == 60 || r.refreshRate == 59) // 60 or 59 fps only
                .Where(r => IsAllowedAspectRatio((float)r.width / (float)r.height))
                .Distinct(new ResolutionComparer()) // remove duplicates
                .ToArray();
        
            // Note: If the player was launched with a resolution which is not in the list, we can't display it
            // If you have this issue while running a player, ensure that you're running the player with an allowed aspect ratio
            var selectedResolution = availableResolutions.FirstOrDefault(f => f.width == Screen.width && f.height == Screen.height);

            if (ButtonLabel != null)
                ButtonLabel.text = getText(selectedResolution);
        }

        public override void Execute(GameObject instigator = null)
        {
            // When we are in the editor we don't have any available resolutions
            if (availableResolutions.Length == 0)
                return ;

            resolution = (resolution + 1) % availableResolutions.Length;

            var selectedResolution = availableResolutions[resolution];
            Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreen);

            if (ButtonLabel != null)
                ButtonLabel.text = getText(selectedResolution);
        }
    }
}
