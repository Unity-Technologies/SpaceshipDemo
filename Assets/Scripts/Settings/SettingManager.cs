using GameplayIngredients;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ManagerDefaultPrefab("SettingManager")]
public class SettingManager : Manager
{
    public Setting[] defaultSettings;
    public string OnSettingChangedMessage = "SETTINGS_CHANGED";

    [System.Serializable]
    public struct Setting
    {
        public string name;
        public int defaultValue;
    }

    GameSaveManager m_GameSaveManger;

    private void Start()
    {
        m_GameSaveManger = Get<GameSaveManager>();

        foreach (var setting in defaultSettings)
        {
            SetValue(setting.name, setting.defaultValue, false);
        }
        Messager.Send(OnSettingChangedMessage);
    }

    public void SetValue(string name, int value, bool sendMessage = true)
    {
        m_GameSaveManger.SetInt(name, GameSaveManager.Location.System, value);

        if (sendMessage)
            Messager.Send(OnSettingChangedMessage);
    }

    public int GetValue(string name)
    {
        if (m_GameSaveManger == null)
            m_GameSaveManger = Get<GameSaveManager>();

        return m_GameSaveManger.GetInt(name, GameSaveManager.Location.System);
    }
}
