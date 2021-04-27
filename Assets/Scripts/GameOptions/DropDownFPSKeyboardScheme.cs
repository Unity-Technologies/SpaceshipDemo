using UnityEngine;
using UnityEngine.UI;

namespace GameOptionsUtility
{
    [RequireComponent(typeof(Dropdown))]
    public class DropDownFPSKeyboardScheme : MonoBehaviour
    {
        public string[] Labels = new string[] { "WASD (Default)", "IJKL (Left-Handed)", "ZQSD (AZERTY Keyboard)" };
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
            foreach (var label in Labels)
            {
                dropdown.options.Add(new Dropdown.OptionData(label));
            }

            int current = (int)GameOption.Get<SpaceshipOptions>().fpsKeyboardScheme;
            dropdown.SetValueWithoutNotify(current);
        }

        void UpdateOptions(int value)
        {
            GameOption.Get<SpaceshipOptions>().fpsKeyboardScheme = (SpaceshipOptions.FPSKeyboardScheme)value;
        }
    }

}

