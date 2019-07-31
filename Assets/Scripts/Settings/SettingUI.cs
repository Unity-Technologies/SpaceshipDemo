using GameplayIngredients;
using GameplayIngredients.Logic;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    public string SettingName = "Setting";

    public TextMeshProUGUI ValueText;
    public string[] Values;

    private void Start()
    {
        UpdateText();
    }

    public void ToggleSetting()
    {
        Manager.Get<SettingManager>().SetValue(SettingName, (GetValue() + 1) % Values.Length);
        UpdateText();
    }

    void UpdateText()
    {
        ValueText.text = Values[GetValue()];
    }

    int GetValue()
    {
       return Manager.Get<SettingManager>().GetValue(SettingName);
    }

}
