using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

namespace GameplayIngredients
{
    public class TimerDisplayRig : MonoBehaviour
    {
        [NonNullCheck]
        public Text text;
        [NonNullCheck]
        public TextMesh textMesh;


        [NonNullCheck]
        public Timer timer;

        [InfoBox("Use the following wildcards:\n - %h : hours\n - %m : minutes\n - %s : seconds\n - %x : milliseconds", InfoBoxType.Normal)]
        public string format = "%h:%m:%s:%x";

        private void OnValidate()
        {
            UpdateText();
        }

        private void Reset()
        {
            UpdateText();
        }

        private void Update()
        {
            if (timer == null || (text == null && textMesh == null))
                return;

            UpdateText();
        }

        void UpdateText()
        {
            var value = format;

            uint hours = timer != null ? timer.CurrentHours: 0;
            uint minutes = timer != null ? timer.CurrentMinutes : 0;
            uint seconds = timer != null ? timer.CurrentSeconds : 0;
            uint milliseconds = timer != null ? timer.CurrentMilliseconds : 0;

            value = value.Replace("%h", hours.ToString("D2"));
            value = value.Replace("%m", minutes.ToString("D2"));
            value = value.Replace("%s", seconds.ToString("D2"));
            value = value.Replace("%x", milliseconds.ToString("D3"));

            if (text != null)
                text.text = value;

            if (textMesh != null)
                textMesh.text = value;
        }
    }
}

