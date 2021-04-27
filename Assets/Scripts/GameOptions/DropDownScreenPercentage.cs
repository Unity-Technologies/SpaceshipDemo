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
                dropdown.options.Add(new Dropdown.OptionData($"{percentage}%"));
            }

            int current = GameOption.Get<SpaceshipOptions>().screenPercentage;
            dropdown.SetValueWithoutNotify(Percentages.ToList().FindIndex(o => o == current));
        }

        void UpdateOptions(int value)
        {
            GameOption.Get<SpaceshipOptions>().screenPercentage = Percentages[value];
        }
    }

}

