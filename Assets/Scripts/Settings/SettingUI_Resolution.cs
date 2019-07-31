using GameplayIngredients;
using GameplayIngredients.Logic;
using NaughtyAttributes;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingUI_Resolution : MonoBehaviour
{
    public string SettingXName = "Resolution.X";
    public string SettingYName = "Resolution.Y";

    public TextMeshProUGUI ValueText;

    public Vector2Int[] Values;

    Vector2Int nativeValue;
    Vector2Int currentValue;
    int index = -1;

    private void OnEnable()
    {
        currentValue = GetValue();
        index = GetIndex();

        nativeValue = GetNativeResolution();
        UpdateText();
    }

    int GetIndex()
    {
        if (Values.Select(o => o.x == currentValue.x && o.y == currentValue.y).Count() > 0)
            return Values.ToList().IndexOf(currentValue);
        else
            return -1;
    }

    public void ToggleSetting()
    {
        do
        {
            index++;
            if (index == Values.Length)
                index = -1;

            currentValue = index == -1 ? nativeValue : Values[index];
        }
        while (!IsSupportedResolution(currentValue));

        Manager.Get<SettingManager>().SetValue(SettingXName, currentValue.x );
        Manager.Get<SettingManager>().SetValue(SettingYName, currentValue.y );
        UpdateText();
    }

    void UpdateText()
    {
        if (index == -1)
        {
            ValueText.text = nativeValue.x + "x" + nativeValue.y + " (Native)";
        }
        else
        {
            var value = GetValue();
            ValueText.text = value.x + "x" + value.y;
        }
    }

    bool IsSupportedResolution(Vector2Int resolution)
    {
        return (Screen.resolutions.Any(o => o.width == resolution.x && o.height == resolution.y));
    }

    Vector2Int GetNativeResolution()
    {
        var last = Screen.resolutions.Last();
        return new Vector2Int(last.width, last.height);
    }

    Vector2Int GetValue()
    {
        if(index == -1)
        {
            return nativeValue;
        }
        else
        {
            var mgr = Manager.Get<SettingManager>();
            return new Vector2Int(mgr.GetValue(SettingXName), mgr.GetValue(SettingYName));
        }
    }
}
