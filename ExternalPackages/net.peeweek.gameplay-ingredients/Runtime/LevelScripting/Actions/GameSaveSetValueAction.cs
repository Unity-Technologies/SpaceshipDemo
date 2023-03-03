using NaughtyAttributes;
using UnityEngine;

namespace GameplayIngredients.Actions
{
    [AddComponentMenu(ComponentMenu.actionsPath + "Game Save : Set Value Action")]
    [Callable("Data", "Actions/ic-action-save.png")]
    public class GameSaveSetValueAction : ActionBase
    {
        public string Key = "SomeKey";
        public GameSaveManager.Location saveLocation = GameSaveManager.Location.System;
        public GameSaveManager.ValueType valueType = GameSaveManager.ValueType.String;
        
        [ShowIf("isString")]
        public string StringValue;
        [ShowIf("isInt")]
        public int IntValue;
        [ShowIf("isBool")]
        public bool BoolValue;
        [ShowIf("isFloat")]
        public float FloatValue;

        public override void Execute(GameObject instigator = null)
        {
            var gsm = Manager.Get<GameSaveManager>();
            switch(valueType)
            {
                case GameSaveManager.ValueType.Bool: gsm.SetBool(Key, saveLocation, BoolValue); break;
                case GameSaveManager.ValueType.Int: gsm.SetInt(Key, saveLocation, IntValue); break;
                case GameSaveManager.ValueType.Float: gsm.SetFloat(Key, saveLocation, FloatValue); break;
                case GameSaveManager.ValueType.String: gsm.SetString(Key, saveLocation, StringValue); break;
            }
        }

        public override string GetDefaultName()
        {
            string value = "";
            switch (valueType)
            {
                case GameSaveManager.ValueType.Bool: value = BoolValue.ToString(); break;
                case GameSaveManager.ValueType.Int: value = IntValue.ToString(); break;
                case GameSaveManager.ValueType.Float: value = FloatValue.ToString(); break;
                case GameSaveManager.ValueType.String: value = StringValue.ToString(); break;
            }
            return $"Game Save Set {valueType} '{Key}' = {value}";
        }

        bool isString() { return valueType == GameSaveManager.ValueType.String; }
        bool isBool() { return valueType == GameSaveManager.ValueType.Bool; }
        bool isInt() { return valueType == GameSaveManager.ValueType.Int; }
        bool isFloat() { return valueType == GameSaveManager.ValueType.Float; }
    }
}
