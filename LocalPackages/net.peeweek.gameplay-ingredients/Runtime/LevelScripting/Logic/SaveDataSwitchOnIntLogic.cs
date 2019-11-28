using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayIngredients.Logic
{
    public class SaveDataSwitchOnIntLogic : LogicBase
    {
        public GameSaveManager.Location SaveLocation = GameSaveManager.Location.System;
        public string Key = "SomeKey";

        [NonNullCheck]
        public Callable[] DefaultCaseToCall;

        [ReorderableList, NonNullCheck]
        public Callable[] CasesToCall;

        public override void Execute(GameObject instigator = null)
        {
            var gsm = Manager.Get<GameSaveManager>();
            if (gsm.HasInt(Key, SaveLocation))
            {
                int value = gsm.GetInt(Key, SaveLocation);
                if(value > 0 && value < CasesToCall.Length)
                {
                    if (CasesToCall[value] != null)
                        Callable.Call(CasesToCall[value], instigator);
                    else
                    {
                        Debug.LogWarning($"[SaveDataSwitchOnIntLogic] {gameObject.name} : Callable at index #{Key} was null, using default case.");
                        CallDefault(instigator);
                    }
                }
                else
                {
                    Debug.LogWarning($"[SaveDataSwitchOnIntLogic] {gameObject.name} : Callable at index #{Key} was out of range, using default case.");
                    CallDefault(instigator);
                }
            }
            else
            {
                Debug.LogWarning($"[SaveDataSwitchOnIntLogic] {gameObject.name} : Could not Find {Key} in {SaveLocation} Save, using default case.");
                CallDefault(instigator);
            }
        }

        void CallDefault(GameObject instigator)
        {
            if(DefaultCaseToCall != null)
            {
                Callable.Call(DefaultCaseToCall, instigator);
            }
            else
            {
                Debug.LogWarning($"[SaveDataSwitchOnIntLogic] {gameObject.name} : Did not set a default callable, aborting.");
            }
        }

        void WarnNotExist(string name, GameSaveManager.ValueType type, GameSaveManager.Location location)
        {
            Debug.LogWarning(string.Format("Save Data Logic: Trying to get {0} value to non existent {1} data in {2} save.", type, name, location));
        }
    }
}
