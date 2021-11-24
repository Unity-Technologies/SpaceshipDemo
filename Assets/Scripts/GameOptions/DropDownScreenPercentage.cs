using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameOptionsUtility
{
    [RequireComponent(typeof(Dropdown))]
    public class DropDownScreenPercentage : MonoBehaviour
    {
        public int[] Percentages = new int[6] { 100, 90, 80, 70, 60, 50 };
        public Dropdown upsamplingDropdown;

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
            foreach (var percentage in Percentages)
            {
                dropdown.options.Add(new Dropdown.OptionData($"{percentage}% {(percentage == 100?"(Native)":"")}"));
            }

            int current = GameOption.Get<SpaceshipOptions>().screenPercentage;
            dropdown.SetValueWithoutNotify(Percentages.ToList().FindIndex(o => o == current));
        }

        void UpdateOptions(int value)
        {
            int val = Percentages[value];
            GameOption.Get<SpaceshipOptions>().screenPercentage = val;

            upsamplingDropdown.interactable = (val < 100);
            upsamplingDropdown.captionText.CrossFadeAlpha(val == 100 ? 0.1f : 1.0f, upsamplingDropdown.colors.fadeDuration, true);
        }
    }

}

