using GameplayIngredients;
using GameplayIngredients.Logic;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnSettingLogic : LogicBase
{
    public GameSaveManager.Location Location = GameSaveManager.Location.System;
    public string SettingName = "Setting";
    [ReorderableList]
    public Callable[] OnSetting;

    public override void Execute(GameObject instigator = null)
    {
        var gsm = Manager.Get<GameSaveManager>();
        if(gsm.HasInt(SettingName, Location))
        {
            int value = gsm.GetInt(SettingName, Location);
            if(OnSetting.Length > value)
            {
                Call(OnSetting[value]);
            }
        }
    }
}
