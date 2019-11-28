using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayIngredients.Actions
{
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

        bool isString() { return valueType == GameSaveManager.ValueType.String; }
        bool isBool() { return valueType == GameSaveManager.ValueType.Bool; }
        bool isInt() { return valueType == GameSaveManager.ValueType.Int; }
        bool isFloat() { return valueType == GameSaveManager.ValueType.Float; }
    }
}
